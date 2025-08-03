#nullable enable

using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Common.Enums;
using WebApp.Server.Features.BackgroundJobLogging;
using WebApp.Server.Features.Game;
using WebApp.Server.OpenAIPlugins;

namespace WebApp.Server.Jobs;

public sealed class SummarizeLastWeeksResults
{
    private readonly Kernel _kernel;
    private readonly IMediator _mediator;
    private readonly CreateBackgroundJobLog.Command _logCommand;

    public SummarizeLastWeeksResults(Kernel kernel, IMediator mediator)
    {
        _kernel = kernel;
        _mediator = mediator;
        _logCommand = new CreateBackgroundJobLog.Command
        {
            BackgroundJobName = nameof(SummarizeLastWeeksResults),
            Started = DateTimeOffset.Now
        };
    }

    public async Task Process(CancellationToken cancellationToken = default)
    {
        try
        {
            var (lastGameId, secondToLastGameId) = await GetRecentGames(cancellationToken);

            if (lastGameId.HasValue)
            {
                var shouldRun = await ShouldRunJob(lastGameId.Value, cancellationToken);

                if (shouldRun)
                {
                    var summary = await SummarizeResults(lastGameId.Value, secondToLastGameId, cancellationToken);

                    await CreateGameSummary(lastGameId.Value, summary, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            AddError(ex.Message, ex.StackTrace);
        }

        await _mediator.Send(_logCommand, CancellationToken.None);
    }

    private async Task<bool> ShouldRunJob(int gameId, CancellationToken cancellationToken)
    {
        var gameSummaryResult = await _mediator.Send(new GetGameSummary.Query()
        {
            GameId = gameId
        }, cancellationToken);

        bool shouldRunJob = gameSummaryResult.Match
        (
            success => false,
            validationProblem =>
            {
                AddError("Failed to query game summaries.", validationProblem);
                return false;
            },
            notFound => true
        );

        return shouldRunJob;
    }

    private async Task<(int? LastGameId, int? SecondToLastGameId)> GetRecentGames(CancellationToken cancellationToken)
    {
        var searchGamesResult = await _mediator.Send(new GameSearch.Query
        {
            SeasonId = SeasonConstants.CURRENT_SEASON_ID,
            TeamId = SeasonConstants.CURRENT_TEAM_ID,
            IsGameComplete = true,
            WeekType = WeekType.RegularSeason,
        }, cancellationToken);

        var games = searchGamesResult.Match
        (
            success => success.Games,
            validationProblem =>
            {
                AddError("Failed to search for games.", validationProblem);
                return [];
            }
        );

        var ordered = games.OrderBy(x => x.GameStartsOn).ToArray();

        return ordered.Length switch
        {
            > 1 => (ordered[^1].GameId, ordered[^2].GameId),
            1 => (ordered[0].GameId, null),
            _ => (null, null)
        };
    }

    private async Task<string> SummarizeResults(int LastGameId, int? SecondToLastGameId, CancellationToken cancellationToken)
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        var history = new ChatHistory();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Required()
        };

        string inputPrompt =
        $"""
        Summarize the user prediction results for the most recently completed football game. Use the 
        {nameof(UserGamePredictionPlugin.UserPrediction.ScoreDifferential)} property to determine who 
        made the most accurate prediction. Lower values are better, and a value of zero means a perfect 
        prediction. The summary should be a single paragraph of at least five sentences. Congratulate 
        the winner, referencing their previous week's results if available. Be sure to poke fun at the loser. 
        It’s all in good fun between friends so tone should be funny, witty, and just a bit mean. Use
        grandiloquent language and only use hyphens only when absolutely necessary.

        Assume the following:
        - Current TeamId: {SeasonConstants.CURRENT_TEAM_ID}
        - Current SeasonId: {SeasonConstants.CURRENT_SEASON_ID}
        - Most recently completed GameId: {LastGameId}
        - Game before the recently completed GameId: {SecondToLastGameId}
        """;

        history.AddUserMessage(inputPrompt);

        var result = await chatCompletionService.GetChatMessageContentAsync(history, executionSettings, _kernel, cancellationToken);

        history.AddMessage(result.Role, result.Content ?? string.Empty);

        return result.Content ?? string.Empty;
    }

    private async Task CreateGameSummary(int gameId, string summary, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateGameSummary.Command
        {
            GameId = gameId,
            Summary = summary
        }, cancellationToken);

        result.Match
        (
            success => Unit.Value,
            validationProblem =>
            {
                AddError("Failed to create game summary.", validationProblem);
                return Unit.Value;
            },
            conflictProblem =>
            {
                AddError("Game summary already exists.");
                return Unit.Value;
            }
        );
    }

    private void AddError(string message, string? stackTrace = null)
    {
        _logCommand.Errors.Add(new CreateBackgroundJobLog.Command.Error
        {
            Message = message,
            ValidationErrors = null,
            StackTrace = stackTrace
        });
    }

    private void AddError(string message, ValidationProblemDetails validationProblemDetails)
    {
        _logCommand.Errors.Add(new CreateBackgroundJobLog.Command.Error
        {
            Message = message,
            ValidationErrors = validationProblemDetails.Errors,
            StackTrace = null
        });
    }
}

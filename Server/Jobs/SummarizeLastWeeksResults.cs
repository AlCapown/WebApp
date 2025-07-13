#nullable enable

using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Constants;
using WebApp.Server.Features.Game;
using WebApp.Server.OpenAIPlugins;

namespace WebApp.Server.Jobs;

public sealed class SummarizeLastWeeksResults
{
    private readonly IChatCompletionService _chatCompletionService;
    private readonly Kernel _kernel;
    private readonly ILogger<SummarizeLastWeeksResults> _logger;
    private readonly IMediator _mediator;

    public SummarizeLastWeeksResults(
        IChatCompletionService chatCompletionService, 
        Kernel kernel, 
        ILogger<SummarizeLastWeeksResults> logger,
        IMediator mediator)
    {
        _chatCompletionService = chatCompletionService;
        _kernel = kernel;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Process(CancellationToken cancellationToken = default)
    {
        var (lastGameId, secondToLastGameId) = await GetRecentGames(cancellationToken);

        var shouldRun = await ShouldRunJob(cancellationToken);
        if (!shouldRun)
        {
            return;
        }

        await SummarizeResults(lastGameId, secondToLastGameId, cancellationToken);
    }

    private Task<bool> ShouldRunJob(CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    private async Task<(int LastGameId, int SecondToLastGameId)> GetRecentGames(CancellationToken cancellationToken)
    {
        var searchGamesResult = await _mediator.Send(new GameSearch.Query
        {
            SeasonId = SeasonConstants.CURRENT_SEASON_ID,
            TeamId = SeasonConstants.CURRENT_TEAM_ID,
            IsGameComplete = true,
        }, cancellationToken);

        var games = searchGamesResult.Match
        (
            success => success,
            validationProblem => throw new InvalidOperationException(validationProblem.Detail)
        );

        var ordered = games.Games.OrderBy(x => x.GameStartsOn).ToArray();

        return (ordered[^1].GameId, ordered[^2].GameId);
    }

    private async Task SummarizeResults(int LastGameId, int SecondToLastGameId, CancellationToken cancellationToken)
    {
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
        grandiloquent language and use hyphens only when absolutely necessary.

        Assume the following:
        - Current TeamId: {SeasonConstants.CURRENT_TEAM_ID}
        - Current SeasonId: {SeasonConstants.CURRENT_SEASON_ID}
        - Most recently completed GameId: {LastGameId}
        - Previous completed GameId: {SecondToLastGameId}
        """;

        history.AddUserMessage(inputPrompt);

        var result = await _chatCompletionService.GetChatMessageContentAsync(history, executionSettings, _kernel, cancellationToken);

        history.AddMessage(result.Role, result.Content ?? string.Empty);

        _logger.LogInformation(
            "SummarizeLastWeeksResults: {Result}",
            result.Content ?? "No content returned from chat completion service."
        );
    }
}

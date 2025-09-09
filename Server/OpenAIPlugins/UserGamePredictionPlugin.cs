#nullable enable

using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Common.Models;
using WebApp.Server.Features.Game;
using WebApp.Server.Features.GamePrediction;

namespace WebApp.Server.OpenAIPlugins;

public sealed class UserGamePredictionPlugin
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserGamePredictionPlugin> _logger;

    public UserGamePredictionPlugin(IMediator mediator, ILogger<UserGamePredictionPlugin> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [KernelFunction]
    [Description("Get user game predictions for football games")]
    [return: Description("A list of games with user predictions for each game")]
    public async Task<List<UserPredictionItem>> SearchGamePredictionAsync
    (
        [Description("Season unique identifier")]
        int seasonId,
        [Description("Searches game and predictions where either the home team or away team match this unique team identifier")]
        int teamId,
        [Description("Optional cancellation token for the operation")]
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("{Plugin} was called with parameters SeasonId: {SeasonId} TeamId: {TeamId}", nameof(SearchGamePredictionAsync), seasonId, teamId);

        var gameSearch = await GetGamesAsync(seasonId, teamId, cancellationToken);
        var gamePredictionSearch = await GetGamePredictionsAsync(seasonId, teamId, cancellationToken);
        return BuildUserPredictionItems(gameSearch, gamePredictionSearch);
    }

    private async ValueTask<GameSearchResponse> GetGamesAsync(int seasonId, int teamId, CancellationToken cancellationToken)
    {
        var gameSearch = await _mediator.Send(new GameSearch.Query
        {
            SeasonId = seasonId,
            TeamId = teamId
        }, cancellationToken);

        return gameSearch.Match
        (
            success => success,
            validationError => throw new InvalidOperationException(validationError.Detail)
        );
    }

    private async ValueTask<GamePredictionSearchResponse> GetGamePredictionsAsync(int seasonId, int teamId, CancellationToken cancellationToken)
    {
        var gamePredictionSearch = await _mediator.Send(new GamePredictionSearch.Query
        {
            SeasonId = seasonId,
            TeamId = teamId
        }, cancellationToken);

        return gamePredictionSearch.Match
        (
            success => success,
            validationError => throw new InvalidOperationException(validationError.Detail),
            notFound => throw new InvalidOperationException(notFound.Detail)
        );
    }

    private static List<UserPredictionItem> BuildUserPredictionItems(GameSearchResponse gameSearchResponse, GamePredictionSearchResponse gamePredictionSearchResponse)
    {
        var games = gameSearchResponse.Games
            .OrderBy(x => x.Week)
            .ToArray();

        var users = gamePredictionSearchResponse.GamePredictions
            .DistinctBy(x => x.UserId)
            .ToArray();

        var predictionLookup = gamePredictionSearchResponse.GamePredictions
            .ToDictionary(key => (key.UserId, key.GameId), value => value);

        var result = new List<UserPredictionItem>(games.Length);

        foreach (var game in games)
        {
            var userPredictions = new List<UserPrediction>(users.Length);

            foreach (var user in users)
            {
                predictionLookup.TryGetValue((user.UserId, game.GameId), out var gamePrediction);

                int scoreDifferential =
                    Math.Abs(game.HomeTeamScore - (gamePrediction?.PredictedHomeTeamScore ?? 0)) +
                    Math.Abs(game.AwayTeamScore - (gamePrediction?.PredictedAwayTeamScore ?? 0));

                bool predictedWinningTeam =
                    game.HomeTeamScore != game.AwayTeamScore &&
                    gamePrediction != null &&
                    gamePrediction.PredictedHomeTeamScore != gamePrediction.PredictedAwayTeamScore &&
                    (
                        (game.HomeTeamScore > game.AwayTeamScore && gamePrediction.PredictedHomeTeamScore > gamePrediction.PredictedAwayTeamScore) ||
                        (game.HomeTeamScore < game.AwayTeamScore && gamePrediction.PredictedHomeTeamScore < gamePrediction.PredictedAwayTeamScore)
                    );

                userPredictions.Add(new UserPrediction
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PredictedHomeTeamScore = gamePrediction?.PredictedHomeTeamScore,
                    PredictedAwayTeamScore = gamePrediction?.PredictedAwayTeamScore,
                    ScoreDifferential = scoreDifferential,
                    PredictedWinningTeam = predictedWinningTeam
                });
            }

            result.Add(new UserPredictionItem
            {
                GameId = game.GameId,
                Week = game.Week,
                HomeTeamId = game.HomeTeamId,
                HomeTeamName = game.HomeTeamName,
                HomeTeamScore = game.HomeTeamScore,
                AwayTeamId = game.AwayTeamId,
                AwayTeamName = game.AwayTeamName,
                AwayTeamScore = game.AwayTeamScore,
                GameStartsOn = game.GameStartsOn,
                IsComplete = game.IsComplete,
                UserPredictions = userPredictions
            });
        }

        return result;
    }

    public sealed record UserPredictionItem
    {
        [Description("The unique identifier for the football game.")]
        public int GameId { get; init; }

        [Description("The week number of the football season.")]
        public int Week { get; init; }

        [Description("The unique identifier for the home team.")]
        public int HomeTeamId { get; init; }

        [Description("The name of the home team.")]
        public required string HomeTeamName { get; init; }

        [Description("The final score for the home team.")]
        public int HomeTeamScore { get; init; }

        [Description("The unique identifier for the away team.")]
        public int AwayTeamId { get; init; }

        [Description("The name of the away team.")]
        public required string AwayTeamName { get; init; }

        [Description("The final score for the away team.")]
        public int AwayTeamScore { get; init; }

        [Description("The scheduled start date and time of the football game.")]
        public DateTimeOffset? GameStartsOn { get; init; }

        [Description("Indicates if the football game is complete.")]
        public bool IsComplete { get; init; }

        [Description("A list of individual user predictions for this game.")]
        public required List<UserPrediction> UserPredictions { get; init; }
    }

    public sealed record UserPrediction
    {
        [Description("The unique identifier for the user.")]
        public required string UserId { get; init; }

        [Description("The first name of the user.")]
        public required string FirstName { get; init; }

        [Description("The last name of the user.")]
        public required string LastName { get; init; }

        [Description("The predicted score for the home team by the user. Is NULL when the user did not provide a prediction.")]
        public int? PredictedHomeTeamScore { get; init; }

        [Description("The predicted score for the away team by the user. Is NULL when the user did not provide a prediction.")]
        public int? PredictedAwayTeamScore { get; init; }

        [Description("The absolute difference between the predicted and actual scores for both teams. The lower the value the more accurate the prediction.")]
        public int ScoreDifferential { get; init; }

        [Description("Indicates if the user correctly predicted the winning team.")]
        public bool PredictedWinningTeam { get; init; }
    }
}

#nullable enable

using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Store.Shared;
using WebApp.Common.Enums;
using WebApp.Common.Models;

namespace WebApp.Client.Store.GameStore;

public sealed class SearchGamesForSeasonEffect : Effect<GameActions.SearchGamesForSeason>
{
    private readonly IApiClient _client;

    public SearchGamesForSeasonEffect(IApiClient apiClient)
    {
        _client = apiClient;
    }

    public override async Task HandleAsync(GameActions.SearchGamesForSeason action, IDispatcher dispatcher)
    {
        await _client.GetAsync(new SearchGamesForSeasonPlan(action), "/Api/Game/Search", new Dictionary<string, string?>
        {
            { nameof(action.SeasonWeekId), action.SeasonWeekId?.ToString() },
            { nameof(action.SeasonId), action.SeasonId?.ToString() },
            { nameof(action.GameId), action.GameId?.ToString() },
            { nameof(action.TeamId), action.TeamId?.ToString() },
            { nameof(action.GameStartsOnMin), action.GameStartsOnMin?.ToString() },
            { nameof(action.GameStartsOnMax), action.GameStartsOnMax?.ToString() },
            { nameof(action.IsGameComplete), action.IsGameComplete?.ToString() },
            { nameof(action.WeekType), action.WeekType?.ToString() },
            { nameof(action.HasSummary), action.HasSummary?.ToString() }
        });
    }

    private sealed class SearchGamesForSeasonPlan : ApiLoadPlan<GameSearchResponse>
    {
        public SearchGamesForSeasonPlan(GameActions.SearchGamesForSeason action) : base(action) { }

        public override JsonTypeInfo<GameSearchResponse> ResponseBodyJsonContext => 
            GameJsonContext.Default.GameSearchResponse;

        public override FetchSuccessAction GetSuccessAction(GameSearchResponse response) =>
            new GameActions.SearchGamesForSeasonSuccess
            {
                Games = response.Games
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new GameActions.SearchGamesForSeasonFailure();
    }
}

public static partial class GameActions
{
    public sealed record SearchGamesForSeason : FetchStartedAction
    {
        public int? SeasonId { get; init; }
        public int? SeasonWeekId { get; init; }
        public int? GameId { get; init; }
        public int? TeamId { get; init; }
        public DateTimeOffset? GameStartsOnMin { get; init; }
        public DateTimeOffset? GameStartsOnMax { get; init; }
        public bool? IsGameComplete { get; init; }
        public WeekType? WeekType { get; init; }
        public bool? HasSummary { get; init; }
    }

    public sealed record SearchGamesForSeasonSuccess : FetchSuccessAction
    {
        public required Game[] Games { get; init; }
    }

    public sealed record SearchGamesForSeasonFailure : FetchFailureAction { }
}

public sealed class SearchGamesForSeasonSuccessReducer : Reducer<GameState, GameActions.SearchGamesForSeasonSuccess>
{
    public override GameState Reduce(GameState state, GameActions.SearchGamesForSeasonSuccess action) =>
        state with
        {
            Games = state.Games.SetItems(action.Games.ToDictionary(key => key.GameId))
        };
}

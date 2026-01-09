#nullable enable

using Fluxor;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Common.Extensions;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.GamePredictionStore;

public sealed class GamePredictionSearchEffect : Effect<GamePredictionActions.GamePredictionSearch>
{
    private readonly IApiClient _client;

    public GamePredictionSearchEffect(IApiClient apiClient)
    {
        _client = apiClient;
    }

    public override async Task HandleAsync(GamePredictionActions.GamePredictionSearch action, IDispatcher dispatcher)
    {
        await _client.GetAsync(new GamePredictionSearchPlan(action), "api/GamePrediction/Search", 
        [
            new(nameof(action.SeasonId), action.SeasonId?.ToString()),
            new(nameof(action.SeasonWeekId), action.SeasonWeekId?.ToString()),
            new(nameof(action.GameId), action.GameId?.ToString()),
            new(nameof(action.TeamId), action.TeamId?.ToString()),
            new(nameof(action.UserId), action.UserId),
            new(nameof(action.LimitToCurrentUser), action.LimitToCurrentUser?.ToString())
        ]);
    }

    private sealed class GamePredictionSearchPlan : ApiLoadPlan<GamePredictionSearchResponse>
    {
        public GamePredictionSearchPlan(GamePredictionActions.GamePredictionSearch action) 
            : base(action) { }

        public override JsonTypeInfo<GamePredictionSearchResponse> ResponseBodyJsonContext =>
            GamePredictionSearchResponseJsonContext.Default.GamePredictionSearchResponse;

        public override FetchSuccessAction GetSuccessAction(GamePredictionSearchResponse response) =>
            new GamePredictionActions.GamePredictionSearchSuccess
            {
                GamePredictions = response.GamePredictions,
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new GamePredictionActions.GamePredictionSearchFailure();
    }
}


public static partial class GamePredictionActions
{
    public sealed record GamePredictionSearch : FetchStartedAction
    {
        public int? SeasonId { get; init; }
        public int? SeasonWeekId { get; init; }
        public int? GameId { get; init; }
        public string? UserId { get; init; }
        public bool? LimitToCurrentUser { get; init; }
        public int? TeamId { get; init; }
    }

    public sealed record GamePredictionSearchSuccess : FetchSuccessAction
    {
        public GamePrediction[] GamePredictions { get; init; } = [];
    }

    public sealed record GamePredictionSearchFailure : FetchFailureAction { }
}


public sealed class GamePredictionSearchSuccessReducer : Reducer<GamePredictionState, GamePredictionActions.GamePredictionSearchSuccess>
{
    public override GamePredictionState Reduce(GamePredictionState state, GamePredictionActions.GamePredictionSearchSuccess action) => 
        state with
        {
            GamePredictions = state.GamePredictions.SetItems(action.GamePredictions.ToKeyValuePairs(key => key.GamePredictionId))
        };
}
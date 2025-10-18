#nullable enable

using Fluxor;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.GamePredictionStore;

public sealed class GetGamePredictionEffect : Effect<GamePredictionActions.GetGamePrediction>
{
    private readonly IApiClient _client;

    public GetGamePredictionEffect(IApiClient apiClient)
    {
        _client = apiClient;
    }

    public override async Task HandleAsync(GamePredictionActions.GetGamePrediction action, IDispatcher dispatcher)
    {
        await _client.GetAsync(new GetGamePredictionPlan(action), $"api/GamePrediction/{action.GamePredictionId}");
    }

    private sealed class GetGamePredictionPlan : ApiLoadPlan<GetGamePredictionResponse>
    {
        public GetGamePredictionPlan(GamePredictionActions.GetGamePrediction action) 
            : base(action) { }

        public override JsonTypeInfo<GetGamePredictionResponse> ResponseJsonContext =>
            GetGamePredictionResponseJsonContext.Default.GetGamePredictionResponse;

        public override FetchSuccessAction GetSuccessAction(GetGamePredictionResponse response) =>
            new GamePredictionActions.GetGamePredictionSuccess
            {
                GamePrediction = response.GamePrediction
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new GamePredictionActions.GetGamePredictionFailure();
    }
}

public static partial class GamePredictionActions
{
    public sealed record GetGamePrediction : FetchStartedAction
    {
        public int GamePredictionId { get; init; }
    }

    public sealed record GetGamePredictionSuccess : FetchSuccessAction
    {
        public required GamePrediction GamePrediction { get; init; }
    }

    public sealed record GetGamePredictionFailure : FetchFailureAction { }
}

public sealed class GetGamePredictionSuccessReducer : Reducer<GamePredictionState, GamePredictionActions.GetGamePredictionSuccess>
{
    public override GamePredictionState Reduce(GamePredictionState state, GamePredictionActions.GetGamePredictionSuccess action) => state with
    {
        GamePredictions = state.GamePredictions.SetItem(action.GamePrediction.GamePredictionId, action.GamePrediction)
    };
}



using Fluxor;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Common.Extensions;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.GamePredictionStore;

public sealed class CreateGamePredictionEffect : Effect<GamePredictionActions.CreateGamePrediction>
{
    private readonly IApiClient _client;

    public CreateGamePredictionEffect(IApiClient apiClient)
    {
        _client = apiClient;
    }

    public override async Task HandleAsync(GamePredictionActions.CreateGamePrediction action, IDispatcher dispatcher)
    {
        var result = await _client.PostAsync(new CreateGamePredictionPlan(action), $"api/GamePrediction", action.Request);

        if (result.IsSuccess)
        {
            dispatcher.DispatchFetch(new GamePredictionActions.GetGamePrediction
            {
                GamePredictionId = result.Response.GamePredictionId,
                FetchOptions = FetchOptions.SilentRefresh
            });
        }
    }

    private sealed class CreateGamePredictionPlan : ApiLoadPlanWithBody<CreateGamePredictionResponse, CreateGamePredictionRequest>
    {
        public CreateGamePredictionPlan(GamePredictionActions.CreateGamePrediction action) 
            : base(action) { }

        public override JsonTypeInfo<CreateGamePredictionResponse> ResponseJsonContext =>
            CreateGamePredictionResponseJsonContext.Default.CreateGamePredictionResponse;

        public override JsonTypeInfo<CreateGamePredictionRequest> BodyJsonContext =>
            CreateGamePredictionRequestJsonContext.Default.CreateGamePredictionRequest;

        public override FetchSuccessAction GetSuccessAction(CreateGamePredictionResponse response) =>
            new GamePredictionActions.CreateGamePredictionSuccess
            {
                GamePredictionId = response.GamePredictionId
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new GamePredictionActions.CreateGamePredictionFailure();
    }
}

public static partial class GamePredictionActions
{
    public sealed record CreateGamePrediction : FetchStartedAction
    {
        public CreateGamePredictionRequest Request { get; init; }
    }

    public sealed record CreateGamePredictionSuccess : FetchSuccessAction 
    {
        public int GamePredictionId { get; init; }
    }

    public sealed record CreateGamePredictionFailure : FetchFailureAction { }
}
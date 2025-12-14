#nullable enable

using Fluxor;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Common.Extensions;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.GamePredictionStore;

public sealed class CreateGamePredictionForUserEffect : Effect<GamePredictionActions.CreateGamePredictionForUser>
{
    private readonly IApiClient _client;

    public CreateGamePredictionForUserEffect(IApiClient apiClient)
    {
        _client = apiClient;
    }

    public override async Task HandleAsync(GamePredictionActions.CreateGamePredictionForUser action, IDispatcher dispatcher)
    {
        var result = await _client.PostAsync(new CreateGamePredictionForUserPlan(action), $"api/GamePrediction/{action.UserId}", action.Request);

        if (result.IsSuccess)
        {
            dispatcher.DispatchFetch(new GamePredictionActions.GetGamePrediction
            {
                GamePredictionId = result.Response.GamePredictionId,
                FetchOptions = FetchOptions.SilentRefresh,
            });
        }
    }

    private sealed class CreateGamePredictionForUserPlan : ApiLoadPlanWithBody<CreateGamePredictionResponse, CreateGamePredictionRequest>
    {
        public CreateGamePredictionForUserPlan(GamePredictionActions.CreateGamePredictionForUser action) 
            : base(action) { }

        public override JsonTypeInfo<CreateGamePredictionResponse> ResponseBodyJsonContext =>
            CreateGamePredictionResponseJsonContext.Default.CreateGamePredictionResponse;

        public override JsonTypeInfo<CreateGamePredictionRequest> RequestBodyJsonContext =>
            CreateGamePredictionRequestJsonContext.Default.CreateGamePredictionRequest;

        public override FetchSuccessAction GetSuccessAction(CreateGamePredictionResponse response) =>
            new GamePredictionActions.CreateGamePredictionForUserSuccess
            {
                GamePredictionId = response.GamePredictionId
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new GamePredictionActions.CreateGamePredictionForUserFailure();
    }
}

public static partial class GamePredictionActions
{
    public sealed record CreateGamePredictionForUser : FetchStartedAction
    {
        public required string UserId { get; init; }
        public required CreateGamePredictionRequest Request { get; init; }
    }

    public sealed record CreateGamePredictionForUserSuccess : FetchSuccessAction 
    {
        public int GamePredictionId { get; init; }
    }

    public sealed record CreateGamePredictionForUserFailure : FetchFailureAction { }
}

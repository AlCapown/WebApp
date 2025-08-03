#nullable enable

using Fluxor;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.GameSummaryStore;

public sealed class LoadGameSummaryEffect : Effect<GameSummaryActions.LoadGameSummary>
{
    private readonly IApiClient _client;

    public LoadGameSummaryEffect(IApiClient apiClient)
    {
        _client = apiClient;
    }

    public override async Task HandleAsync(GameSummaryActions.LoadGameSummary action, IDispatcher dispatcher)
    {
        await _client.GetAsync(new GetGameSummaryPlan(action), $"/Api/Game/{action.GameId}/Summary");
    }

    private sealed class GetGameSummaryPlan : ApiLoadPlan<GameSummary>
    {
        public GetGameSummaryPlan(GameSummaryActions.LoadGameSummary action) : base(action) { }

        public override JsonTypeInfo<GameSummary> ResponseJsonContext => 
            GameSummaryJsonContext.Default.GameSummary;

        public override FetchSuccessAction GetSuccessAction(GameSummary response) =>
            new GameSummaryActions.LoadGameSummarySuccess
            {
                GameSummary = response
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new GameSummaryActions.LoadGameSummaryFailure();
    }
}

public static partial class GameSummaryActions
{
    public sealed record LoadGameSummary : FetchStartedAction
    {
        public int GameId { get; init; }
    }

    public sealed record LoadGameSummarySuccess : FetchSuccessAction
    {
        public required GameSummary GameSummary { get; init; }
    }

    public sealed record LoadGameSummaryFailure : FetchFailureAction { }
}

public sealed class LoadGameSummarySuccessReducer : Reducer<GameSummaryState, GameSummaryActions.LoadGameSummarySuccess>
{
    public override GameSummaryState Reduce(GameSummaryState state, GameSummaryActions.LoadGameSummarySuccess action) =>
        state with
        {
            GameSummaries = state.GameSummaries.SetItem(action.GameSummary.GameId, action.GameSummary)
        };
}

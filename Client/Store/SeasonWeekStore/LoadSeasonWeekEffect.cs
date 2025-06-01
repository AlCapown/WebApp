using Fluxor;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.SeasonWeekStore;

public sealed class LoadSeasonWeekEffect : Effect<SeasonWeekActions.LoadSeasonWeek>
{
    private readonly IApiClient _client;

    public LoadSeasonWeekEffect(IApiClient apiClient)
    {
        _client = apiClient;
    }

    public override async Task HandleAsync(SeasonWeekActions.LoadSeasonWeek action, IDispatcher dispatcher)
    {
        await _client.GetAsync(new LoadSeasonWeekPlan(action), $"api/Season/{action.SeasonId}/Week/{action.SeasonWeekId}");
    }

    private sealed class LoadSeasonWeekPlan : ApiLoadPlan<SeasonWeek>
    {
        public LoadSeasonWeekPlan(SeasonWeekActions.LoadSeasonWeek action) : base(action) { }

        public override JsonTypeInfo<SeasonWeek> ResponseJsonContext =>
            SeasonWeekJsonContext.Default.SeasonWeek;

        public override FetchSuccessAction GetSuccessAction(SeasonWeek response)
        {
            var action = (SeasonWeekActions.LoadSeasonWeek)FetchStartedAction;

            return new SeasonWeekActions.LoadSeasonWeekSuccess
            {
                SeasonId = action.SeasonId,
                SeasonWeekId = action.SeasonWeekId,
                SeasonWeek = response,
            };
        }

        public override FetchFailureAction GetFailureAction(ApiError apiError)
        {
            var action = (SeasonWeekActions.LoadSeasonWeek)FetchStartedAction;

            return new SeasonWeekActions.LoadSeasonWeekFailure
            {
                SeasonWeekId = action.SeasonWeekId,
                SeasonId = action.SeasonId
            };
        }
    }
}

public static partial class SeasonWeekActions
{
    public sealed record LoadSeasonWeek : FetchStartedAction
    {
        public int SeasonId { get; init; }
        public int SeasonWeekId { get; init; }
    }

    public sealed record LoadSeasonWeekSuccess : FetchSuccessAction
    {
        public int SeasonId { get; init; }
        public int SeasonWeekId { get; init; }
        public SeasonWeek SeasonWeek { get; init; }
    }

    public sealed record LoadSeasonWeekFailure : FetchFailureAction
    {
        public int SeasonId { get; init; }
        public int SeasonWeekId { get; init; }
    }
}

public sealed class LoadSeasonWeekSuccessReducer : Reducer<SeasonWeekState, SeasonWeekActions.LoadSeasonWeekSuccess>
{
    public override SeasonWeekState Reduce(SeasonWeekState state, SeasonWeekActions.LoadSeasonWeekSuccess action) =>
        state with
        {
            Weeks = state.Weeks.SetItem(action.SeasonWeek.SeasonWeekId, action.SeasonWeek)
        };
}

#nullable enable

using Fluxor;
using System;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Common.Extensions;
using WebApp.Client.Store.Shared;
using WebApp.Common.Enums;
using WebApp.Common.Models;

namespace WebApp.Client.Store.SeasonWeekStore;

public sealed class LoadSeasonWeekListEffect : Effect<SeasonWeekActions.LoadSeasonWeekList>
{
    private readonly IApiClient _client;

    public LoadSeasonWeekListEffect(IApiClient apiClient)
    {
        _client = apiClient;
    }

    public override async Task HandleAsync(SeasonWeekActions.LoadSeasonWeekList action, IDispatcher dispatcher)
    {
        await _client.GetAsync(new LoadSeasonWeekListPlan(action), $"api/Season/{action.SeasonId}/Week");
    }

    private sealed class LoadSeasonWeekListPlan : ApiLoadPlan<SeasonWeekSearchResponse>
    {
        public LoadSeasonWeekListPlan(SeasonWeekActions.LoadSeasonWeekList action)
            : base(action) { }

        public override JsonTypeInfo<SeasonWeekSearchResponse> ResponseBodyJsonContext =>
            GetSeasonWeekListResponseJsonContext.Default.SeasonWeekSearchResponse;

        public override FetchSuccessAction GetSuccessAction(SeasonWeekSearchResponse response)
        {
            if (FetchStartedAction is not SeasonWeekActions.LoadSeasonWeekList action)
            {
                throw new InvalidCastException("FetchStartedAction is not of type LoadSeasonWeekList.");
            }

            return new SeasonWeekActions.LoadSeasonWeekListSuccess
            {
                SeasonId = action.SeasonId,
                Weeks = response.SeasonWeeks,
            };
        }

        public override FetchFailureAction GetFailureAction(ApiError apiError)
        {
            if (FetchStartedAction is not SeasonWeekActions.LoadSeasonWeekList action)
            {
                throw new InvalidCastException("FetchStartedAction is not of type LoadSeasonWeekList.");
            }

            return new SeasonWeekActions.LoadSeasonWeekListFailure
            {
                SeasonId = action.SeasonId,
            };
        }
    }
}

public static partial class SeasonWeekActions
{
    public sealed record LoadSeasonWeekList : FetchStartedAction
    {
        public int SeasonId { get; init; }
        public WeekType? WeekType { get; init; }
    }

    public sealed record LoadSeasonWeekListSuccess : FetchSuccessAction
    {
        public int SeasonId { get; init; }
        public required SeasonWeek[] Weeks { get; init; }
    }

    public sealed record LoadSeasonWeekListFailure : FetchFailureAction
    {
        public int SeasonId { get; init; }
    }
}

public sealed class LoadSeasonWeekListSuccessReducer : Reducer<SeasonWeekState, SeasonWeekActions.LoadSeasonWeekListSuccess>
{
    public override SeasonWeekState Reduce(SeasonWeekState state, SeasonWeekActions.LoadSeasonWeekListSuccess action) =>
        state with
        {
            Weeks = state.Weeks.SetItems(action.Weeks.ToKeyValuePairs(key => key.SeasonWeekId))
        };
}

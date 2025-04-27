﻿using Fluxor;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.SeasonStore;

public sealed class LoadSeasonListEffect : Effect<SeasonActions.LoadSeasonList>
{
    private readonly IApiClient _apiClient;

    public LoadSeasonListEffect(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task HandleAsync(SeasonActions.LoadSeasonList action, IDispatcher dispatcher)
    {
        await _apiClient.GetAsync(new LoadSeasonListPlan(action), "api/Season");
    }

    private sealed class LoadSeasonListPlan : ApiLoadPlan<GetSeasonListResponse>
    {
        public LoadSeasonListPlan(SeasonActions.LoadSeasonList action)
            : base(action) { }

        public override JsonTypeInfo<GetSeasonListResponse> ResponseJsonContext
            => GetSeasonListResponseJsonContext.Default.GetSeasonListResponse;

        public override FetchSuccessAction GetSuccessAction(GetSeasonListResponse response) =>
            new SeasonActions.LoadSeasonListSuccess
            {
                Seasons = response.Seasons
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new SeasonActions.LoadSeasonListFailure();
    }
}

public static partial class SeasonActions
{
    public sealed record LoadSeasonList : FetchStartedAction { }

    public sealed record LoadSeasonListSuccess : FetchSuccessAction
    {
        public Season[] Seasons { get; init; }
    }

    public sealed record LoadSeasonListFailure : FetchFailureAction { }
}

public sealed class SeasonListSuccessReducer : Reducer<SeasonState, SeasonActions.LoadSeasonListSuccess>
{
    public override SeasonState Reduce(SeasonState state, SeasonActions.LoadSeasonListSuccess action) =>
        state with
        {
            Seasons = state.Seasons.SetItems(action.Seasons.ToDictionary(key => key.SeasonId)),
        };
}

#nullable enable

using Fluxor;
using System;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Common.Extensions;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.SeasonWeekStore;

public sealed class UpdateSeasonWeekEffect : Effect<SeasonWeekActions.UpdateSeasonWeek>
{
    private readonly IApiClient _apiClient;

    public UpdateSeasonWeekEffect(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task HandleAsync(SeasonWeekActions.UpdateSeasonWeek action, IDispatcher dispatcher)
    {
        var result = await _apiClient.PutAsync(new UpdateSeasonWeekPlan(action), $"api/Season/{action.SeasonId}/Week/{action.SeasonWeekId}", action.Request);

        if (result.IsSuccess)
        {
            dispatcher.DispatchFetch(new SeasonWeekActions.LoadSeasonWeek
            {
                SeasonId = action.SeasonId,
                SeasonWeekId = action.SeasonWeekId,
                FetchOptions = FetchOptions.SilentRefresh
            });
        }
    }

    private sealed class UpdateSeasonWeekPlan : ApiLoadPlanWithBodyNoContent<UpdateSeasonWeekRequest>
    {
        public UpdateSeasonWeekPlan(SeasonWeekActions.UpdateSeasonWeek action) : base(action) { }

        public override JsonTypeInfo<UpdateSeasonWeekRequest> RequestBodyJsonContext =>
            UpdateSeasonWeekRequestJsonContext.Default.UpdateSeasonWeekRequest;

        public override FetchSuccessAction GetSuccessAction(NoContentResponse response)
        {
            if (FetchStartedAction is not SeasonWeekActions.UpdateSeasonWeek action)
            {
                throw new InvalidCastException("FetchStartedAction is not of type UpdateSeasonWeek.");
            }

            return new SeasonWeekActions.UpdateSeasonWeekSuccess
            {
                SeasonId = action.SeasonId,
                SeasonWeekId = action.SeasonWeekId,
            };
        }

        public override FetchFailureAction GetFailureAction(ApiError apiError)
        {
            if (FetchStartedAction is not SeasonWeekActions.UpdateSeasonWeek action)
            {
                throw new InvalidCastException("FetchStartedAction is not of type UpdateSeasonWeek.");
            }

            return new SeasonWeekActions.UpdateSeasonWeekFailure
            {
                SeasonId = action.SeasonId,
                SeasonWeekId = action.SeasonWeekId,
            };
        }
    }
}

public static partial class SeasonWeekActions
{
    public sealed record UpdateSeasonWeek : FetchStartedAction
    {
        public int SeasonId { get; init; }
        public int SeasonWeekId { get; init; }
        public required UpdateSeasonWeekRequest Request { get; init; }
    }

    public sealed record UpdateSeasonWeekSuccess : FetchSuccessAction
    {
        public int SeasonId { get; init; }
        public int SeasonWeekId { get; init; }
    }

    public sealed record UpdateSeasonWeekFailure : FetchFailureAction
    {
        public int SeasonId { get; init; }
        public int SeasonWeekId { get; init; }
    }
}
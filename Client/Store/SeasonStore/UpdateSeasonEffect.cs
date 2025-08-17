using Fluxor;
using System;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Common.Extensions;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.SeasonStore;

public sealed class UpdateSeasonEffect : Effect<SeasonActions.UpdateSeason>
{
    private readonly IApiClient _apiClient;
    public UpdateSeasonEffect(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task HandleAsync(SeasonActions.UpdateSeason action, IDispatcher dispatcher)
    {
        var result = await _apiClient.PutAsync(new UpdateSeasonPlan(action), $"api/Season/{action.SeasonId}", action.Request);

        if (result.IsSuccess)
        {
            dispatcher.DispatchFetch(new SeasonActions.LoadSeason
            {
                SeasonId = action.SeasonId,
                HideLoading = true,
                DispatchErrorToWindow = false,
                ForceDispatch = true
            });
        }
    }

    private sealed class UpdateSeasonPlan : ApiLoadPlanWithBodyNoContent<UpdateSeasonRequest>
    {
        public UpdateSeasonPlan(SeasonActions.UpdateSeason action) : base(action) { }

        public override JsonTypeInfo<UpdateSeasonRequest> BodyJsonContext =>
            UpdateSeasonRequestJsonContext.Default.UpdateSeasonRequest;

        public override FetchSuccessAction GetSuccessAction(NoContentResponse response) =>
            new SeasonActions.UpdateSeasonSuccess();

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new SeasonActions.UpdateSeasonFailure();
    }
}

public static partial class SeasonActions
{
    public sealed record UpdateSeason : FetchStartedAction
    {
        public int SeasonId { get; init; }
        public UpdateSeasonRequest Request { get; init; }
    }

    public sealed record UpdateSeasonSuccess : FetchSuccessAction { }

    public sealed record UpdateSeasonFailure : FetchFailureAction { }
}
#nullable enable

using Fluxor;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Common.Extensions;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.SeasonStore;

public sealed class CreateSeasonEffect : Effect<SeasonActions.CreateSeason>
{
    private readonly IApiClient _apiClient;

    public CreateSeasonEffect(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task HandleAsync(SeasonActions.CreateSeason action, IDispatcher dispatcher)
    {
        var result = await _apiClient.PostAsync(new CreateSeasonPlan(action), "api/Season", action.Request);

        if (result.IsSuccess && action.Request.SeasonId.HasValue)
        {
            dispatcher.DispatchFetch(new SeasonActions.LoadSeason
            {
                SeasonId = action.Request.SeasonId.Value,
                FetchOptions = FetchOptions.SilentRefresh,
            });
        }
    }

    private sealed class CreateSeasonPlan : ApiLoadPlanWithBodyNoContent<CreateSeasonRequest>
    {
        public CreateSeasonPlan(SeasonActions.CreateSeason action) 
            : base(action) { }

        public override JsonTypeInfo<CreateSeasonRequest> BodyJsonContext =>
            CreateSeasonRequestJsonContext.Default.CreateSeasonRequest;

        public override FetchSuccessAction GetSuccessAction(NoContentResponse response) =>
            new SeasonActions.CreateSeasonSuccess();

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new SeasonActions.CreateSeasonFailure();
    }
}

public static partial class SeasonActions
{
    public sealed record CreateSeason : FetchStartedAction
    {
        public required CreateSeasonRequest Request { get; init; }
    }

    public sealed record CreateSeasonSuccess : FetchSuccessAction { }

    public sealed record CreateSeasonFailure : FetchFailureAction { }
}
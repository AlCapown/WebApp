#nullable enable

using Fluxor;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.SeasonStore;

public sealed class LoadSeasonEffect : Effect<SeasonActions.LoadSeason>
{
    private readonly IApiClient _apiClient;

    public LoadSeasonEffect(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task HandleAsync(SeasonActions.LoadSeason action, IDispatcher dispatcher)
    {
        await _apiClient.GetAsync(new LoadSeasonPlan(action), $"api/Season/{action.SeasonId}");
    }

    private sealed class LoadSeasonPlan : ApiLoadPlan<Season>
    {
        public LoadSeasonPlan(SeasonActions.LoadSeason action)
            : base(action) { }

        public override JsonTypeInfo<Season> ResponseBodyJsonContext =>
            SeasonJsonContext.Default.Season;

        public override FetchSuccessAction GetSuccessAction(Season response) =>
            new SeasonActions.LoadSeasonSuccess
            {
                Season = response
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new SeasonActions.LoadSeasonFailure();
    }
}

public static partial class SeasonActions
{
    public sealed record LoadSeason : FetchStartedAction
    {
        public int SeasonId { get; init; }
    }

    public sealed record LoadSeasonSuccess : FetchSuccessAction
    {
        public required Season Season { get; init; }
    }

    public sealed record LoadSeasonFailure : FetchFailureAction { }
}

public sealed class LoadSeasonSuccessReducer : Reducer<SeasonState, SeasonActions.LoadSeasonSuccess>
{
    public override SeasonState Reduce(SeasonState state, SeasonActions.LoadSeasonSuccess action) =>
        state with
        {
            Seasons = state.Seasons.SetItem(action.Season.SeasonId, action.Season),
        };
}

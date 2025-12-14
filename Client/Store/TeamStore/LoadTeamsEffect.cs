#nullable enable

using Fluxor;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.TeamStore;

public sealed class LoadTeamsEffect : Effect<TeamActions.LoadTeams>
{
    private readonly IApiClient _client;

    public LoadTeamsEffect(IApiClient apiClient)
    {
        _client = apiClient;
    }

    public override async Task HandleAsync(TeamActions.LoadTeams action, IDispatcher dispatcher)
    {
        await _client.GetAsync(new LoadTeamsPlan(action), "api/Team");
    }

    private sealed class LoadTeamsPlan : ApiLoadPlan<TeamSearchResponse>
    {
        public LoadTeamsPlan(TeamActions.LoadTeams action)
            : base(action) { }

        public override JsonTypeInfo<TeamSearchResponse> ResponseBodyJsonContext =>
            GetTeamsResponseJsonContext.Default.TeamSearchResponse;

        public override FetchSuccessAction GetSuccessAction(TeamSearchResponse response) =>
            new TeamActions.LoadTeamsSuccess
            {
                Teams = response.Teams,
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new TeamActions.LoadTeamsFailure();
    }
}

public static partial class TeamActions
{
    public sealed record LoadTeams : FetchStartedAction { }

    public sealed record LoadTeamsSuccess : FetchSuccessAction
    {
        public required Team[] Teams { get; init; }
    }

    public sealed record LoadTeamsFailure : FetchFailureAction { }
}

public sealed class LoadTeamsSuccessReducer : Reducer<TeamState, TeamActions.LoadTeamsSuccess>
{
    public override TeamState Reduce(TeamState state, TeamActions.LoadTeamsSuccess action) =>
        state with
        {
            Teams = state.Teams.SetItems(action.Teams.ToDictionary(key => key.TeamId))
        };
}

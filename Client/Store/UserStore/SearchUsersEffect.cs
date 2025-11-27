#nullable enable

using Fluxor;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.UserStore;

public sealed class SearchUsersEffect : Effect<UserActions.SearchUsers>
{
    private readonly IApiClient _client;

    public SearchUsersEffect(IApiClient client)
    {
        _client = client;
    }

    public override async Task HandleAsync(UserActions.SearchUsers action, IDispatcher dispatcher)
    {
        await _client.GetAsync(new SearchUsersPlan(action), $"api/User/Search", new Dictionary<string, string?>
        {
            { nameof(action.UserId), action.UserId }
        });
    }

    private sealed class SearchUsersPlan: ApiLoadPlan<SearchUsersResponse>
    {
        public SearchUsersPlan(UserActions.SearchUsers action)
            : base(action) { }

        public override JsonTypeInfo<SearchUsersResponse> ResponseJsonContext =>
            SearchUsersJsonContext.Default.SearchUsersResponse;

        public override FetchSuccessAction GetSuccessAction(SearchUsersResponse response) =>
            new UserActions.SearchUsersSuccess
            {
                Users = response.Users
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new UserActions.GetUserByIdFailure();
    }
}


public static partial class UserActions
{
    public sealed record SearchUsers : FetchStartedAction
    {
        public string? UserId { get; init; }
    }

    public sealed record SearchUsersSuccess : FetchSuccessAction
    {
        public required User[] Users { get; init; }
    }

    public sealed record SearchUsersFailure : FetchFailureAction { }
}

public sealed class SearchUsersSuccessReducer : Reducer<UserState, UserActions.SearchUsersSuccess>
{
    public override UserState Reduce(UserState state, UserActions.SearchUsersSuccess action) =>
        state with
        {
            Users = state.Users.SetItems(action.Users.ToDictionary(x => x.UserId))
        };
}
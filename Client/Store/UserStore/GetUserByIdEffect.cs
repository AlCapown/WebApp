using Fluxor;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Store.Shared;
using WebApp.Common.Models;

namespace WebApp.Client.Store.UserStore;

public sealed class GetUserByIdEffect : Effect<UserActions.GetUserById>
{
    private readonly IApiClient _client;

    public GetUserByIdEffect(IApiClient client)
    {
        _client = client;
    }

    public override async Task HandleAsync(UserActions.GetUserById action, IDispatcher dispatcher)
    {
        await _client.GetAsync(new GetUserByIdPlan(action), $"api/User/{action.UserId}");
    }

    private sealed class GetUserByIdPlan : ApiLoadPlan<User>
    {
        public GetUserByIdPlan(UserActions.GetUserById action)
            : base(action) { }

        public override JsonTypeInfo<User> ResponseJsonContext =>
            UserJsonContext.Default.User;

        public override FetchSuccessAction GetSuccessAction(User user) =>
            new UserActions.GetUserByIdSuccess
            {
                User = user
            };

        public override FetchFailureAction GetFailureAction(ApiError apiError) =>
            new UserActions.GetUserByIdFailure();
    }
}

public static partial class UserActions
{
    public sealed record GetUserById : FetchStartedAction
    {
        public string UserId { get; init; }
    }

    public sealed record GetUserByIdSuccess : FetchSuccessAction
    {
        public User User { get; init; }
    }

    public sealed record GetUserByIdFailure : FetchFailureAction { }
}

public sealed class GetUserByIdSuccessReducer : Reducer<UserState, UserActions.GetUserByIdSuccess>
{
    public override UserState Reduce(UserState state, UserActions.GetUserByIdSuccess action) =>
        state with
        {
            Users = state.Users.SetItem(action.User.UserId, action.User)
        };
}
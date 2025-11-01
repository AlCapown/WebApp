#nullable enable

using Fluxor;
using System.Collections.Immutable;
using WebApp.Common.Models;

namespace WebApp.Client.Store.UserStore;

public sealed record UserState
{
    /// <summary>
    /// User dictionary where the UserId is the key
    /// </summary>
    public required ImmutableDictionary<string, User> Users { get; init; }
}

public sealed class UserFeature : Feature<UserState>
{
    public override string GetName() => "Users";

    protected override UserState GetInitialState() => new()
    {
        Users = ImmutableDictionary.Create<string, User>()
    };
}

public static partial class UserActions { }


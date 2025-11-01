#nullable enable

using Fluxor;
using System.Collections.Immutable;
using WebApp.Common.Models;

namespace WebApp.Client.Store.GameStore;

public sealed record GameState
{
    /// <summary>
    /// Games dictionary where the GameId is the key
    /// </summary>
    public required ImmutableDictionary<int, Game> Games { get; init; }
}

public sealed class GameFeature : Feature<GameState>
{
    public override string GetName() => "Games";

    protected override GameState GetInitialState() => new()
    {
        Games = ImmutableDictionary.Create<int, Game>()
    };
}

public static partial class GameActions { }

using Fluxor;
using System.Collections.Immutable;
using WebApp.Common.Models;

namespace WebApp.Client.Store.TeamStore;

public sealed record TeamState
{
    /// <summary>
    /// Team dictionary where the team identifier is the key.
    /// </summary>
    public ImmutableDictionary<int, Team> Teams { get; init; }
}

public sealed class TeamFeature : Feature<TeamState>
{
    public override string GetName() => "Teams";

    protected override TeamState GetInitialState()
    {
        return new TeamState
        {
            Teams = ImmutableDictionary.Create<int, Team>()
        };
    }
}

public static partial class TeamActions { }

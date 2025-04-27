using Fluxor;
using System.Collections.Immutable;
using WebApp.Common.Models;

namespace WebApp.Client.Store.SeasonWeekStore;

public sealed record SeasonWeekState
{
    public ImmutableDictionary<int, SeasonWeek> Weeks { get; init; }
}

public sealed class SeasonWeekFeature : Feature<SeasonWeekState>
{
    public override string GetName() => "SeasonsWeek";

    protected override SeasonWeekState GetInitialState() => new()
    {
        Weeks = ImmutableDictionary.Create<int, SeasonWeek>()
    };
}

public static partial class SeasonWeekActions { }

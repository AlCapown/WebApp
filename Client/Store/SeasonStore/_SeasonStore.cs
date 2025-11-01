#nullable enable

using Fluxor;
using System.Collections.Immutable;
using WebApp.Common.Models;

namespace WebApp.Client.Store.SeasonStore;

public sealed record SeasonState
{
    /// <summary>
    /// Season dictionary where the seasonId is the key
    /// </summary>
    public required ImmutableDictionary<int, Season> Seasons { get; init; }
}

public sealed class SeasonFeature : Feature<SeasonState>
{
    public override string GetName() => "Seasons";

    protected override SeasonState GetInitialState() => new()
    {
        Seasons = ImmutableDictionary.Create<int, Season>()
    };
}

public static partial class SeasonActions { }
    

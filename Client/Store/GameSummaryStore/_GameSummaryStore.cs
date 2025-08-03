#nullable enable

using Fluxor;
using System.Collections.Immutable;
using WebApp.Common.Models;

namespace WebApp.Client.Store.GameSummaryStore;

public sealed record GameSummaryState
{
    /// <summary>
    /// Game summaries dictionary where the GameId is the key.
    /// </summary>
    public required ImmutableDictionary<int, GameSummary> GameSummaries { get; init; }
}

public sealed class GameSummaryFeature : Feature<GameSummaryState>
{
    public override string GetName() => "GameSummaries";

    protected override GameSummaryState GetInitialState()
    {
        return new GameSummaryState
        {
            GameSummaries = ImmutableDictionary.Create<int, GameSummary>()
        };
    }
}

public static partial class GameSummaryActions { }



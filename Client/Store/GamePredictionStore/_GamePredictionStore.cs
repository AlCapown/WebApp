using Fluxor;
using System.Collections.Immutable;
using WebApp.Common.Models;

namespace WebApp.Client.Store.GamePredictionStore;

public sealed record GamePredictionState
{
    /// <summary>
    /// GamePredictions dictionary where the GamePredictionId is the key
    /// </summary>
    public ImmutableDictionary<int, GamePrediction> GamePredictions { get; init; }
}

public sealed class GamePredictionFeature : Feature<GamePredictionState>
{
    public override string GetName() => "GamePredictions";

    protected override GamePredictionState GetInitialState() => new()
    {
        GamePredictions = ImmutableDictionary.Create<int, GamePrediction>()
    };
}

public static partial class GamePredictionActions { }


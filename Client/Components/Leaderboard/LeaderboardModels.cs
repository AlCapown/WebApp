#nullable enable

using System.Collections.Generic;

namespace WebApp.Client.Components.Leaderboard;

public sealed record LeaderboardItem
{
    /// <summary>
    /// Week number for which the predictions were made. This indicates the specific week of the season that the leaderboard entry corresponds to.
    /// </summary>
    public int Week { get; init; }
    
    /// <summary>
    /// Gets the collection of user predictions, keyed by user identifier.
    /// </summary>
    /// <remarks>
    /// Each entry in the dictionary represents a single user's prediction
    /// </remarks>
    public required IReadOnlyDictionary<string, UserPrediction> UserPredictions { get; init; }
}

public sealed record UserPrediction
{
    public required string UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public int ScoreDifferential { get; init; }
    public bool PredictedWinningTeam { get; init; }
}

public sealed record User
{
    public required string UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
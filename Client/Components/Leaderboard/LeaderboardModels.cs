#nullable enable

using System.Collections.Generic;

namespace WebApp.Client.Components.Leaderboard;

public sealed record LeaderboardItem
{
    public int Week { get; init; }
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
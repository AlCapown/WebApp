using System.Collections.Generic;

namespace WebApp.Client.Components.Leaderboard;

public record LeaderboardItem
{
    public int Week { get; init; }
    public IReadOnlyDictionary<string, UserPrediction> UserPredictions { get; init; }
}

public record UserPrediction
{
    public string UserId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int ScoreDifferential { get; init; }
    public bool PredictedWinningTeam { get; init; }
}

public record User
{
    public string UserId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
}
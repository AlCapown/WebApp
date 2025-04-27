using System.Collections.Generic;

namespace WebApp.Client.Components.Leaderboard;

public class LeaderboardItem
{
    public int Week { get; set; }
    public Dictionary<string, UserPrediction> UserPredictions { get; set; }
}

public class UserPrediction
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int ScoreDifferential { get; set; }
    public bool PredictedWinningTeam { get; set; }
}

public class User
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
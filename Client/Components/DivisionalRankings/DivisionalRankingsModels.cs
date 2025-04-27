namespace WebApp.Client.Components.DivisionalRankings;

public record TeamRecord
{
    public int TeamId { get; init; }
    public string TeamName { get; init; }
    public string Division { get; init; }
    public int Wins { get; init; }
    public int Ties { get; init; }
    public int Losses { get; init; }
}
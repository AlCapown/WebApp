
namespace WebApp.Client.Components.DivisionalRankings;

public sealed record TeamRecord
{
    public int TeamId { get; init; }
    public required string TeamName { get; init; }
    public required string Division { get; init; }
    public int Wins { get; init; }
    public int Ties { get; init; }
    public int Losses { get; init; }
}
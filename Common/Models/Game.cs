using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Common.Enums;

namespace WebApp.Common.Models;

public sealed record Game
{
    public int GameId { get; init; }
    public int SeasonWeekId { get; init; }
    public int SeasonId { get; init; }
    public int Week { get; init; }
    public WeekType SeasonWeekTypeName { get; init; }
    public DateTimeOffset? GameStartsOn { get; init; }
    public int HomeTeamId { get; init; }
    public string HomeTeamName { get; init; }
    public int HomeTeamScore { get; init; }
    public int AwayTeamId { get; init; }
    public string AwayTeamName { get; init; }
    public int AwayTeamScore { get; init; }
    public string ClockTime { get; init; }
    public int Quarter { get; init; }
    public bool IsComplete { get; init; }
}

public sealed record SearchGamesResponse
{
    public Game[] Games { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(SearchGamesResponse))]
public partial class SearchGamesResponseJsonContext : JsonSerializerContext { }
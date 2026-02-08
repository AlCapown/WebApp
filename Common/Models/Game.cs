#nullable enable

using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Common.Enums;

namespace WebApp.Common.Models;

/// <summary>
/// Represents a sporting game with teams, scores, and scheduling information.
/// This record is used as a data transfer object for game-related operations.
/// </summary>
[ImmutableObject(true)]
public sealed record Game
{
    /// <summary>
    /// Unique identifier for the game.
    /// </summary>
    public int GameId { get; init; }
    
    /// <summary>
    /// Identifier of the season week this game belongs to.
    /// </summary>
    public int SeasonWeekId { get; init; }
    
    /// <summary>
    /// Identifier of the season this game belongs to.
    /// </summary>
    public int SeasonId { get; init; }
    
    /// <summary>
    /// The week number within the season (e.g., Week 1, Week 2).
    /// </summary>
    public int Week { get; init; }
    
    /// <summary>
    /// The type of week (Preseason, RegularSeason, PostSeason, or OffSeason).
    /// </summary>
    public WeekType SeasonWeekTypeName { get; init; }
    
    /// <summary>
    /// The scheduled start date and time of the game.
    /// Null if the start time is not yet determined.
    /// </summary>
    public DateTimeOffset? GameStartsOn { get; init; }
    
    /// <summary>
    /// Identifier of the home team.
    /// </summary>
    public int HomeTeamId { get; init; }
    
    /// <summary>
    /// Display name of the home team.
    /// </summary>
    public required string HomeTeamName { get; init; }
    
    /// <summary>
    /// Current or final score for the home team.
    /// </summary>
    public int HomeTeamScore { get; init; }
    
    /// <summary>
    /// Identifier of the away team.
    /// </summary>
    public int AwayTeamId { get; init; }
    
    /// <summary>
    /// Display name of the away team.
    /// </summary>
    public required string AwayTeamName { get; init; }
    
    /// <summary>
    /// Current or final score for the away team.
    /// </summary>
    public int AwayTeamScore { get; init; }
    
    /// <summary>
    /// Current game clock time in format MM:SS.
    /// </summary>
    public string? ClockTime { get; init; }
    
    /// <summary>
    /// Current quarter or period of the game.
    /// 0 indicates the game hasn't started, 5+ may indicate overtime.
    /// </summary>
    public int Quarter { get; init; }
    
    /// <summary>
    /// Indicates whether the game has been completed.
    /// </summary>
    public bool IsComplete { get; init; }

    /// <summary>
    /// Indicates whether the game has an summary recap of the user predictions.
    /// </summary>
    public bool HasSummary { get; init; }
}

public sealed record GameSearchResponse
{
    public required Game[] Games { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Game))]
[JsonSerializable(typeof(GameSearchResponse))]
public sealed partial class GameJsonContext : JsonSerializerContext { }


[ImmutableObject(true)]
public sealed record GameSummary
{
    /// <summary>
    /// The unique identifier for the game summary record.
    /// </summary>
    public int GameSummaryId { get; init; }

    /// <summary>
    /// The identifier for the game.
    /// </summary>
    public int GameId { get; init; }

    /// <summary>
    /// The AI summary for the game.
    /// </summary>
    public required string Summary { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GameSummary))]
public sealed partial class GameSummaryJsonContext : JsonSerializerContext { }
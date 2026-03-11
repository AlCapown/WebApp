using System.Text.Json.Serialization;

namespace WebApp.ExternalIntegrations.ESPN.Service.Models;

/**
 * Returned result from the ESPN Scoreboard endpoint. 
 * Note this model is not complete but has all the fields this app needs.
 */
public class ESPNScoreboardModel
{
    public required League[] Leagues { get; set; }
    public required Season Season { get; set; }
    public required Week Week { get; set; }
    public required Event[] Events { get; set; }
}

#region Leagues

public class League
{
    /// <summary>
    /// League name
    /// </summary>
    /// <remarks>
    /// National Football League
    /// </remarks>
    public required string Name { get; set; }

    /// <summary>
    /// League abbreviation
    /// </summary>
    /// <remarks>
    /// NFL
    /// </remarks>
    public required string Abbreviation { get; set; }
    
    /// <summary>
    /// Season Start Date
    /// </summary>
    public DateTimeOffset CalendarStartDate { get; set; }
    
    /// <summary>
    /// Season End Date
    /// </summary>
    public DateTimeOffset CalendarEndDate { get; set; }

    public LeagueCalendar[] Calendar { get; set; } = [];
}

public class LeagueCalendar
{
    /// <summary>
    /// Season Type Label. Examples: Preseason, Regular Season, Postseason
    /// </summary>
    public required string Label { get; set; }
    
    /// <summary>
    /// Season Type Identifier
    /// </summary>
    public required string Value { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public LeagueCalendarEntries[] Entries { get; set; } = [];
}

public class LeagueCalendarEntries
{
    public required string Label { get; set; }
    public required string AlternativeLabel { get; set; }
    public required string Value { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
}

#endregion

#region Season

public class Season
{
    /// <summary>
    /// Season Type Identifier
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// YearId
    /// </summary>
    public int Year { get; set; }

}

#endregion

#region Week

public class Week
{
    public int Number { get; set; }
}

#endregion

#region Events

public class Event
{
    public DateTimeOffset Date { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public required EventSeason Season { get; set; }
    public required EventWeek Week { get; set; }
    public EventCompetition[] Competitions { get; set; } = [];
}

public class EventSeason
{
    /// <summary>
    /// Season Type Identifier
    /// </summary>
    public int Type { get; set; }


    /// <summary>
    /// Season Type Identifier in regular name string
    /// </summary>
    [JsonIgnore]
    public string? TypeName =>
        Type switch
        {
            1 => "Preseason",
            2 => "RegularSeason",
            3 => "PostSeason",
            4 => "OffSeason",
            _ => null
        };

    /// <summary>
    /// YearId
    /// </summary>
    public int Year { get; set; }
}

public class EventWeek
{
    /// <summary>
    /// Week number
    /// </summary>
    public int Number { get; set; }
}

public class EventCompetition
{
    public EventCompetitor[] Competitors { get; set; } = [];
    public required EventStatus Status { get; set; }
}

public class EventCompetitor
{
    public required string HomeAway { get; set; }
    public required EventTeam Team { get; set; }
    public required string Score { get; set; }
}

public class EventTeam
{
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }
    public required string DisplayName { get; set; }
    public required string ShortDisplayName { get; set; }
}

public class EventStatus
{
    public required string DisplayClock { get; set; }
    public int Period { get; set; }
    public required EventStatusType Type { get; set; }
}

public class EventStatusType
{
    public required string Name { get; set; }
    public bool Completed { get; set; }
    public required string Description { get; set; }
    public required string Detail { get; set; }
    public required string ShortDetail { get; set; }
}

#endregion

#region JsonContext

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(ESPNScoreboardModel))]
public partial class ESPNScoreboardModelJsonContext : JsonSerializerContext { }


#endregion
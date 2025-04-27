using System.Text.Json.Serialization;

namespace WebApp.ExternalIntegrations.ESPN.Service.Models;

/**
 * Returned result from the ESPN Scoreboard endpoint. 
 * Note this model is not complete but has all the fields this app needs.
 */
public class ESPNScoreboardModel
{
    public League[] Leagues { get; set; }
    public Season Season { get; set; }
    public Week Week { get; set; }
    public Event[] Events { get; set; }
}

#region Leagues

public class League
{
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    /// <summary>
    /// Season Start Date
    /// </summary>
    public DateTime CalendarStartDate { get; set; }
    /// <summary>
    /// Season End Date
    /// </summary>
    public DateTime CalendarEndDate { get; set; }
    public LeagueCalendar[] Calendar { get; set; }
}

public class LeagueCalendar
{
    /// <summary>
    /// Season Type Label. Examples: Preseason, Regular Season, Postseason
    /// </summary>
    public string Label { get; set; }
    /// <summary>
    /// Season Type Identifier
    /// </summary>
    public string Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeagueCalendarEntries[] Entries { get; set; }
}

public class LeagueCalendarEntries
{
    public string Label { get; set; }
    public string AlternativeLabel { get; set; }
    public string Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
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
    public DateTime Date { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public EventSeason Season { get; set; }
    public EventWeek Week { get; set; }
    public EventCompetition[] Competitions { get; set; }
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
    public string TypeName =>
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
    public EventCompetitor[] Competitors { get; set; }
    public EventStatus Status { get; set; }
}

public class EventCompetitor
{
    public string HomeAway { get; set; }
    public EventTeam Team { get; set; }
    public string Score { get; set; }
}

public class EventTeam
{
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public string DisplayName { get; set; }
    public string ShortDisplayName { get; set; }
}

public class EventStatus
{
    public string DisplayClock { get; set; }
    public int Period { get; set; }
    public EventStatusType Type { get; set; }
}

public class EventStatusType
{
    public string Name { get; set; }
    public bool Completed { get; set; }
    public string Description { get; set; }
    public string Detail { get; set; }
    public string ShortDetail { get; set; }
}

#endregion

#region JsonContext

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(ESPNScoreboardModel))]
public partial class ESPNScoreboardModelJsonContext : JsonSerializerContext { }


#endregion
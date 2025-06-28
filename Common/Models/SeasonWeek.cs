using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Common.Enums;

namespace WebApp.Common.Models;

[ImmutableObject(true)]
public sealed record SeasonWeek
{
    public int SeasonWeekId { get; init; }
    public int SeasonId { get; init; }
    public int Week { get; init; }
    public DateOnly WeekStart { get; init; }
    public DateOnly WeekEnd { get; init; }
    public WeekType WeekType { get; init; }
    public string Description { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(SeasonWeek))]
public partial class SeasonWeekJsonContext : JsonSerializerContext { }

public record SeasonWeekSearchResponse
{
    public SeasonWeek[] SeasonWeeks { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(SeasonWeekSearchResponse))]
public partial class GetSeasonWeekListResponseJsonContext : JsonSerializerContext { }

[ImmutableObject(true)]
public sealed record UpdateSeasonWeekRequest
{
    public DateOnly? WeekStart { get; init; }
    public DateOnly? WeekEnd { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(UpdateSeasonWeekRequest))]
public partial class UpdateSeasonWeekRequestJsonContext : JsonSerializerContext { }

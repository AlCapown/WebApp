using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Common.Enums;

namespace WebApp.Common.Models;

public record SeasonWeek
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

public record GetSeasonWeekListResponse
{
    public SeasonWeek[] SeasonWeeks { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GetSeasonWeekListResponse))]
public partial class GetSeasonWeekListResponseJsonContext : JsonSerializerContext { }

public class UpdateSeasonWeekRequest
{
    [Required]
    public DateOnly? WeekStart { get; set; }
    [Required]
    public DateOnly? WeekEnd { get; set; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(UpdateSeasonWeekRequest))]
public partial class UpdateSeasonWeekRequestJsonContext : JsonSerializerContext { }

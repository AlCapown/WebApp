using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Common.Models;

[ImmutableObject(true)]
public sealed record Season
{
    public int SeasonId { get; init; }
    public string Description { get; init; }
    public bool IsCurrent { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Season))]
public sealed partial class SeasonJsonContext : JsonSerializerContext { }

public sealed record GetSeasonListResponse
{
    public Season[] Seasons { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GetSeasonListResponse))]
public sealed partial class GetSeasonListResponseJsonContext : JsonSerializerContext { }

public sealed class CreateSeasonRequest
{
    [Required]
    public int? SeasonId { get; set; }

    [Required]
    [StringLength(255)]
    public string Description { get; set; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(CreateSeasonRequest))]
public sealed partial class CreateSeasonRequestJsonContext : JsonSerializerContext { }


public sealed class UpdateSeasonRequest
{
    [Required]
    [StringLength(255)]
    public string Description { get; set; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(UpdateSeasonRequest))]
public sealed partial class UpdateSeasonRequestJsonContext : JsonSerializerContext { }
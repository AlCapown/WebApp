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
public partial class SeasonJsonContext : JsonSerializerContext { }

public record GetSeasonListResponse
{
    public Season[] Seasons { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GetSeasonListResponse))]
public partial class GetSeasonListResponseJsonContext : JsonSerializerContext { }

public class CreateSeasonRequest
{
    [Required]
    public int? SeasonId { get; set; }

    [Required]
    [StringLength(255)]
    public string Description { get; set; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(CreateSeasonRequest))]
public partial class CreateSeasonRequestJsonContext : JsonSerializerContext { }

public record CreateSeasonResponse
{
    public int SeasonId { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(CreateSeasonResponse))]
public partial class CreateSeasonResponseJsonContext : JsonSerializerContext { }

public class UpdateSeasonRequest
{
    [Required]
    [StringLength(255)]
    public string Description { get; set; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(UpdateSeasonRequest))]
public partial class UpdateSeasonRequestJsonContext : JsonSerializerContext { }
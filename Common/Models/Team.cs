using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Common.Models;

public record Team
{
    public int TeamId { get; init; }
    public string TeamFullName { get; init; }
    public string TeamName { get; init; }
    public string Abbreviation { get; init; }
    public string BrandingLogo { get; init; }
    public string Division { get; init; }
    public string Conference { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Team))]
public partial class TeamJsonContext : JsonSerializerContext { }

public record GetTeamsResponse
{
    public Team[] Teams { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GetTeamsResponse))]
public partial class GetTeamsResponseJsonContext : JsonSerializerContext { }
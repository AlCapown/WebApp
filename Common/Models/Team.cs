using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Common.Models;

[ImmutableObject(true)]
public sealed record Team
{
    public int TeamId { get; init; }
    public required string TeamFullName { get; init; }
    public required string TeamName { get; init; }
    public required string Abbreviation { get; init; }
    public string? BrandingLogo { get; init; }
    public required string Division { get; init; }
    public required string Conference { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Team))]
public sealed partial class TeamJsonContext : JsonSerializerContext { }


public sealed record TeamSearchResponse
{
    public required Team[] Teams { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(TeamSearchResponse))]
public sealed partial class GetTeamsResponseJsonContext : JsonSerializerContext { }
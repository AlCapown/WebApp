#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Common.Models;

public sealed record User
{
    public required string UserId { get; init; }
    public required string UserName { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public bool IsAdmin { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(User))]
public partial class UserJsonContext : JsonSerializerContext { }


public sealed record SearchUsersResponse
{
    public required User[] Users { get; init; }
}


[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(SearchUsersResponse))]
public partial class SearchUsersJsonContext : JsonSerializerContext { }

public sealed record CurrentUserInfoResponse
{
    [MemberNotNullWhen(true, nameof(NameClaimType), nameof(RoleClaimType))]
    public bool IsAuthenticated { get; init; }
    public string? NameClaimType { get; init; }
    public string? RoleClaimType { get; init; }
    public ClaimValue[] Claims { get; init; } = [];
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(CurrentUserInfoResponse))]
public partial class CurrentUserInfoResponseJsonContext : JsonSerializerContext { }


public sealed record ClaimValue
{
    public required string Type { get; init; }
    public required string Value { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(ClaimValue))]
public partial class ClaimValueJsonContext : JsonSerializerContext { }
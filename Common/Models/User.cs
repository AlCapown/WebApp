using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Common.Models;

public record User
{
    public string UserId { get; init; }
    public string UserName { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public bool IsAdmin { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(User))]
public partial class UserJsonContext : JsonSerializerContext { }


public record CurrentUserInfoResponse
{
    public bool IsAuthenticated { get; init; }
    public string NameClaimType { get; init; }
    public string RoleClaimType { get; init; }
    public ClaimValue[] Claims { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(CurrentUserInfoResponse))]
public partial class CurrentUserInfoResponseJsonContext : JsonSerializerContext { }


public record ClaimValue
{
    public ClaimValue() { }

    public ClaimValue(string type, string value)
    {
        Type = type; 
        Value = value;
    }
    
    public string Type { get; init; }

    public string Value { get; init; }
}

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(ClaimValue))]
public partial class ClaimValueJsonContext : JsonSerializerContext { }
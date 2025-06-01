#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace WebApp.Common.Extensions;

public static class ClaimExtensions
{
    public static string? GetUserId(this IEnumerable<Claim> claims)
    {
        return claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}

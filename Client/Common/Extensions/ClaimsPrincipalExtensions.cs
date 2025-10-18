#nullable enable

using System.Linq;
using System.Security.Claims;

namespace WebApp.Client.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Get UserId from claims.
    /// </summary>
    /// <param name="claimsPrincipal">ClaimsPrincipal</param>
    /// <returns>The UserId if the user is authenticated, null otherwise.</returns>
    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null || claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
        {
            return null;
        }

        return claimsPrincipal.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    } 
}

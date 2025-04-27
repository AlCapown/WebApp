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
    public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        if (!claimsPrincipal.Identity.IsAuthenticated) 
        {
            return null;
        }
        
        return claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
    } 
}

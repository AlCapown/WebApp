#nullable enable

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Client.Common.Constants;
using WebApp.Common.Models;

namespace WebApp.Client.Infrastructure;

public sealed class WebAppAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly TimeSpan _userCacheExpiryInterval = TimeSpan.FromSeconds(60);

    private readonly NavigationManager _navigation;
    private readonly IHttpClientFactory _httpClientFactory;

    private ClaimsPrincipal? CachedUser { get; set; }
    private DateTimeOffset UserExpiry { get; set; } = DateTimeOffset.MinValue;

    public WebAppAuthenticationStateProvider(NavigationManager navigation, IHttpClientFactory httpClientFactory)
    {
        _navigation = navigation;
        _httpClientFactory = httpClientFactory;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserCachedAsync();
        return new AuthenticationState(user);
    }
        
    public void SignIn()
    {
        _navigation.NavigateTo("Account/Login", true);
    }

    private async ValueTask<ClaimsPrincipal> GetUserCachedAsync()
    {
        // Check if we have a valid cached user
        if (CachedUser is not null && DateTimeOffset.UtcNow < UserExpiry)
        {
            return CachedUser;
        }

        // Fetch new user and cache it. Multiple threads may reach here simultaneously but this is fine.
        CachedUser = await GetUserAsync();
        UserExpiry = DateTimeOffset.UtcNow.Add(_userCacheExpiryInterval);

        return CachedUser;
    }

    private async Task<ClaimsPrincipal> GetUserAsync()
    {
        var client = _httpClientFactory.CreateClient(ServiceConstants.WEBAPP_API_CLIENT);
        
        CurrentUserInfoResponse? user = null;

        try
        {
            user = await client.GetFromJsonAsync("api/User", CurrentUserInfoResponseJsonContext.Default.CurrentUserInfoResponse);
        }
        catch (Exception) 
        { }

        if (user is null || !user.IsAuthenticated)
        {
            // Anynomous User
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var identity = new ClaimsIdentity(nameof(AuthenticationStateProvider), user.NameClaimType, user.RoleClaimType);

        if (user.Claims != null)
        {
            identity.AddClaims(user.Claims.Select(c => new Claim(c.Type, c.Value)));
        }

        return new ClaimsPrincipal(identity);
    }
}

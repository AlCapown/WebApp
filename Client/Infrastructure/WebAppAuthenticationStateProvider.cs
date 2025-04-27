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
    private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

    private readonly NavigationManager _navigation;
    private readonly IHttpClientFactory _httpClientFactory;

    private ClaimsPrincipal CachedUser { get; set; } = null;
    private DateTimeOffset UserLastFetched { get; set; } = DateTimeOffset.Now;

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
        var now = DateTimeOffset.Now;

        if (CachedUser is not null && now < UserLastFetched.Add(_userCacheRefreshInterval))
        {
            return CachedUser;
        }

        CachedUser = await GetUserAsync();
        UserLastFetched = now;

        return CachedUser;
    }

    private async Task<ClaimsPrincipal> GetUserAsync()
    {
        var client = _httpClientFactory.CreateClient(ServiceConstants.WEBAPP_API_CLIENT);
        
        CurrentUserInfoResponse user = null;

        try
        {
            user = await client.GetFromJsonAsync("api/User", CurrentUserInfoResponseJsonContext.Default.CurrentUserInfoResponse);
        }
        catch (Exception) { }

        if (user == null || !user.IsAuthenticated)
        {
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

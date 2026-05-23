using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Client.Common.Constants;
using WebApp.Common.Models;

namespace WebApp.Client.Infrastructure;

internal sealed class WebAppAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly TimeSpan _userCacheExpiryInterval = TimeSpan.FromMinutes(5);

    private readonly NavigationManager _navigation;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<WebAppAuthenticationStateProvider> _logger;

    private ClaimsPrincipal? CachedUser { get; set; }
    private DateTimeOffset UserExpiry { get; set; } = DateTimeOffset.MinValue;

    private Task<ClaimsPrincipal>? _fetchUserTask;

    public WebAppAuthenticationStateProvider(
        NavigationManager navigation, 
        IHttpClientFactory httpClientFactory, 
        TimeProvider timeProvider, 
        ILogger<WebAppAuthenticationStateProvider> logger)
    {
        _navigation = navigation;
        _httpClientFactory = httpClientFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current authentication state asynchronously.
    /// Uses a cached user representation if available and valid; otherwise fetches the user from the server.
    /// </summary>
    /// <returns></returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserCachedAsync();
        return new AuthenticationState(user);
    }

    /// <summary>
    /// Navigates to the logout endpoint, triggering a forced reload to clear the user session.
    /// </summary>
    public void Logout()
    {
        _navigation.NavigateTo("Account/Logout", true);
    }
        
    /// <summary>
    /// Navigates to the login endpoint, triggering a forced reload to initiate the authentication process.
    /// </summary>
    public void Login()
    {
        _navigation.NavigateTo("Account/Login", true);
    }

    /// <summary>
    /// Invalidates the cached user identity and notifies the application that the authentication state has changed.
    /// </summary>
    public void RefreshState()
    {
        UserExpiry = DateTimeOffset.MinValue;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Retrieves the user's claims principal from cache if available and not expired; 
    /// otherwise, coordinates fetching it from the server to prevent concurrent duplicate requests.
    /// </summary>
    /// <returns></returns>
    private async ValueTask<ClaimsPrincipal> GetUserCachedAsync()
    {
        // Check if we have a valid cached user
        if (CachedUser is not null && _timeProvider.GetUtcNow() < UserExpiry)
        {
            return CachedUser;
        }

        // If a fetch is already in flight, wait for it
        if (_fetchUserTask is not null)
        {
            return await _fetchUserTask;
        }

        // Otherwise, start a new fetch and then wait for it
        _fetchUserTask = OrchistrateFetchUserAsync();
        return await _fetchUserTask;
    }

    /// <summary>
    /// Wraps the remote user fetch process, updates the local cache, and manages the in-flight fetch task.
    /// </summary>
    /// <returns></returns>
    private async Task<ClaimsPrincipal> OrchistrateFetchUserAsync()
    {
        try
        {
            CachedUser = await FetchUserAsync();
            UserExpiry = _timeProvider.GetUtcNow().Add(_userCacheExpiryInterval);
            return CachedUser;
        }
        finally
        {
            _fetchUserTask = null;
        }
    }

    /// <summary>
    /// Actually makes the API call to the server to get the current user's claims, roles, and authentication status.
    /// </summary>
    /// <returns></returns>
    private async Task<ClaimsPrincipal> FetchUserAsync()
    {
        var client = _httpClientFactory.CreateClient(ServiceConstants.WEBAPP_API_CLIENT);
        
        CurrentUserInfoResponse? user = null;

        try
        {
            user = await client.GetFromJsonAsync("api/User", CurrentUserInfoResponseJsonContext.Default.CurrentUserInfoResponse);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Error fetching user information.");
        }

        if (user is not { IsAuthenticated: true })
        {   
            // Anonymous User
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var identity = new ClaimsIdentity(nameof(AuthenticationStateProvider), user.NameClaimType, user.RoleClaimType);

        if (user.Claims is not null)
        {
            identity.AddClaims(user.Claims.Select(c => new Claim(c.Type, c.Value)));
        }

        return new ClaimsPrincipal(identity);
    }
}

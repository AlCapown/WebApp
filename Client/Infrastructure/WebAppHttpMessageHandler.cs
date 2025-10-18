#nullable enable

using Microsoft.JSInterop;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Client.Infrastructure;

public sealed class WebAppHttpMessageHandler : DelegatingHandler
{
    private static readonly TimeSpan _tokenCacheExpiryInterval = TimeSpan.FromMinutes(30);

    private readonly IJSRuntime _jsRuntime;
    private readonly WebAppAuthenticationStateProvider _authenticationStateProvider;

    private string? CachedToken { get; set; }
    private DateTimeOffset TokenExpiry { get; set; } = DateTimeOffset.MinValue;

    public WebAppHttpMessageHandler(IJSRuntime jsRuntime, WebAppAuthenticationStateProvider authenticationStateProvider)
    {
        _jsRuntime = jsRuntime;
        _authenticationStateProvider = authenticationStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var antiforgeryToken = await GetCachedAntiforgeryTokenAsync(cancellationToken);

        request.Headers.Add("X-XSRF-TOKEN", antiforgeryToken);

        var responseMessage = await base.SendAsync(request, cancellationToken);
        
        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            // if server returned 401 Unauthorized, redirect to login page
            _authenticationStateProvider.SignIn();
        }

        return responseMessage;
    }

    private async ValueTask<string> GetCachedAntiforgeryTokenAsync(CancellationToken cancellationToken)
    {
        // Check if we have a valid cached token
        if (CachedToken is not null && DateTimeOffset.UtcNow < TokenExpiry)
        {
            return CachedToken;
        }

        // Fetch new token and cache it. Multiple threads may reach here simultaneously, but it's acceptable.
        var token = await _jsRuntime.InvokeAsync<string>("getAntiforgeryToken", cancellationToken);

        CachedToken = token;
        TokenExpiry = DateTimeOffset.UtcNow.Add(_tokenCacheExpiryInterval);
        
        return token;
    }
}

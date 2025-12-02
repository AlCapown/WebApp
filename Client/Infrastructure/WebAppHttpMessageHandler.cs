#nullable enable

using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Client.Infrastructure;

public sealed class WebAppHttpMessageHandler : DelegatingHandler
{
    private static readonly TimeSpan _tokenExpiryInterval = TimeSpan.FromMinutes(10);
    private readonly IJSRuntime _jsRuntime;

    private string? CachedToken { get; set; }
    private DateTimeOffset TokenExpiry { get; set; } = DateTimeOffset.MinValue;

    public WebAppHttpMessageHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var antiforgeryToken = await GetCachedAntiforgeryTokenAsync(cancellationToken);
        request.Headers.Add("X-XSRF-TOKEN", antiforgeryToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async ValueTask<string> GetCachedAntiforgeryTokenAsync(CancellationToken cancellationToken)
    {
        if (CachedToken is not null && DateTimeOffset.UtcNow < TokenExpiry)
        {
            return CachedToken;
        }

        // Fetch new token and cache it. Multiple threads may reach here simultaneously, but this is fine.
        var token = await _jsRuntime.InvokeAsync<string?>("getAntiforgeryToken", cancellationToken)
            ?? throw new InvalidOperationException("Antiforgery token retrieval failed.");

        CachedToken = token;
        TokenExpiry = DateTimeOffset.UtcNow.Add(_tokenExpiryInterval);

        return CachedToken;
    }
}

#nullable enable

using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Client.Infrastructure;

public sealed class WebAppHttpMessageHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;
    private string? CachedToken { get; set; }

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
        if (CachedToken is not null)
        {
            return CachedToken;
        }

        // Fetch new token and cache it. Multiple threads may reach here simultaneously but this is fine.
        var token = await _jsRuntime.InvokeAsync<string?>("getAntiforgeryToken", cancellationToken)
            ?? throw new InvalidOperationException("Antiforgery token retrieval failed.");

        CachedToken = token;
        return CachedToken;
    }
}

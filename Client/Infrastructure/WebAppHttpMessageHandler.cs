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
    private Task<string>? _fetchTokenTask;

    public WebAppHttpMessageHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var antiforgeryToken = await GetCachedAntiforgeryTokenAsync();
        request.Headers.Add("X-XSRF-TOKEN", antiforgeryToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async ValueTask<string> GetCachedAntiforgeryTokenAsync()
    {
        // Check if we have a cached token
        if (CachedToken is not null)
        {
            return CachedToken;
        }

        // If a fetch is already in flight, wait for it
        if (_fetchTokenTask is not null)
        {
            return await _fetchTokenTask;
        }

        // Otherwise, start a new fetch and then wait for it
        _fetchTokenTask = OrchestrateFetchTokenAsync();
        return await _fetchTokenTask;
    }

    private async Task<string> OrchestrateFetchTokenAsync()
    {
        try
        {
            // Pass CancellationToken.None instead of a request-specific token since this task could be
            // shared between 2 different Web Requests. We don't want first requests cancellation to interrupt
            // the token fetch for the second request.
            var token = await _jsRuntime.InvokeAsync<string?>("getAntiforgeryToken", CancellationToken.None)
                ?? throw new InvalidOperationException("Antiforgery token retrieval failed.");

            CachedToken = token;

            return CachedToken;
        }
        finally
        {
            _fetchTokenTask = null;
        }
    }
}

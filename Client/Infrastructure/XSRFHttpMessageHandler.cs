using Microsoft.JSInterop;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Client.Infrastructure;

public sealed class XSRFHttpMessageHandler : DelegatingHandler
{ 
    private readonly IJSRuntime _jsRuntime;
    private readonly WebAppAuthenticationStateProvider _authenticationStateProvider;

    public XSRFHttpMessageHandler(IJSRuntime jsRuntime, WebAppAuthenticationStateProvider authenticationStateProvider)
    {
        _jsRuntime = jsRuntime;
        _authenticationStateProvider = authenticationStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {

        var antiforgeryToken = await _jsRuntime.InvokeAsync<string>("getAntiforgeryToken", cancellationToken);
        request.Headers.Add("X-XSRF-TOKEN", antiforgeryToken);
        var responseMessage = await base.SendAsync(request, cancellationToken);
        

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            // if server returned 401 Unauthorized, redirect to login page
            _authenticationStateProvider.SignIn();
        }

        return responseMessage;
    }
}

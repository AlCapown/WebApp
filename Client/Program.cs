using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Threading.Tasks;
using WebApp.Client.Api;
using WebApp.Client.Common.Constants;
using WebApp.Client.Infrastructure;
using WebApp.Client.Store;
using WebApp.Client.Store.Shared;


#if DEBUG
using Fluxor.Blazor.Web.ReduxDevTools;
#endif

namespace WebApp.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
 
        builder.RootComponents.Add<App>("#app");

        builder.Logging.SetMinimumLevel(builder.HostEnvironment.IsDevelopment() ? LogLevel.Information : LogLevel.Warning);

        builder.Services.AddAuthorizationCore();

        builder.Services
            .AddHttpClient(ServiceConstants.WEBAPP_API_CLIENT, client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
            })
            .AddHttpMessageHandler<XSRFHttpMessageHandler>()
            .AddPolicyHandler
            (
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            );

        // Fluxor for state management
        // https://github.com/mrpmorris/Fluxor
        builder.Services.AddFluxor(options =>
        {
            options.ScanAssemblies(typeof(FluxorAotHints).Assembly);
            options.AddMiddleware<FetchMiddleware>();

#if DEBUG
            options.UseReduxDevTools();
#endif
        });

        builder.Services.AddSingleton<AuthenticationStateProvider, WebAppAuthenticationStateProvider>();
        builder.Services.AddSingleton(sp => (WebAppAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        builder.Services.AddScoped<IApiClient, ApiClient>();
        builder.Services.AddScoped<XSRFHttpMessageHandler>();

        builder.Services.AddMudServices();

        var app = builder.Build();

        await app.RunAsync();
    }
}

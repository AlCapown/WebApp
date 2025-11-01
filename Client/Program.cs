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

        // Authorization
        builder.Services.AddAuthorizationCore();

        // Authentication
        builder.Services.AddSingleton<AuthenticationStateProvider, WebAppAuthenticationStateProvider>();
        builder.Services.AddSingleton(sp => (WebAppAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        // API Http Client
        builder.Services.AddScoped<IApiClient, ApiClient>();
        builder.Services.AddScoped<WebAppHttpMessageHandler>();
        builder.Services
            .AddHttpClient(ServiceConstants.WEBAPP_API_CLIENT, client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json; charset=utf-8");
            })
            .AddHttpMessageHandler<WebAppHttpMessageHandler>()
            .AddPolicyHandler
            (
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            );

        builder.Services.AddFluxor(options =>
        {
            options.ScanAssemblies(typeof(Program).Assembly);
            options.AddMiddleware<FetchMiddleware>();
#if DEBUG
            options.UseReduxDevTools();
#endif
        });


        builder.Services.AddMudServices();

        var app = builder.Build();

        await app.RunAsync();
    }
}

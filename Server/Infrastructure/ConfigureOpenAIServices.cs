using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using WebApp.Server.Infrastructure.Options;
using WebApp.Server.OpenAIPlugins;

namespace WebApp.Server.Infrastructure;

public static class ConfigureOpenAIServices
{
    public static IServiceCollection AddOpenAIServices(this IServiceCollection services, AzureOpenAI options)
    {
        services.AddScoped<UserGamePredictionPlugin>();

        services.AddKernel()
            .AddAzureOpenAIChatCompletion
            (
                deploymentName: options.DeploymentName,
                endpoint: options.Endpoint,
                apiKey: options.ApiKey
            );

        return services;
    }
}

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using WebApp.Server.Infrastructure.Options;
using WebApp.Server.OpenAIPlugins;

namespace WebApp.Server.Infrastructure;

public static class ConfigureOpenAIServices
{
    public static IServiceCollection AddOpenAIServices(this IServiceCollection services)
    {
        services.AddScoped<UserGamePredictionPlugin>();

        services.AddScoped(serviceProvider =>
        {
            var builder = Kernel.CreateBuilder();

            var azureOpenAIOptions = serviceProvider.GetRequiredService<IOptions<AzureOpenAI>>();

            builder.AddAzureOpenAIChatCompletion
            (
                deploymentName: azureOpenAIOptions.Value.DeploymentName,
                endpoint: azureOpenAIOptions.Value.Endpoint,
                apiKey: azureOpenAIOptions.Value.ApiKey
            );

            var kernel = builder.Build();

            kernel.Plugins.AddFromObject(serviceProvider.GetRequiredService<UserGamePredictionPlugin>(), nameof(UserGamePredictionPlugin));

            return kernel;
        });

        return services;
    }
}

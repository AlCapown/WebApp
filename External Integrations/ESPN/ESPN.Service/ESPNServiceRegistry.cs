using Microsoft.Extensions.DependencyInjection;
using WebApp.ExternalIntegrations.ESPN.Service.Api;

namespace WebApp.ExternalIntegrations.ESPN.Service;

public static class ESPNServiceRegistry
{
    public static IServiceCollection RegisterESPNServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IESPNApi, ESPNApi>()
            .AddHttpClient(Constants.ESPN_SERVICE_NAME);

        return serviceCollection;
    }
}

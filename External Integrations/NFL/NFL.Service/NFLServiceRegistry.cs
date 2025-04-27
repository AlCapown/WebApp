using Microsoft.Extensions.DependencyInjection;
using WebApp.ExternalIntegrations.NFL.Service.Api;

namespace WebApp.ExternalIntegrations.NFL.Service
{
    public static class NFLServiceRegistry
    {
        public static IServiceCollection RegisterNFLServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<INFLApi, NFLApi>()
                .AddHttpClient(Constants.NFL_SERVICE_NAME);

            return serviceCollection;
        }
    }
}

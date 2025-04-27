using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WebApp.ExternalIntegrations.ESPN.Service;

namespace WebApp.Server.Infrastructure;

public static class WebAppServiceRegistry
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection serviceCollection)
    {
        Assembly[] assemblies =
        [
            // Services
            typeof(ESPNServiceRegistry).Assembly,

            // Host
            typeof(Program).Assembly
        ];

        serviceCollection
            .AddMediatR(c =>
            {
                c.RegisterServicesFromAssemblies(assemblies);
            })
            .RegisterESPNServices();

        return serviceCollection;
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebApp.Common.Infrastructure;

namespace WebApp.Client.Infrastructure;

internal static class AddTimeProvider
{
    public static IServiceCollection AddTimeProviderService(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        if (isDevelopment)
        {
            var frozenDateTime = configuration.GetValue<string>("Development:FrozenDateTime");
            if (DateTimeOffset.TryParse(frozenDateTime, out var parsedFrozenDateTime))
            {
                return services.AddSingleton<TimeProvider>(new FrozenTimeProvider(parsedFrozenDateTime));
            }

            var simulatedDateTime = configuration.GetValue<string>("Development:SimulatedDateTime");
            if (DateTimeOffset.TryParse(simulatedDateTime, out var parsedSimulatedDateTime))
            {
                return services.AddSingleton<TimeProvider>(new SimulatedTimeProvider(parsedSimulatedDateTime));
            }
        }

        return services.AddSingleton(TimeProvider.System);
    }
}

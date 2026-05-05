using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebApp.Common.Infrastructure;

namespace WebApp.Client.Infrastructure;

internal static class AddTimeProvider
{
    public static IServiceCollection AddTimeProviderService(this IServiceCollection services, WebAssemblyHostConfiguration configuration, bool isDevelopment)
    {
        if (isDevelopment)
        {
            var frozenTime = configuration.GetValue<string>("Development:FrozenTime");
            if (DateTimeOffset.TryParse(frozenTime, out var frozenDate))
            {
                return services.AddSingleton<TimeProvider>(new FrozenTimeProvider(frozenDate));
            }

            var countDownTime = configuration.GetValue<string>("Development:CountdownTime");
            if (DateTimeOffset.TryParse(countDownTime, out var countdownDate))
            {
                return services.AddSingleton<TimeProvider>(new CountdownTimeProvider(countdownDate));
            }
        }

        return services.AddSingleton(TimeProvider.System);
    }
}

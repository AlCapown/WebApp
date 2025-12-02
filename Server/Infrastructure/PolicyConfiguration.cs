using Microsoft.Extensions.DependencyInjection;
using WebApp.Common.Constants;

namespace WebApp.Server.Infrastructure;

public static class PolicyConfiguration
{
    /// <summary>
    /// Adds authorization policies to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddPolicies(this IServiceCollection services)
    {
        services
            .AddAuthorizationBuilder()
            .AddPolicy(Policy.ADMIN, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRole.ADMIN);
            })
            .AddPolicy(Policy.HANGFIRE, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRole.ADMIN);
            })
            .AddPolicy(Policy.USER, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRole.USER, AppRole.ADMIN);
            });

        return services;
    }
}


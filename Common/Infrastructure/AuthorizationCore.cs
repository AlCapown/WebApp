using Microsoft.Extensions.DependencyInjection;
using WebApp.Common.Constants;

namespace WebApp.Common.Infrastructure;

public static class AuthorizationCore
{
    /// <summary>
    /// Adds authorization and configures policies for WebApp. These are shared by both the client and server.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(Policy.USER, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRole.USER, AppRole.ADMIN);
            });

            options.AddPolicy(Policy.ADMIN, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRole.ADMIN);
            });

            options.AddPolicy(Policy.HANGFIRE, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRole.ADMIN);
            });

        });

        return services;
    }
}


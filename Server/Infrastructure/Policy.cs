using Microsoft.Extensions.DependencyInjection;
using WebApp.Common.Constants;

namespace WebApp.Server.Infrastructure;

public static class Policy
{
    /// <summary>
    /// Policy for regular authenticated users with the "User" role.
    /// Grants access to features and resources available to standard users.
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// Policy for administrators with the "Admin" role.
    /// Grants access to administrative features and resources.
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Policy for accessing Hangfire dashboard and related features.
    /// Restricted to users with the "Admin" role.
    /// </summary>
    public const string Hangfire = "Hangfire";


    /// <summary>
    /// Adds authorization policies to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddPolicies(this IServiceCollection services)
    {
        services
            .AddAuthorizationBuilder()
            .AddPolicy(Admin, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRole.ADMIN);
            })
            .AddPolicy(Hangfire, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRole.ADMIN);
            })
            .AddPolicy(User, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRole.USER);
            });

        return services;
    }
}


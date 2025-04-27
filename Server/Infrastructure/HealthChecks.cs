using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Net.Http.Headers;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace WebApp.Server.Infrastructure;

public static class HealthChecks
{
    public static IServiceCollection RegisterHealthChecks(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        services.AddHealthChecks()
            .AddSqlServer(
                connectionString: configurationManager.GetConnectionString("Database"),
                name: "Database",
                healthQuery: "SELECT 1;",
                failureStatus: HealthStatus.Degraded
            )
            .AddSqlServer(
                connectionString: configurationManager.GetConnectionString("HangfireDatabase"),
                name: "Hangfire",
                healthQuery: "SELECT 1;",
                failureStatus: HealthStatus.Degraded
            );

        return services;
    }

    public static HealthCheckOptions GetHealthCheckOptions()
    {
        return new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter
        };

        static async Task HealthCheckResponseWriter(HttpContext httpContext, HealthReport healthReport)
        {

            httpContext.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(5),
            };

            await httpContext.Response.WriteAsJsonAsync(new
            {
                status = healthReport.Status.ToString(),
                version = Assembly.GetEntryAssembly().GetName().Version.ToString(),
                checks = healthReport.Entries.Select(e => new
                {
                    description = e.Key,
                    status = e.Value.Status.ToString(),
                    statusDetails = e.Value.Description,
                    data = e.Value.Data.Select(p => new { p.Key, p.Value }),
                    responseTime = e.Value.Duration.TotalMilliseconds
                }),
                totalResponseTime = healthReport.TotalDuration.TotalMilliseconds
            }, httpContext.RequestAborted);
        }
    }
}
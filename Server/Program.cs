﻿using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;
using StackExchange.Redis;
using System;
using System.Text.Json.Serialization;
using WebApp.Database;
using WebApp.Database.Tables;
using WebApp.ExternalIntegrations.ESPN.Service;
using WebApp.Server.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.AddServerHeader = true;
});

var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;

// Configure x-forwarded headers
services
    .Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // Private LAN IP Range: 172.16.0.0 - 172.16.255.255
        options.KnownNetworks.Add(IPNetwork.Parse("172.16.0.0/12"));
    });

// Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("Redis");
    options.InstanceName = "WebApp";
});

builder.Services.AddHybridCache();


// Register Data Protection
services
    .AddDataProtection()
    .SetApplicationName("WebApp")
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));

// Register Controllers with Json Serialization Options
services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Register Razor Pages for login/account pages
services.AddRazorPages();

// Source Generator version of Mediator
services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

services.RegisterESPNServices();

// Fluent Validation
services.AddValidatorsFromAssemblyContaining<Program>();


// Exception Handling Middleware
services.AddExceptionHandler<ApiExceptionHandler>();

// Register HealthChecks
services.RegisterHealthChecks(configuration);

// Register logging provider
services
    .AddSerilog(configureLogger =>
    {
        configureLogger
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console();
    });

// Register Database Entity Maps
services
    .Scan(scan => scan.FromAssemblyOf<IEntityTypeMap>()
    .AddClasses(classes => classes.AssignableTo<IEntityTypeMap>())
    .AsImplementedInterfaces()
    .WithTransientLifetime());

// Register Entity Framework Database Context with MsSql
services
    .AddDbContext<WebAppDbContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("Database"), ServerOptions =>
        {
            ServerOptions.MigrationsAssembly("WebApp.Database");
        });

        if (environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
        }
    });

// Register Anti-forgery token to protect against XSRF vulnerabilities
services
    .AddAntiforgery(options =>
    {
        options.HeaderName = "X-XSRF-TOKEN";
        options.Cookie.Name = "X-XSRF-TOKEN";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// Register Microsoft Identity for individual user accounts
services
    .AddIdentityCore<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddUserManager<UserManager<AppUser>>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<WebAppDbContext>();

// Register Authentication Providers
services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
        microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
        microsoftOptions.AuthorizationEndpoint = configuration["Authentication:Microsoft:AuthorizationEndpoint"];
        microsoftOptions.TokenEndpoint = configuration["Authentication:Microsoft:TokenEndpoint"];
        microsoftOptions.UsePkce = true;
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
        googleOptions.AuthorizationEndpoint = configuration["Authentication:Google:AuthorizationEndpoint"];
        googleOptions.TokenEndpoint = configuration["Authentication:Google:TokenEndpoint"];
        googleOptions.UsePkce = true;
    })
    .AddOpenIdConnect("Yahoo", "Yahoo", yahooOptions =>
    {
        yahooOptions.ClientId = configuration["Authentication:Yahoo:ClientId"];
        yahooOptions.ClientSecret = configuration["Authentication:Yahoo:ClientSecret"];
        yahooOptions.Authority = configuration["Authentication:Yahoo:AuthorityEndpoint"];
        yahooOptions.ResponseType = OpenIdConnectResponseType.Code;
        yahooOptions.UsePkce = true;
        yahooOptions.Scope.Add("email");
        yahooOptions.Scope.Add("profile");
    })
    .AddIdentityCookies(options =>
    {
        options.ApplicationCookie.Configure(options =>
        {
            options.LogoutPath = "/Account/Logout";
            options.LoginPath = "/Account/Login";
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
            options.Cookie.Name = "WebAppCookie";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    });


// Add Authorization Policies
services
    .AddPolicies();

// Add Hangfire for background job processing
services
    .AddHangfire(options =>
    {
        options.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
        options.UseSimpleAssemblyNameTypeSerializer();
        options.UseRecommendedSerializerSettings();
        options.UseSqlServerStorage(configuration.GetConnectionString("HangfireDatabase"), new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true,
            EnableHeavyMigrations = true, // Automatic updates to schema versions
        });
    })
    .AddHangfireServer(options =>
    {
        options.WorkerCount = 1;
    });


var app = builder.Build();


#if DEBUG
app.Logger.LogInformation("Application Name: {Application}", app.Environment.ApplicationName);
app.Logger.LogInformation("Hosting Environment: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("WebApp Sql Connection String: {SQLConnection}", app.Configuration.GetConnectionString("Database"));
app.Logger.LogInformation("Hangfire Sql Connection String: {HangfireConnection}", app.Configuration.GetConnectionString("HangfireDatabase"));
app.Logger.LogInformation("Redis Connection String: {RedisConnection}", app.Configuration.GetConnectionString("Redis"));
#endif

app.UseForwardedHeaders();

app.UseWhen(x => x.Request.Path.Value.StartsWith("/api", StringComparison.InvariantCultureIgnoreCase), builder =>
{
    builder.UseExceptionHandler("/");
});

if (app.Environment.IsDevelopment())
{
    app.UseWhen(x => !x.Request.Path.Value.StartsWith("/api", StringComparison.InvariantCultureIgnoreCase), builder =>
    {
        builder.UseDeveloperExceptionPage();
    });

    app.UseWebAssemblyDebugging();
}
else
{
    app.UseWhen(x => !x.Request.Path.Value.StartsWith("/api", StringComparison.InvariantCultureIgnoreCase), builder =>
    {
        builder.UseExceptionHandler("/error");
    });

    app.UseHsts();
}


// Ensure cross origin isolation is enabled. This is required for
// multithreading support in WASM (when it is finally implemented).
app.Use((context, next) =>
{
    context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
    context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
    return next.Invoke();
});


app.UseBlazorFrameworkFiles();
app.MapStaticAssets();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHangfireDashboardWithAuthorizationPolicy(Policy.Hangfire);
app.MapFallbackToPage("/WasmIndex");
app.UseHealthChecks("/api/healthchecks", WebApp.Server.Infrastructure.HealthChecks.GetHealthCheckOptions());

try
{
    app.Logger.LogInformation("Database migration started.");
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<WebAppDbContext>();
    await dbContext.Database.MigrateAsync();
    app.Logger.LogInformation("Database migration completed successfully.");
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Database migration failed.");
    throw;
}

ReoccurringJobsScheduler.Schedule();

await app.RunAsync();

public partial class Program { }
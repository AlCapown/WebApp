using Microsoft.Extensions.Caching.Hybrid;
using System;

namespace WebApp.Server.Infrastructure;

public static class CacheOptions
{
    /// <summary>
    /// Cache configuration with a 20 minute local cache expiration and 60 minute distributed cache expiration.
    /// </summary>
    public static HybridCacheEntryOptions STANDARD_L20_D60 { get; } = new HybridCacheEntryOptions
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(20),
        Expiration = TimeSpan.FromMinutes(60),
    };
}

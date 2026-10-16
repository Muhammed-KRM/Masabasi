using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Masabasi.API.Extensions;

/// <summary>
/// Vitrin (Public) trafiğinin aşırı yüklenmesi durumunda Admin panelinin ve API'nin etkilenmemesi için
/// izole edilmiş hız sınırlama (Rate Limiting) havuzlarını yapılandıran uzantı sınıfı.
/// </summary>
public static class RateLimitingExtensions
{
    public const string PublicReadPolicy = "PublicReadPolicy";
    public const string PublicReservationPolicy = "PublicReservationPolicy";
    public const string AdminIsolatedPolicy = "AdminIsolatedPolicy";

    public static IServiceCollection AddTrafficIsolationPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // 1. Halka Açık Okuma Havuzu: IP başına dakikada 120 istek (Vitrin okuma trafiği)
            options.AddPolicy(PublicReadPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anon-read",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 120,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            // 2. Masa Koltuk Kayıt Havuzu (Bot / Spam Koruması): IP başına 10 dakikada en fazla 5 kayıt denemesi
            options.AddPolicy(PublicReservationPolicy, httpContext =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anon-reserve",
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(10),
                        SegmentsPerWindow = 2,
                        QueueLimit = 0
                    }));

            // 3. İzole Admin Havuzu: Public havuzları tamamen dolsa dahi Admin işlemleri kendi özel token havuzundan engelsiz çalışır
            options.AddPolicy(AdminIsolatedPolicy, httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "admin-pool",
                    factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 500,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        TokensPerPeriod = 500,
                        AutoReplenishment = true
                    }));
        });

        return services;
    }
}

using Masabasi.Data.Context;
using Masabasi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Masabasi.Data;

/// <summary>
/// Veri erişim katmanı servislerinin ve EF Core veritabanı bağlamının DI (Dependency Injection) kayıtlarını sağlayan uzantı sınıfı.
/// </summary>
public static class DataServiceRegistration
{
    /// <summary>
    /// Masabaşı veri erişim katmanı bağımlılıklarını (DbContext, Repository'ler) servis koleksiyonuna kaydeder.
    /// </summary>
    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        bool useInMemory = string.IsNullOrWhiteSpace(connectionString) ||
                           connectionString.StartsWith("InMemory:", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(configuration["UseInMemoryDatabase"], "true", StringComparison.OrdinalIgnoreCase);

        services.AddDbContext<MasabasiDbContext>(options =>
        {
            if (!useInMemory && !string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(MasabasiDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                });
            }
            else
            {
                // Bağlantı cümlesi verilmemişse veya InMemory belirtilmişse bellek içi sağlayıcı kullanılır
                string dbName = connectionString?.StartsWith("InMemory:", StringComparison.OrdinalIgnoreCase) == true
                    ? connectionString["InMemory:".Length..]
                    : "MasabasiDevDb";
                options.UseInMemoryDatabase(string.IsNullOrWhiteSpace(dbName) ? "MasabasiDevDb" : dbName);
            }
        });

        // Depo (Repository) kayıtları
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IEventRepository, EventRepository>();

        return services;
    }
}

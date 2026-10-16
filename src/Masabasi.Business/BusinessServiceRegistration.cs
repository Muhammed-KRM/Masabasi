using FluentValidation;
using Masabasi.Business.Interfaces;
using Masabasi.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Masabasi.Business;

/// <summary>
/// Masabaşı iş mantığı, doğrulama ve önbellek servislerinin DI (Dependency Injection) kayıtlarını sağlayan uzantı sınıfı.
/// </summary>
public static class BusinessServiceRegistration
{
    /// <summary>
    /// İş katmanı bağımlılıklarını (HybridCache, FluentValidation, Servisler) servis koleksiyonuna kaydeder.
    /// </summary>
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        // 1. .NET 10 HybridCache Kaydı (L1 In-Memory + L2 Dağıtık Önbellek)
        #pragma warning disable EXTEXP0018 // HybridCache preview/experimental warning if any
        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024; // 1 MB
            options.DefaultEntryOptions = new()
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromSeconds(30)
            };
        });
        #pragma warning restore EXTEXP0018

        // 2. FluentValidation Doğrulayıcılarının Derlemeden Otomatik Taranarak Kaydedilmesi
        services.AddValidatorsFromAssembly(typeof(BusinessServiceRegistration).Assembly);

        // 3. İş Mantığı Servis Kayıtları
        services.AddScoped<IStaticSnapshotService, StaticSnapshotManager>();
        services.AddScoped<IEventService, EventManager>();
        services.AddScoped<IGameCatalogService, GameCatalogManager>();
        services.AddScoped<IArticleService, ArticleManager>();

        return services;
    }
}

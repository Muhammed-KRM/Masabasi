using FluentAssertions;
using FluentValidation;
using Masabasi.Business;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.Interfaces;
using Masabasi.Data;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Masabasi.Data.Repositories;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Masabasi.UnitTests.Business;

/// <summary>
/// Data ve Business katmanlarının modüler DI extension metotlarını (AddDataLayer, AddBusinessLayer) test eden sınıf.
/// </summary>
public sealed class ServiceRegistrationTests
{
    [Fact]
    public void AddDataLayer_And_AddBusinessLayer_ShouldResolveAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDataLayer(configuration);
        services.AddBusinessLayer();

        var serviceProvider = services.BuildServiceProvider();

        // Assert - Data Layer
        serviceProvider.GetService<MasabasiDbContext>().Should().NotBeNull();
        serviceProvider.GetService<IGenericRepository<BoardGame>>().Should().NotBeNull();
        serviceProvider.GetService<IEventRepository>().Should().NotBeNull();

        // Assert - Business Layer
        serviceProvider.GetService<HybridCache>().Should().NotBeNull();
        serviceProvider.GetService<IValidator<CreateReservationDto>>().Should().NotBeNull();
        serviceProvider.GetService<IEventService>().Should().NotBeNull();
        serviceProvider.GetService<IGameCatalogService>().Should().NotBeNull();
        serviceProvider.GetService<IArticleService>().Should().NotBeNull();
        serviceProvider.GetService<IStaticSnapshotService>().Should().NotBeNull();
    }
}

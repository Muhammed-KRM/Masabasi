using System.Net;
using FluentAssertions;
using Xunit;

namespace Masabasi.UnitTests.Integration;

/// <summary>
/// Sistem canlılık ve sağlık kontrolü (Health Check) uç noktasını doğrulayan E2E entegrasyon testi.
/// </summary>
public sealed class HealthCheckE2ETests : IClassFixture<MasabasiApiFactory>
{
    private readonly HttpClient _client;

    public HealthCheckE2ETests(MasabasiApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ShouldReturn200Ok_AndHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }
}

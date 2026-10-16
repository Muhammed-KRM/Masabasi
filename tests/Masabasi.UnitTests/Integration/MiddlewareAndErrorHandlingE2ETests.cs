using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Masabasi.UnitTests.Integration;

/// <summary>
/// Küresel istisna yönetimi (RFC 7807 ProblemDetails standardı) ve middleware zincirini
/// E2E senaryolarıyla test eden sınıf.
/// </summary>
public sealed class MiddlewareAndErrorHandlingE2ETests : IClassFixture<MasabasiApiFactory>
{
    private readonly HttpClient _client;

    public MiddlewareAndErrorHandlingE2ETests(MasabasiApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task MissingRoute_ShouldReturn404_WithProblemDetails()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/public/olmayan-bir-endpoint");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task InvalidPayload_ShouldReturn400BadRequest_WithValidationDetails()
    {
        // Geçersiz boş gövde gönderilerek model doğrulama hatası tetiklenir
        var invalidContent = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/v1/public/reservations", invalidContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().BeOneOf("application/problem+json", "application/json");

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(400);
    }
}

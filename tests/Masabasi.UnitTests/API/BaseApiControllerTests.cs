using FluentAssertions;
using Masabasi.API.Controllers;
using Masabasi.Business.Common;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Masabasi.UnitTests.API;

/// <summary>
/// BaseApiController HandleResult metodunun HTTP durum kodu eşlemelerini test eden sınıf.
/// </summary>
public sealed class BaseApiControllerTests
{
    private sealed class TestController : BaseApiController
    {
        public IActionResult TestHandleResult<T>(ServiceResult<T> result) => HandleResult(result);
    }

    private readonly TestController _controller = new();

    [Fact]
    public void HandleResult_ShouldReturnOk_WhenSuccessWith200()
    {
        var result = ServiceResult<string>.Success("Veri", 200);
        var actionResult = _controller.TestHandleResult(result);

        var ok = actionResult as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().Be("Veri");
    }

    [Fact]
    public void HandleResult_ShouldReturnCreated_WhenSuccessWith201()
    {
        var result = ServiceResult<string>.Success("Yeni Kaynak", 201);
        var actionResult = _controller.TestHandleResult(result);

        var obj = actionResult as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(201);
        obj.Value.Should().Be("Yeni Kaynak");
    }

    [Fact]
    public void HandleResult_ShouldReturnNoContent_WhenSuccessWith204()
    {
        var result = ServiceResult<string?>.Success(null, 204);
        var actionResult = _controller.TestHandleResult(result);

        var noContent = actionResult as NoContentResult;
        noContent.Should().NotBeNull();
        noContent!.StatusCode.Should().Be(204);
    }

    [Theory]
    [InlineData(400)]
    [InlineData(404)]
    [InlineData(409)]
    [InlineData(500)]
    public void HandleResult_ShouldReturnExpectedStatusCode_WhenFailure(int statusCode)
    {
        var result = ServiceResult<string>.Failure("Hata Mesajı", statusCode);
        var actionResult = _controller.TestHandleResult(result);

        var obj = actionResult as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(statusCode);
    }
}

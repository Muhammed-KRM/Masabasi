using FluentAssertions;
using Masabasi.API.Controllers.Admin;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Masabasi.UnitTests.API;

/// <summary>
/// SnapshotAdminController yönetim ucu testleri.
/// </summary>
public sealed class SnapshotAdminControllerTests
{
    [Fact]
    public async Task ExportAllSnapshots_ShouldCallExportService_AndReturnOk()
    {
        // Arrange
        var snapshotServiceMock = new Mock<IStaticSnapshotService>();
        var controller = new SnapshotAdminController(snapshotServiceMock.Object);

        // Act
        var result = await controller.ExportAllSnapshots(CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        snapshotServiceMock.Verify(s => s.ExportAllSnapshotsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

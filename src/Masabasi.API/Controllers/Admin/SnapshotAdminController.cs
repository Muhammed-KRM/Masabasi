using Masabasi.API.Extensions;
using Masabasi.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Masabasi.API.Controllers.Admin;

/// <summary>
/// Statik snapshot paketini (/public/data/*.json, /public/llms.txt, /public/sitemap.xml)
/// tek tıkla yeniden üretme ucu.
/// </summary>
[Route("api/v1/admin/snapshot")]
[EnableRateLimiting(RateLimitingExtensions.AdminIsolatedPolicy)]
public sealed class SnapshotAdminController : BaseApiController
{
    private readonly IStaticSnapshotService _snapshotService;

    public SnapshotAdminController(IStaticSnapshotService snapshotService)
    {
        _snapshotService = snapshotService;
    }

    /// <summary>
    /// Tüm veritabanı verisini statik JSON snapshot ve AI llms.txt dosyaları olarak dışa aktarır.
    /// </summary>
    [HttpPost("export-all")]
    public async Task<IActionResult> ExportAllSnapshots(CancellationToken cancellationToken)
    {
        await _snapshotService.ExportAllSnapshotsAsync(cancellationToken);
        return Ok(new
        {
            success = true,
            message = "Tüm statik snapshot dosyaları, llms.txt ve sitemap.xml başarıyla üretildi.",
            exportedAtUtc = DateTimeOffset.UtcNow
        });
    }
}

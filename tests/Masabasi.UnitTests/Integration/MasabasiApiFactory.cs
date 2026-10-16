using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Masabasi.UnitTests.Integration;

/// <summary>
/// Masabaşı API'sinin uçtan uca (E2E) entegrasyon testleri için In-Memory veritabanı ve
/// test konfigürasyonlarını izole eden WebApplicationFactory sınıfı.
/// </summary>
public sealed class MasabasiApiFactory : WebApplicationFactory<Program>
{
    private static readonly string UniqueDb = "InMemory:Masabasi_Test_Db_" + Guid.NewGuid();
    private readonly string _testSnapshotDir = Path.Combine(Path.GetTempPath(), "masabasi_test_snapshots_" + Guid.NewGuid());

    static MasabasiApiFactory()
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", UniqueDb);
        Environment.SetEnvironmentVariable("UseInMemoryDatabase", "true");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = UniqueDb,
                ["UseInMemoryDatabase"] = "true",
                ["SnapshotSettings:FrontendPublicPath"] = _testSnapshotDir
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        try
        {
            if (Directory.Exists(_testSnapshotDir))
            {
                Directory.Delete(_testSnapshotDir, true);
            }
        }
        catch
        {
            // Test sonrası geçici klasör temizleme hatası göz ardı edilir
        }
    }
}

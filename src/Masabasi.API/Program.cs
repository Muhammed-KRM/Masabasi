using Masabasi.API.Extensions;
using Masabasi.API.Hubs;
using Masabasi.API.Middlewares;
using Masabasi.Business;
using Masabasi.Data;
using Masabasi.Data.Context;
using Masabasi.Data.Seed;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veri Erişim Katmanı (EF Core 10, PostgreSQL & DbContext)
builder.Services.AddDataLayer(builder.Configuration);

// 2. İş Mantığı Katmanı (HybridCache, FluentValidation, Managers)
builder.Services.AddBusinessLayer();

// 3. Vitrin vs. Admin Trafik İzolasyonu Politikaları (Rate Limiting)
builder.Services.AddTrafficIsolationPolicies();

// 4. OutputCache (Vitrin Halka Açık Yanıtlarını RAM'de Önbellekleme)
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(p => p.Expire(TimeSpan.FromSeconds(30)));
});

// 5. SignalR (Canlı Masa Kontenjanı Değişim Hub'ı)
builder.Services.AddSignalR();

// 6. Controller Kaydı
builder.Services.AddControllers();

// 7. OpenAPI ve Dokümantasyon
builder.Services.AddOpenApi();

// 8. Health Checks (Angular ResilientDataService için 200ms API kontrolü)
builder.Services.AddHealthChecks();

// 9. CORS Yapılandırması (Angular SSR ve CSR İstemcileri İçin)
const string MasabasiCorsPolicy = "MasabasiCorsPolicy";
string[] allowedOrigins = builder.Configuration
    .GetSection("CorsSettings:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:4200", "http://localhost:4000", "https://masabasikulubu.com"];

builder.Services.AddCors(options =>
{
    options.AddPolicy(MasabasiCorsPolicy, policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Küresel Hata Yakalama Middleware'i
app.UseMiddleware<GlobalExceptionMiddleware>();

// OpenAPI ve Scalar UI (Geliştirme Ortamı)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Masabaşı Oyun Kulübü API";
        options.Theme = ScalarTheme.Mars;
    });
}

// CORS ve Güvenlik
app.UseCors(MasabasiCorsPolicy);

// Hız Sınırlama (Rate Limiting) ve Önbellek (OutputCache)
app.UseRateLimiter();
app.UseOutputCache();

// Uç Nokta Eşlemeleri
app.MapControllers();
app.MapHub<TableHub>("/hubs/table");

// Angular için Hızlı Sağlık Kontrolü Ucu
app.MapHealthChecks("/health");

// Başlangıç Veritabanı Tohumlama (Seeding)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<MasabasiDbContext>();
        await MasabasiDbSeeder.SeedAsync(db, app.Logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Başlangıç veritabanı tohumlama işlemi sırasında geçici bir uyarı oluştu.");
    }
}

app.Run();

// Entegrasyon ve API testleri için Program sınıfı görünürlüğü
public partial class Program { }

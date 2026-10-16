using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Masabasi.API.Middlewares;

/// <summary>
/// API genelinde yakalanmamış tüm istisnaları (unhandled exceptions) yakalayan,
/// loglayan ve istemciye standart RFC 7807 ProblemDetails dönen küresel ara katman (middleware).
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            string traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            _logger.LogError(ex, "İşlenmemiş API hatası yakalandı. İstek Yolu: {Path}, TraceId: {TraceId}", context.Request.Path, traceId);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Sunucu Hatası",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "İşlem sırasında beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;

            string json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }
}

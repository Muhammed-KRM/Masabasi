using Masabasi.Business.Common;
using Microsoft.AspNetCore.Mvc;

namespace Masabasi.API.Controllers;

/// <summary>
/// Masabaşı API controller'ları için ortak ServiceResult dönüşüm mantığını barındıran temel controller.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// ServiceResult sonucunu HTTP durum koduna uygun ActionResult yanıtına dönüştürür.
    /// </summary>
    protected IActionResult HandleResult<T>(ServiceResult<T> result)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                StatusCodes.Status201Created => StatusCode(StatusCodes.Status201Created, result.Data),
                StatusCodes.Status204NoContent => NoContent(),
                _ => Ok(result.Data)
            };
        }

        var errorResponse = new
        {
            error = result.ErrorMessage ?? "İşlem sırasında bir hata oluştu.",
            statusCode = result.StatusCode
        };

        return StatusCode(result.StatusCode, errorResponse);
    }
}

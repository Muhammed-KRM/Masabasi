namespace Masabasi.Business.Common;

/// <summary>
/// İş katmanı metotlarının istisna (exception) fırlatmadan standart sonuç ve hata durumu döndürmesini sağlayan genel sarmalayıcı.
/// </summary>
/// <typeparam name="T">İşlem sonucunda taşınan veri tipi.</typeparam>
public sealed record ServiceResult<T>
{
    /// <summary>İşlemin başarılı olup olmadığını belirtir.</summary>
    public bool IsSuccess { get; init; }

    /// <summary>Başarılı işlem sonucunda dönen veri.</summary>
    public T? Data { get; init; }

    /// <summary>Hata durumunda kullanıcıya veya API yanıtına dönülecek açıklama mesajı.</summary>
    public string? ErrorMessage { get; init; }

    /// <summary>HTTP durum kodu karşılığı (200, 201, 400, 404, 409, 500 vb.).</summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Başarılı bir sonuç nesnesi oluşturur.
    /// </summary>
    public static ServiceResult<T> Success(T data, int statusCode = 200) =>
        new() { IsSuccess = true, Data = data, StatusCode = statusCode };

    /// <summary>
    /// Hatalı bir sonuç nesnesi oluşturur.
    /// </summary>
    public static ServiceResult<T> Failure(string errorMessage, int statusCode = 400) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };
}

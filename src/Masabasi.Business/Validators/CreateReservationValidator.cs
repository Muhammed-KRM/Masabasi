using FluentValidation;
using Masabasi.Business.DTOs.Events;

namespace Masabasi.Business.Validators;

/// <summary>
/// Masa koltuk rezervasyonu isteklerinin geçerlilik kurallarını doğrulayan FluentValidation sınıfı.
/// </summary>
public sealed class CreateReservationValidator : AbstractValidator<CreateReservationDto>
{
    private static readonly string[] AllowedExperienceLevels = ["Beginner", "Intermediate", "Veteran"];

    public CreateReservationValidator()
    {
        RuleFor(x => x.GameTableId)
            .NotEmpty()
            .WithMessage("Rezervasyon yapılacak oyun masası kimliği zorunludur.");

        RuleFor(x => x.PlayerName)
            .NotEmpty().WithMessage("Katılımcı adı ve soyadı zorunludur.")
            .Length(2, 80).WithMessage("Katılımcı adı 2 ile 80 karakter arasında olmalıdır.");

        RuleFor(x => x.ContactHandle)
            .NotEmpty().WithMessage("İletişim bilgisi zorunludur.")
            .Must(BeValidContactHandle)
            .WithMessage("İletişim bilgisi geçerli bir telefon numarası veya '@' ile başlayan Instagram kullanıcı adı olmalıdır.");

        RuleFor(x => x.ExperienceLevel)
            .NotEmpty().WithMessage("Deneyim seviyesi seçilmelidir.")
            .Must(level => AllowedExperienceLevels.Contains(level))
            .WithMessage("Geçersiz deneyim seviyesi. ('Beginner', 'Intermediate' veya 'Veteran' olmalıdır)");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Not alanı en fazla 500 karakter olabilir.");
    }

    private static bool BeValidContactHandle(string handle)
    {
        if (string.IsNullOrWhiteSpace(handle))
            return false;

        string trimmed = handle.Trim();

        // Instagram kullanıcı adı kontrolü: '@kullanici_adi'
        if (trimmed.StartsWith('@') && trimmed.Length >= 3)
            return true;

        // Telefon numarası kontrolü (en az 10 basamak veya standart rakamlar)
        string digitsOnly = new(trimmed.Where(char.IsDigit).ToArray());
        return digitsOnly.Length >= 10;
    }
}

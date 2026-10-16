using FluentValidation;
using Masabasi.Business.DTOs.Events;

namespace Masabasi.Business.Validators;

/// <summary>
/// Cumartesi etkinliği oluşturma isteklerinin kurallarını denetleyen validator.
/// </summary>
public sealed class CreateEventValidator : AbstractValidator<CreateEventDto>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Etkinlik başlığı zorunludur.")
            .Length(5, 200).WithMessage("Etkinlik başlığı 5 ile 200 karakter arasında olmalıdır.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("SEO kısa adı (slug) zorunludur.")
            .Matches("^[a-z0-9-]+$").WithMessage("Slug yalnızca küçük harf, rakam ve tire (-) içerebilir.");

        RuleFor(x => x.TldrSummary)
            .NotEmpty().WithMessage("Etkinlik özeti zorunludur.")
            .Length(20, 500).WithMessage("Etkinlik özeti 20 ile 500 karakter arasında olmalıdır.");

        RuleFor(x => x.StartDateUtc)
            .GreaterThan(DateTimeOffset.UtcNow.AddMinutes(-30))
            .WithMessage("Etkinlik başlangıç saati geçmiş bir tarih olamaz.");

        RuleFor(x => x.EndDateUtc)
            .GreaterThan(x => x.StartDateUtc)
            .WithMessage("Etkinlik bitiş saati başlangıç saatinden sonra olmalıdır.");

        RuleFor(x => x.VenueName)
            .NotEmpty().WithMessage("Mekan adı zorunludur.")
            .MaximumLength(150);

        RuleFor(x => x.VenueAddress)
            .NotEmpty().WithMessage("Mekan adresi zorunludur (Google Event Schema için gereklidir).")
            .MaximumLength(300);
    }
}

using FluentValidation;
using Masabasi.Business.DTOs.Articles;

namespace Masabasi.Business.Validators;

/// <summary>
/// Blog ve Topic Cluster içeriklerinin SEO ve GEO standartlarına uygunluğunu doğrulayan validator.
/// </summary>
public sealed class CreateArticleValidator : AbstractValidator<CreateArticleDto>
{
    private static readonly string[] AllowedTopicClusters =
    [
        "KutuOyunlari",
        "FrpDnD",
        "KartOyunlari",
        "DijitaldenMasaustune",
        "CosplayAksesuar",
        "KulupHaberleri"
    ];

    public CreateArticleValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Makale başlığı zorunludur.")
            .Length(25, 75).WithMessage("SEO açısından başlık 25 ile 75 karakter arasında olmalıdır.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("URL kısa adı (slug) zorunludur.")
            .Matches("^[a-z0-9-]+$").WithMessage("Slug yalnızca küçük harf, rakam ve tire (-) içerebilir.");

        RuleFor(x => x.TopicCluster)
            .NotEmpty().WithMessage("Topic Cluster kategorisi seçilmelidir.")
            .Must(cluster => AllowedTopicClusters.Contains(cluster))
            .WithMessage("Geçersiz Topic Cluster kategorisi.");

        RuleFor(x => x.MetaDescription)
            .NotEmpty().WithMessage("SEO meta açıklaması zorunludur.")
            .Length(90, 180).WithMessage("SEO meta açıklaması 90 ile 180 karakter arasında olmalıdır.");

        RuleFor(x => x.TldrSummary)
            .NotEmpty().WithMessage("Yapay Zeka (GEO) ve hızlı okuma için TL;DR özeti zorunludur.")
            .Must(BeAdequateWordCount)
            .WithMessage("TL;DR özeti 20 ile 80 kelime arasında olmalıdır.");

        RuleFor(x => x.ContentMarkdown)
            .NotEmpty().WithMessage("Makale içeriği zorunludur.")
            .MinimumLength(100).WithMessage("Makale içeriği en az 100 karakter olmalıdır.");

        RuleFor(x => x.AuthorName)
            .NotEmpty().WithMessage("Yazar adı zorunludur.")
            .MaximumLength(100).WithMessage("Yazar adı en fazla 100 karakter olabilir.");
    }

    private static bool BeAdequateWordCount(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var words = text.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        return words.Length is >= 20 and <= 80;
    }
}

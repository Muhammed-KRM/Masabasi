using FluentAssertions;
using Masabasi.Business.DTOs.Articles;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.Validators;
using Xunit;

namespace Masabasi.UnitTests.Business;

/// <summary>
/// FluentValidation doğrulayıcılarının tüm kurallarını ve sınır durumlarını (iletişim, SEO uzunlukları, kelime sayıları, tarih mantığı) test eden sınıf.
/// </summary>
public sealed class ValidatorTests
{
    private readonly CreateReservationValidator _reservationValidator = new();
    private readonly CreateArticleValidator _articleValidator = new();
    private readonly CreateEventValidator _eventValidator = new();

    #region CreateReservationValidator Tests

    [Theory]
    [InlineData("@kullanici", true)]
    [InlineData("05321234567", true)]
    [InlineData("+905321234567", true)]
    [InlineData("a", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void CreateReservationValidator_ShouldValidateContactHandleProperly(string contactHandle, bool expectedValid)
    {
        var dto = new CreateReservationDto(
            GameTableId: Guid.NewGuid(),
            PlayerName: "Test Oyuncu",
            ContactHandle: contactHandle,
            ExperienceLevel: "Beginner",
            Notes: null);

        var result = _reservationValidator.Validate(dto);
        result.IsValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("A", false)]
    [InlineData("Ali", true)]
    public void CreateReservationValidator_ShouldValidatePlayerNameLength(string playerName, bool expectedValid)
    {
        var dto = new CreateReservationDto(
            GameTableId: Guid.NewGuid(),
            PlayerName: playerName,
            ContactHandle: "@oyuncu",
            ExperienceLevel: "Beginner",
            Notes: null);

        var result = _reservationValidator.Validate(dto);
        result.IsValid.Should().Be(expectedValid);
    }

    [Fact]
    public void CreateReservationValidator_ShouldFail_WhenGameTableIdIsEmpty()
    {
        var dto = new CreateReservationDto(
            GameTableId: Guid.Empty,
            PlayerName: "Ahmet",
            ContactHandle: "@ahmet",
            ExperienceLevel: "Beginner",
            Notes: null);

        var result = _reservationValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateReservationDto.GameTableId));
    }

    [Fact]
    public void CreateReservationValidator_ShouldFail_WhenNotesExceeds500Characters()
    {
        var longNotes = new string('x', 501);
        var dto = new CreateReservationDto(
            GameTableId: Guid.NewGuid(),
            PlayerName: "Ahmet",
            ContactHandle: "@ahmet",
            ExperienceLevel: "Beginner",
            Notes: longNotes);

        var result = _reservationValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateReservationDto.Notes));
    }

    #endregion

    #region CreateArticleValidator Tests

    [Fact]
    public void CreateArticleValidator_ShouldFail_WhenTldrWordCountIsTooShort()
    {
        var dto = new CreateArticleDto(
            Title: "D&D 5e Başlangıç Kılavuzu: Zarlar ve Masalar",
            Slug: "dnd-5e-baslangic-kilavuzu",
            TopicCluster: "FrpDnD",
            TldrSummary: "Çok kısa özet.",
            MetaDescription: "D&D 5e başlangıç rehberi hakkında detaylı bilgiler ve ipuçları burada yer almaktadır. Masabaşı Oyun Kulübü.",
            ContentMarkdown: "## Giriş\nDetaylı içerik metni burada yer almaktadır. En az yüz karakter olmalıdır.",
            AuthorName: "Yazar",
            FaqJson: null,
            CoverImageUrl: null);

        var result = _articleValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateArticleDto.TldrSummary));
    }

    [Fact]
    public void CreateArticleValidator_ShouldFail_WhenTldrWordCountExceeds80Words()
    {
        string eightyOneWords = string.Join(" ", Enumerable.Repeat("kelime", 85));

        var dto = new CreateArticleDto(
            Title: "D&D 5e Başlangıç Kılavuzu: Zarlar ve Masalar",
            Slug: "dnd-5e-baslangic-kilavuzu",
            TopicCluster: "FrpDnD",
            TldrSummary: eightyOneWords,
            MetaDescription: "D&D 5e başlangıç rehberi hakkında detaylı bilgiler ve ipuçları burada yer almaktadır. Masabaşı Oyun Kulübü.",
            ContentMarkdown: "## Giriş\nDetaylı içerik metni burada yer almaktadır. En az yüz karakter olmalıdır.",
            AuthorName: "Yazar",
            FaqJson: null,
            CoverImageUrl: null);

        var result = _articleValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateArticleDto.TldrSummary));
    }

    [Theory]
    [InlineData("Gecersiz-Cluster", false)]
    [InlineData("FrpDnD", true)]
    [InlineData("KutuOyunlari", true)]
    [InlineData("DijitaldenMasaustune", true)]
    public void CreateArticleValidator_ShouldValidateTopicClusters(string cluster, bool expectedValid)
    {
        string validTldr = "Masabaşı Oyun Kulübü tarafından hazırlanan bu kapsamlı rehber makalesinde en temel kutu oyunu kuralları, masa stratejileri ve oyuncu taktikleri detaylıca anlatılmaktadır.";
        string validMeta = "Masabaşı Oyun Kulübü kutu oyunları rehberi ile stratejileri, kuralları ve en popüler masa oyunlarını hemen keşfedin.";
        string validMarkdown = new string('m', 150);

        var dto = new CreateArticleDto(
            Title: "Uygun Başlık Uzunluğu Yeterli",
            Slug: "uygun-baslik-uzunlugu",
            TopicCluster: cluster,
            TldrSummary: validTldr,
            MetaDescription: validMeta,
            ContentMarkdown: validMarkdown,
            AuthorName: "Yazar",
            FaqJson: null,
            CoverImageUrl: null);

        var result = _articleValidator.Validate(dto);
        result.IsValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("gecerli-slug-123", true)]
    [InlineData("GECERSIZ_BUYUK_HARF", false)]
    [InlineData("bosluk lu slug", false)]
    [InlineData("turkce_ş_ğ_ü", false)]
    public void CreateArticleValidator_ShouldEnforceLowercaseAlphaNumericSlug(string slug, bool expectedValid)
    {
        string validTldr = "Masabaşı Oyun Kulübü tarafından hazırlanan bu kapsamlı rehber makalesinde en temel kutu oyunu kuralları, masa stratejileri ve oyuncu taktikleri detaylıca anlatılmaktadır.";
        string validMeta = "Masabaşı Oyun Kulübü kutu oyunları rehberi ile stratejileri, kuralları ve en popüler masa oyunlarını hemen keşfedin.";
        string validMarkdown = new string('m', 150);

        var dto = new CreateArticleDto(
            Title: "Uygun Başlık Uzunluğu Yeterli",
            Slug: slug,
            TopicCluster: "KutuOyunlari",
            TldrSummary: validTldr,
            MetaDescription: validMeta,
            ContentMarkdown: validMarkdown,
            AuthorName: "Yazar",
            FaqJson: null,
            CoverImageUrl: null);

        var result = _articleValidator.Validate(dto);
        result.IsValid.Should().Be(expectedValid);
    }

    #endregion

    #region CreateEventValidator Tests

    [Fact]
    public void CreateEventValidator_ShouldFail_WhenStartDateIsInThePast()
    {
        var dto = new CreateEventDto(
            Title: "Geçmiş Tarihli Etkinlik",
            Slug: "gecmis-etkinlik",
            TldrSummary: "Geçmiş tarihli etkinlik için kısa özet açıklaması.",
            StartDateUtc: DateTimeOffset.UtcNow.AddDays(-2),
            EndDateUtc: DateTimeOffset.UtcNow.AddDays(-2).AddHours(5),
            VenueName: "Kadıköy",
            VenueAddress: "Kadıköy",
            MeetupEventUrl: null,
            ExternalRegistrationUrl: null);

        var result = _eventValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateEventDto.StartDateUtc));
    }

    [Fact]
    public void CreateEventValidator_ShouldFail_WhenEndDateIsBeforeStartDate()
    {
        var start = DateTimeOffset.UtcNow.AddDays(2);
        var dto = new CreateEventDto(
            Title: "Tarih Sırası Hatalı Etkinlik",
            Slug: "tarih-sirasi-hatali",
            TldrSummary: "Tarih sırası hatalı etkinlik açıklaması burada.",
            StartDateUtc: start,
            EndDateUtc: start.AddHours(-1), // Bitiş başlangıçtan önce!
            VenueName: "Kadıköy",
            VenueAddress: "Kadıköy",
            MeetupEventUrl: null,
            ExternalRegistrationUrl: null);

        var result = _eventValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateEventDto.EndDateUtc));
    }

    [Fact]
    public void CreateEventValidator_ShouldFail_WhenVenueDetailsAreMissing()
    {
        var start = DateTimeOffset.UtcNow.AddDays(2);
        var dto = new CreateEventDto(
            Title: "Mekansız Etkinlik",
            Slug: "mekansiz-etkinlik",
            TldrSummary: "Mekan bilgisi eksik olan etkinlik özeti burada.",
            StartDateUtc: start,
            EndDateUtc: start.AddHours(5),
            VenueName: "", // Boş!
            VenueAddress: "", // Boş!
            MeetupEventUrl: null,
            ExternalRegistrationUrl: null);

        var result = _eventValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateEventDto.VenueName));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateEventDto.VenueAddress));
    }

    #endregion
}

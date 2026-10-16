using FluentAssertions;
using Masabasi.Business.DTOs.Articles;
using Masabasi.Business.DTOs.Events;
using Masabasi.Business.Validators;
using Xunit;

namespace Masabasi.UnitTests.Business;

/// <summary>
/// FluentValidation doğrulayıcılarının kurallarını (iletişim bilgisi, SEO uzunlukları, TL;DR kelime sayısı) test eden sınıf.
/// </summary>
public sealed class ValidatorTests
{
    private readonly CreateReservationValidator _reservationValidator = new();
    private readonly CreateArticleValidator _articleValidator = new();

    [Theory]
    [InlineData("@kullanici", true)]
    [InlineData("05321234567", true)]
    [InlineData("+905321234567", true)]
    [InlineData("a", false)] // Çok kısa / geçersiz
    [InlineData("", false)]
    public void CreateReservationValidator_ShouldValidateContactHandleProperly(string contactHandle, bool expectedValid)
    {
        // Arrange
        var dto = new CreateReservationDto(
            GameTableId: Guid.NewGuid(),
            PlayerName: "Test Oyuncu",
            ContactHandle: contactHandle,
            ExperienceLevel: "Beginner",
            Notes: null);

        // Act
        var result = _reservationValidator.Validate(dto);

        // Assert
        result.IsValid.Should().Be(expectedValid);
    }

    [Fact]
    public void CreateReservationValidator_ShouldFail_WhenExperienceLevelIsInvalid()
    {
        // Arrange
        var dto = new CreateReservationDto(
            GameTableId: Guid.NewGuid(),
            PlayerName: "Test Oyuncu",
            ContactHandle: "@oyuncu",
            ExperienceLevel: "MasterOfTheUniverse", // Geçersiz!
            Notes: null);

        // Act
        var result = _reservationValidator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateReservationDto.ExperienceLevel));
    }

    [Fact]
    public void CreateArticleValidator_ShouldFail_WhenTldrWordCountIsTooShort()
    {
        // Arrange
        var dto = new CreateArticleDto(
            Title: "D&D 5e Başlangıç Kılavuzu: Zarlar ve Masalar",
            Slug: "dnd-5e-baslangic-kilavuzu",
            TopicCluster: "FrpDnD",
            TldrSummary: "Çok kısa özet.", // Sadece 3 kelime (en az 20 kelime olmalı)
            MetaDescription: "D&D 5e başlangıç rehberi hakkında detaylı bilgiler ve ipuçları burada yer almaktadır.",
            ContentMarkdown: "## Giriş\nDetaylı içerik metni burada yer almaktadır...",
            AuthorName: "Yazar",
            FaqJson: null,
            CoverImageUrl: null);

        // Act
        var result = _articleValidator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateArticleDto.TldrSummary));
    }
}

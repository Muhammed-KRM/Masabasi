using FluentAssertions;
using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Events;
using Xunit;

namespace Masabasi.UnitTests.Business;

/// <summary>
/// ServiceResult, PagedResult ve DTO hesaplanan özelliklerini (RemainingSeats, IsFull) test eden sınıf.
/// </summary>
public sealed class CommonResultTests
{
    [Fact]
    public void ServiceResult_Success_ShouldSetPropertiesProperly()
    {
        var result = ServiceResult<string>.Success("İşlem Başarılı", 200);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be("İşlem Başarılı");
        result.StatusCode.Should().Be(200);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ServiceResult_Failure_ShouldSetErrorAndStatusCode()
    {
        var result = ServiceResult<string>.Failure("Kayıt bulunamadı", 404);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(404);
        result.ErrorMessage.Should().Be("Kayıt bulunamadı");
    }

    [Theory]
    [InlineData(25, 10, 3, 1, true, false)]  // 25 kayıt, sayfa 1/3 -> sonraki var, önceki yok
    [InlineData(25, 10, 3, 2, true, true)]   // 25 kayıt, sayfa 2/3 -> sonraki var, önceki var
    [InlineData(25, 10, 3, 3, false, true)]  // 25 kayıt, sayfa 3/3 -> sonraki yok, önceki var
    [InlineData(20, 10, 2, 2, false, true)]  // 20 kayıt tam bölünüyor
    [InlineData(0, 10, 0, 1, false, false)]  // 0 kayıt
    public void PagedResult_ShouldCalculatePaginationMetricsAccurately(
        int totalCount, int pageSize, int expectedPages, int currentPage, bool expectedNext, bool expectedPrev)
    {
        var items = new List<string>();
        var paged = new PagedResult<string>(items, totalCount, currentPage, pageSize);

        paged.TotalPages.Should().Be(expectedPages);
        paged.HasNextPage.Should().Be(expectedNext);
        paged.HasPreviousPage.Should().Be(expectedPrev);
    }

    [Theory]
    [InlineData(5, 3, 2, false)] // 5 kapasite, 3 dolu -> 2 boş, dolu değil
    [InlineData(5, 5, 0, true)]  // 5 kapasite, 5 dolu -> 0 boş, dolu
    [InlineData(5, 6, 0, true)]  // 5 kapasite, 6 dolu -> 0 boş (asla eksi olmamalı), dolu
    [InlineData(10, 0, 10, false)]// 10 kapasite, 0 dolu -> 10 boş, dolu değil
    public void GameTableDto_ComputedProperties_ShouldCalculateRemainingSeatsAndIsFull(
        int maxPlayers, int currentPlayers, int expectedRemaining, bool expectedIsFull)
    {
        var dto = new GameTableDto(
            Id: Guid.NewGuid(),
            TableNumber: 1,
            GameTitle: "Secret Hitler",
            Category: "SocialDeduction",
            HostOrDmName: "DM",
            MaxPlayers: maxPlayers,
            CurrentPlayers: currentPlayers,
            IsBeginnerFriendly: true);

        dto.RemainingSeats.Should().Be(expectedRemaining);
        dto.IsFull.Should().Be(expectedIsFull);
    }
}

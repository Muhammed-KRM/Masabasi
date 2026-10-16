using Masabasi.Business.Common;
using Masabasi.Business.DTOs.Games;
using Masabasi.Business.Interfaces;
using Masabasi.Data.Context;
using Masabasi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masabasi.Business.Services;

/// <summary>
/// Kulüp envanterindeki kutu oyunları, FRP sistemleri ve kural rehberlerinin iş mantığını yöneten servis.
/// </summary>
public sealed class GameCatalogManager : IGameCatalogService
{
    private readonly MasabasiDbContext _dbContext;
    private readonly IStaticSnapshotService _snapshotService;
    private readonly ILogger<GameCatalogManager> _logger;

    public GameCatalogManager(
        MasabasiDbContext dbContext,
        IStaticSnapshotService snapshotService,
        ILogger<GameCatalogManager> logger)
    {
        _dbContext = dbContext;
        _snapshotService = snapshotService;
        _logger = logger;
    }

    public async Task<ServiceResult<IReadOnlyList<BoardGameDto>>> GetGamesAsync(
        GameFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.BoardGames
            .AsNoTracking()
            .Where(g => !g.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(g => g.Category.ToLower() == filter.Category.Trim().ToLower());
        }

        if (filter.PlayerCount.HasValue)
        {
            query = query.Where(g => g.MinPlayers <= filter.PlayerCount.Value && g.MaxPlayers >= filter.PlayerCount.Value);
        }

        if (filter.MaxComplexity.HasValue)
        {
            query = query.Where(g => g.ComplexityRating <= filter.MaxComplexity.Value);
        }

        if (filter.OnlyInInventory is true)
        {
            query = query.Where(g => g.IsInClubInventory);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            string term = filter.SearchTerm.Trim().ToLower();
            query = query.Where(g => g.Title.ToLower().Contains(term) || g.TldrSummary.ToLower().Contains(term));
        }

        var list = await query
            .OrderBy(g => g.Title)
            .Select(g => new BoardGameDto(
                g.Id,
                g.Title,
                g.Slug,
                g.Category,
                g.MinPlayers,
                g.MaxPlayers,
                g.RecommendedPlayerCount,
                g.PlayTimeMinutes,
                g.ComplexityRating,
                g.TldrSummary,
                g.RulesMarkdown,
                g.ImageUrl,
                g.IsInClubInventory,
                g.YearPublished))
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<BoardGameDto>>.Success(list);
    }

    public async Task<ServiceResult<BoardGameDto>> GetGameBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var game = await _dbContext.BoardGames
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Slug == slug && !g.IsDeleted, cancellationToken);

        if (game is null)
            return ServiceResult<BoardGameDto>.Failure("Oyun kütüphanede bulunamadı.", 404);

        var dto = new BoardGameDto(
            game.Id,
            game.Title,
            game.Slug,
            game.Category,
            game.MinPlayers,
            game.MaxPlayers,
            game.RecommendedPlayerCount,
            game.PlayTimeMinutes,
            game.ComplexityRating,
            game.TldrSummary,
            game.RulesMarkdown,
            game.ImageUrl,
            game.IsInClubInventory,
            game.YearPublished);

        return ServiceResult<BoardGameDto>.Success(dto);
    }

    public async Task<ServiceResult<Guid>> CreateGameAsync(
        CreateBoardGameDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Slug))
            return ServiceResult<Guid>.Failure("Oyun başlığı ve SEO kısa adı zorunludur.", 400);

        bool slugExists = await _dbContext.BoardGames.AnyAsync(g => g.Slug == request.Slug, cancellationToken);
        if (slugExists)
            return ServiceResult<Guid>.Failure("Bu SEO kısa adına sahip bir oyun zaten mevcut.", 409);

        var game = new BoardGame
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Slug = request.Slug.Trim().ToLowerInvariant(),
            Category = request.Category.Trim(),
            MinPlayers = request.MinPlayers,
            MaxPlayers = request.MaxPlayers,
            RecommendedPlayerCount = request.RecommendedPlayerCount?.Trim(),
            PlayTimeMinutes = request.PlayTimeMinutes,
            ComplexityRating = request.ComplexityRating,
            TldrSummary = request.TldrSummary.Trim(),
            RulesMarkdown = request.RulesMarkdown.Trim(),
            ImageUrl = request.ImageUrl?.Trim(),
            IsInClubInventory = request.IsInClubInventory,
            YearPublished = request.YearPublished,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _dbContext.BoardGames.Add(game);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _snapshotService.ExportGamesSnapshotAsync(cancellationToken);
        await _snapshotService.ExportLlmsTxtAsync(cancellationToken);
        await _snapshotService.ExportSitemapXmlAsync(cancellationToken);

        _logger.LogInformation("Yeni oyun kütüphaneye eklendi: {Title} ({Id})", game.Title, game.Id);
        return ServiceResult<Guid>.Success(game.Id, 201);
    }

    public async Task<ServiceResult<bool>> UpdateGameAsync(
        Guid id,
        CreateBoardGameDto request,
        CancellationToken cancellationToken = default)
    {
        var game = await _dbContext.BoardGames.FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);
        if (game is null)
            return ServiceResult<bool>.Failure("Güncellenecek oyun bulunamadı.", 404);

        game.Title = request.Title.Trim();
        game.Category = request.Category.Trim();
        game.MinPlayers = request.MinPlayers;
        game.MaxPlayers = request.MaxPlayers;
        game.RecommendedPlayerCount = request.RecommendedPlayerCount?.Trim();
        game.PlayTimeMinutes = request.PlayTimeMinutes;
        game.ComplexityRating = request.ComplexityRating;
        game.TldrSummary = request.TldrSummary.Trim();
        game.RulesMarkdown = request.RulesMarkdown.Trim();
        game.ImageUrl = request.ImageUrl?.Trim();
        game.IsInClubInventory = request.IsInClubInventory;
        game.YearPublished = request.YearPublished;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _snapshotService.ExportGamesSnapshotAsync(cancellationToken);
        await _snapshotService.ExportSitemapXmlAsync(cancellationToken);

        _logger.LogInformation("Oyun bilgileri güncellendi: {Title}", game.Title);
        return ServiceResult<bool>.Success(true);
    }
}

using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Manga> Mangas { get; set; }
    public DbSet<MangaSource> MangaSources { get; set; }
    public DbSet<Source> Sources { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        builder.Entity<Source>().HasData(
            new Source { Id = 1, Name = "MangaDex", BaseUrl = "https://api.mangadex.org/manga/" },
            new Source { Id = 2, Name = "AsuraScans", BaseUrl = "https://asuracomic.net/series/" }
        );

        builder.Entity<Manga>().HasData(
            new Manga { Id = 1, MyAnimeListId = 147324, AniListId = 109957, TitleRomaji = "Dubeon Saneun Ranker", TitleEnglish = "Second Life Ranker" },
            new Manga { Id = 2, MyAnimeListId = 13, AniListId = 30013, TitleRomaji = "One Piece", TitleEnglish = "One Piece" },
            new Manga { Id = 3, MyAnimeListId = 157888, AniListId = 163824, TitleRomaji = "Cheolhyeolgeomga Sanyanggaeui Hoegwi", TitleEnglish = "Revenge of the Baskerville Bloodhound" }
        );
        
        builder.Entity<MangaSource>().HasData(
            new MangaSource { Id = 1, MangaId = 1, SourceId = 1, Url = "1ffca916-3ad7-46d2-9591-a9b39e639971" },
            new MangaSource { Id = 3, MangaId = 2, SourceId = 1, Url = "a1c7c817-4e59-43b7-9365-09675a149a6f" },
            new MangaSource { Id = 4, MangaId = 3, SourceId = 2, Url = "revenge-of-the-iron-blooded-sword-hound-da0c5e71" }

        );
    }
}
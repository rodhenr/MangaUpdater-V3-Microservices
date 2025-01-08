using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<LogEvent> LogEvents { get; set; }
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
            new Manga { Id = 2, MyAnimeListId = 127781, AniListId = 121753, TitleRomaji = "Tensei Kizoku, Kantei Skill de Nariagaru: Jakushou Ryouchi wo Uketsuida node, Yuushuu na Jinzai wo Fuyashiteitara, Saikyou Ryouchi ni Natteta", TitleEnglish = "As a Reincarnated Aristocrat, I’ll Use My Appraisal Skill to Rise in the World" },
            new Manga { Id = 3, MyAnimeListId = 123456, AniListId = 114048, TitleRomaji = "Shinmai Ossan Bouken-sha, Saikyou Party ni Shinu Hodo Kitaerarete Muteki ni Naru.", TitleEnglish = "The Ossan Newbie Adventurer, Trained to Death by the Most Powerful Party, Became Invincible" },
            new Manga { Id = 4, MyAnimeListId = 111466, AniListId = 101715, TitleRomaji = "Rougo ni Sonaete Isekai de 8-manmai no Kinka wo Tamemasu", TitleEnglish = "Saving 80,000 Gold in Another World for my Retirement" },
            new Manga { Id = 5, MyAnimeListId = 157888, AniListId = 163824, TitleRomaji = "Cheolhyeolgeomga Sanyanggaeui Hoegwi", TitleEnglish = "Revenge of the Baskerville Bloodhound" }
        );
        
        builder.Entity<MangaSource>().HasData(
            new MangaSource { Id = 1, MangaId = 1, SourceId = 1, Url = "1ffca916-3ad7-46d2-9591-a9b39e639971" },
            new MangaSource { Id = 2, MangaId = 2, SourceId = 1, Url = "fef2e4da-36f9-48e9-8317-2516f4b6ab14" },
            new MangaSource { Id = 3, MangaId = 3, SourceId = 1, Url = "a2320293-f00e-43a0-8d08-1110cf26a894" },
            new MangaSource { Id = 4, MangaId = 4, SourceId = 1, Url = "89ed3ec2-ebe6-4d6b-92eb-d753a8bb365e" },
            new MangaSource { Id = 5, MangaId = 5, SourceId = 2, Url = "revenge-of-the-iron-blooded-sword-hound-da0c5e71" }
        );
    }
}
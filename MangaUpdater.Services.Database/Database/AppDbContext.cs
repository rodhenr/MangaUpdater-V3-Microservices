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
            new Manga { Id = 5, MyAnimeListId = 157888, AniListId = 163824, TitleRomaji = "Cheolhyeolgeomga Sanyanggaeui Hoegwi", TitleEnglish = "Revenge of the Baskerville Bloodhound" },
            new Manga { Id = 6, MyAnimeListId = 146949, AniListId = 149332, TitleRomaji = "Geomsul Myeongga Mangnaeadeul", TitleEnglish = "The Swordmaster's Son" },
            new Manga { Id = 7, MyAnimeListId = 151483, AniListId = 153883, TitleRomaji = "SSS-geup Ranker Hoegwihada", TitleEnglish = "The SSS-Ranker Returns" },
            new Manga { Id = 8, MyAnimeListId = 147322, AniListId = 125636, TitleRomaji = "Man-Level Yeongung-nim-kkeseo Gwihwan Hasinda!", TitleEnglish = "The Max Level Hero Strikes Back!" },
            new Manga { Id = 9, MyAnimeListId = 150561, AniListId = 151025, TitleRomaji = "Sinhwa-geup Gwisok Item-eul Son-e Neoeotda", TitleEnglish = "Mythic Item Obtained" },
            new Manga { Id = 10, MyAnimeListId = 154587, AniListId = 159441, TitleRomaji = "Pick Me Up!, Infinite Gacha", TitleEnglish = "Pick Me Up" },
            new Manga { Id = 11, MyAnimeListId = 159916, AniListId = 167318, TitleRomaji = "Extra-ga Neomu Gangham", TitleEnglish = "The Extra Is Too Powerful" },
            new Manga { Id = 12, MyAnimeListId = 160118, AniListId = 166635, TitleRomaji = "Absolute Necromancer", TitleEnglish = "All-Master Necromancer" },
            new Manga { Id = 13, MyAnimeListId = 147995, AniListId = 130511, TitleRomaji = "Level Up Mothaneun Player", TitleEnglish = "The Player Who Can't Level Up" },
            new Manga { Id = 14, MyAnimeListId = 147392, AniListId = 137280, TitleRomaji = "Na Honja Man-Level Newbie", TitleEnglish = "I'm the Max-Level Newbie" },
            new Manga { Id = 15, MyAnimeListId = 122650, AniListId = 110989, TitleRomaji = "Hazurewaku no \"Joutai Ijou Skill\" de Saikyou ni Natta Ore ga Subete wo Juurin suru made", TitleEnglish = "Failure Frame: I Became the Strongest and Annihilated Everything With Low-Level Spells" },
            new Manga { Id = 16, MyAnimeListId = 90125, AniListId = 86635, TitleRomaji = "Kaguya-sama wa Kokurasetai: Tensai-tachi no Renai Zunousen", TitleEnglish = "Kaguya-sama: Love Is War" }
        );
        
        builder.Entity<MangaSource>().HasData(
            new MangaSource { Id = 1, MangaId = 1, SourceId = 1, Url = "1ffca916-3ad7-46d2-9591-a9b39e639971" },
            new MangaSource { Id = 2, MangaId = 2, SourceId = 1, Url = "fef2e4da-36f9-48e9-8317-2516f4b6ab14" },
            new MangaSource { Id = 3, MangaId = 3, SourceId = 1, Url = "a2320293-f00e-43a0-8d08-1110cf26a894" },
            new MangaSource { Id = 4, MangaId = 4, SourceId = 1, Url = "89ed3ec2-ebe6-4d6b-92eb-d753a8bb365e" },
            new MangaSource { Id = 5, MangaId = 5, SourceId = 2, Url = "revenge-of-the-iron-blooded-sword-hound-da0c5e71" },
            new MangaSource { Id = 6, MangaId = 6, SourceId = 2, Url = "swordmasters-youngest-son-e6946e27" },
            new MangaSource { Id = 7, MangaId = 7, SourceId = 2, Url = "return-of-the-sss-class-ranker-f6fde482" },
            new MangaSource { Id = 8, MangaId = 8, SourceId = 2, Url = "the-max-level-hero-has-returned-cc806d84" },
            new MangaSource { Id = 9, MangaId = 9, SourceId = 2, Url = "i-obtained-a-mythic-item-5c23ef60" },
            new MangaSource { Id = 10, MangaId = 10, SourceId = 2, Url = "pick-me-up-infinite-gacha-e764ac18" },
            new MangaSource { Id = 11, MangaId = 11, SourceId = 2, Url = "the-extra-is-too-strong-ac4babd7" },
            new MangaSource { Id = 12, MangaId = 12, SourceId = 2, Url = "absolute-necromancer-f3d79560" },
            new MangaSource { Id = 13, MangaId = 13, SourceId = 2, Url = "player-who-cant-level-up-6937decb" },
            new MangaSource { Id = 14, MangaId = 14, SourceId = 2, Url = "solo-max-level-newbie-6fb35ee2" },
            new MangaSource { Id = 15, MangaId = 15, SourceId = 1, Url = "0b171f64-89a5-4c37-b5f9-75cca57e8787" },
            new MangaSource { Id = 16, MangaId = 16, SourceId = 1, Url = "37f5cce0-8070-4ada-96e5-fa24b1bd4ff9" }
        );
    }
}
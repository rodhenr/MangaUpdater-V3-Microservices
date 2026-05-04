using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<LogEvent> LogEvents { get; set; }
    public DbSet<Manga> Mangas { get; set; }
    public DbSet<MangaSource> MangaSources { get; set; }
    public DbSet<SourceApiProfile> SourceApiProfiles { get; set; }
    public DbSet<SourceRequestProfile> SourceRequestProfiles { get; set; }
    public DbSet<SourceScrapingProfile> SourceScrapingProfiles { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        builder.Entity<Source>().HasData(
            new Source
            {
                Id = 1,
                Name = "MangaDex",
                Slug = "mangadex",
                BaseUrl = "https://api.mangadex.org/manga/",
                IsEnabled = true,
                EngineType = "JsonApi",
                RequestMode = "HttpGet",
                RequiresBrowser = false,
                DefaultUserAgent = "MangaUpdater/1.0",
                QueueName = "get-chapters-Mangadex",
                SupportsPagination = true
            },
            new Source
            {
                Id = 2,
                Name = "AsuraScans",
                Slug = "asurascans",
                BaseUrl = "https://asurascans.com",
                IsEnabled = true,
                EngineType = "HtmlXPath",
                RequestMode = "HttpGet",
                RequiresBrowser = false,
                QueueName = "get-chapters-AsuraScans",
                SupportsPagination = false
            },
            new Source
            {
                Id = 3,
                Name = "VortexScans",
                Slug = "vortexscans",
                BaseUrl = "https://vortexscans.org/api/chapters?postId=",
                IsEnabled = true,
                EngineType = "JsonApi",
                RequestMode = "HttpGet",
                RequiresBrowser = false,
                QueueName = "get-chapters-VortexScans",
                SupportsPagination = true
            }
        );

        builder.Entity<SourceRequestProfile>().HasData(
            new SourceRequestProfile
            {
                Id = 1,
                SourceId = 1,
                IsActive = true,
                Version = 1,
                Method = "GET",
                UrlTemplate = "{BaseUrl}{MangaUrlPart}/feed?translatedLanguage[]=en&limit=199&order[chapter]=asc&limit=200&offset={Offset}",
                HeadersJson = "{\"User-Agent\":\"MangaUpdater/1.0\"}",
                TimeoutSeconds = 30,
                UseCookies = false,
                AcceptLanguage = "en-US"
            },
            new SourceRequestProfile
            {
                Id = 2,
                SourceId = 2,
                IsActive = true,
                Version = 1,
                Method = "GET",
                UrlTemplate = "{BaseUrl}{MangaUrlPart}",
                TimeoutSeconds = 30,
                UseCookies = false
            },
            new SourceRequestProfile
            {
                Id = 3,
                SourceId = 3,
                IsActive = true,
                Version = 1,
                Method = "GET",
                UrlTemplate = "{BaseUrl}{MangaUrlPart}&skip={Offset}&take=200&order=desc",
                TimeoutSeconds = 30,
                UseCookies = false
            }
        );

        builder.Entity<SourceScrapingProfile>().HasData(
            new SourceScrapingProfile
            {
                Id = 1,
                SourceId = 2,
                IsActive = true,
                Version = 1,
                ChapterNodesXPath = "//a[contains(@href, '/chapter/')]",
                ChapterUrlXPath = ".",
                ChapterUrlAttribute = "href",
                ChapterNumberAttribute = "href",
                ChapterNumberRegex = "chapter/(\\d+(\\.\\d+)?)",
                ChapterDateXPath = ".",
                ChapterDateRegex = "((?:(?:\\d+|a|an|one)\\s+(?:second|sec|minute|min|hour|hr|day|week|wk|month|mo|year|yr)(?:s)?\\s+ago)|today|yesterday|last\\s+(?:week|month|year)|(?:[A-Za-z]{3}\\s+\\d{1,2},\\s+\\d{4}))",
                DateParseMode = "RelativeOrFormat",
                DateCulture = "en-US",
                DateFormatPrimary = "MMM dd, yyyy",
                RelativeDateRegex = "((?:\\d+|a|an|one))\\s+(second|sec|minute|min|hour|hr|day|week|wk|month|mo|year|yr)s?\\s+ago",
                IgnoreTextContains1 = "First Chapter",
                IgnoreTextContains2 = "Latest Chapter",
                UrlJoinMode = "BaseUrlPrefix",
                DeduplicationKeyMode = "ChapterNumber",
                ChapterSortMode = "NumericAscending",
                ResultLimit = 500
            }
        );

        builder.Entity<SourceApiProfile>().HasData(
            new SourceApiProfile
            {
                Id = 1,
                SourceId = 1,
                IsActive = true,
                Version = 1,
                EndpointTemplate = "{BaseUrl}{MangaUrlPart}/feed?translatedLanguage[]=en&limit=199&order[chapter]=asc&limit=200&offset={Offset}",
                HttpMethod = "GET",
                DataRootPath = "data",
                ChapterNumberPath = "attributes.chapter",
                ChapterDatePath = "attributes.createdAt",
                ChapterUrlPath = "id",
                ResultUrlTemplate = "https://mangadex.org/chapter/{Value}",
                PaginationMode = "Offset",
                OffsetParameterName = "offset",
                ResultLimit = 200
            },
            new SourceApiProfile
            {
                Id = 2,
                SourceId = 3,
                IsActive = true,
                Version = 1,
                EndpointTemplate = "{BaseUrl}{MangaUrlPart}&skip={Offset}&take=200&order=desc",
                HttpMethod = "GET",
                DataRootPath = "post.chapters",
                ChapterNumberPath = "number",
                ChapterDatePath = "createdAt",
                PaginationMode = "Offset",
                OffsetParameterName = "skip",
                LimitParameterName = "take",
                ResultLimit = 200
            }
        );

        builder.Entity<Manga>().HasData(
            new Manga { Id = 1, MyAnimeListId = 147324, AniListId = 109957, TitleRomaji = "Dubeon Saneun Ranker", TitleEnglish = "Second Life Ranker", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx109957-EgJWdR7l9TBG.jpg" },
            new Manga { Id = 2, MyAnimeListId = 127781, AniListId = 121753, TitleRomaji = "Tensei Kizoku, Kantei Skill de Nariagaru: Jakushou Ryouchi wo Uketsuida node, Yuushuu na Jinzai wo Fuyashiteitara, Saikyou Ryouchi ni Natteta", TitleEnglish = "As a Reincarnated Aristocrat, I’ll Use My Appraisal Skill to Rise in the World", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx121753-vhvIdfxdaEdF.jpg" },
            new Manga { Id = 3, MyAnimeListId = 123456, AniListId = 114048, TitleRomaji = "Shinmai Ossan Bouken-sha, Saikyou Party ni Shinu Hodo Kitaerarete Muteki ni Naru.", TitleEnglish = "The Ossan Newbie Adventurer, Trained to Death by the Most Powerful Party, Became Invincible", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx114048-4HEtdYDcXI8r.jpg" },
            new Manga { Id = 4, MyAnimeListId = 111466, AniListId = 101715, TitleRomaji = "Rougo ni Sonaete Isekai de 8-manmai no Kinka wo Tamemasu", TitleEnglish = "Saving 80,000 Gold in Another World for my Retirement", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/nx101715-4yYFDOadUtnC.jpg" },
            new Manga { Id = 5, MyAnimeListId = 157888, AniListId = 163824, TitleRomaji = "Cheolhyeolgeomga Sanyanggaeui Hoegwi", TitleEnglish = "Revenge of the Baskerville Bloodhound", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx163824-KiablxybJD6i.jpg" },
            new Manga { Id = 6, MyAnimeListId = 146949, AniListId = 149332, TitleRomaji = "Geomsul Myeongga Mangnaeadeul", TitleEnglish = "The Swordmaster's Son", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx149332-adkSyOFY3c5U.jpg" },
            new Manga { Id = 7, MyAnimeListId = 151483, AniListId = 153883, TitleRomaji = "SSS-geup Ranker Hoegwihada", TitleEnglish = "The SSS-Ranker Returns", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx153883-thHiGEnqxFoB.jpg" },
            new Manga { Id = 8, MyAnimeListId = 147322, AniListId = 125636, TitleRomaji = "Man-Level Yeongung-nim-kkeseo Gwihwan Hasinda!", TitleEnglish = "The Max Level Hero Strikes Back!", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx125636-g0gkyLZbo3Tz.png" },
            new Manga { Id = 9, MyAnimeListId = 150561, AniListId = 151025, TitleRomaji = "Sinhwa-geup Gwisok Item-eul Son-e Neoeotda", TitleEnglish = "Mythic Item Obtained", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx151025-j7nZBNb46cv9.jpg" },
            new Manga { Id = 10, MyAnimeListId = 154587, AniListId = 159441, TitleRomaji = "Pick Me Up!, Infinite Gacha", TitleEnglish = "Pick Me Up", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx159441-n919hUzb0j44.jpg" },
            new Manga { Id = 11, MyAnimeListId = 159916, AniListId = 167318, TitleRomaji = "Extra-ga Neomu Gangham", TitleEnglish = "The Extra Is Too Powerful", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx167318-fmcRXTsFE99i.jpg" },
            new Manga { Id = 12, MyAnimeListId = 160118, AniListId = 166635, TitleRomaji = "Absolute Necromancer", TitleEnglish = "All-Master Necromancer", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx166635-6Y7R6AZe52Fv.jpg" },
            new Manga { Id = 13, MyAnimeListId = 147995, AniListId = 130511, TitleRomaji = "Level Up Mothaneun Player", TitleEnglish = "The Player Who Can't Level Up", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx130511-4O6dF8oaiVJh.jpg" },
            new Manga { Id = 14, MyAnimeListId = 147392, AniListId = 137280, TitleRomaji = "Na Honja Man-Level Newbie", TitleEnglish = "I'm the Max-Level Newbie", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx137280-C8kbBitLxlwR.png" },
            new Manga { Id = 15, MyAnimeListId = 122650, AniListId = 110989, TitleRomaji = "Hazurewaku no \"Joutai Ijou Skill\" de Saikyou ni Natta Ore ga Subete wo Juurin suru made", TitleEnglish = "Failure Frame: I Became the Strongest and Annihilated Everything With Low-Level Spells", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx110989-DKLKwQ5ojqXD.jpg" },
            new Manga { Id = 16, MyAnimeListId = 90125, AniListId = 86635, TitleRomaji = "Kaguya-sama wa Kokurasetai: Tensai-tachi no Renai Zunousen", TitleEnglish = "Kaguya-sama: Love Is War", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx86635-EdaLQmsn86Fy.png" },
            new Manga { Id = 17, MyAnimeListId = 148458, AniListId = 151457, TitleRomaji = "Newbie-ga Neomu Gangham", TitleEnglish = "The Overpowered Newbie", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx151457-7v5jKk5yAnAc.png" },
            new Manga { Id = 18, MyAnimeListId = 130331, AniListId = 122063, TitleRomaji = "Shangri-La Frontier: Kusoge Hunter, Kamige ni Idoman to su", TitleEnglish = "Shangri-La Frontier", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx122063-zq7rF3cdgxpX.jpg" },
            new Manga { Id = 19, MyAnimeListId = 13, AniListId = 30013, TitleRomaji = "One Piece", TitleEnglish = "One Piece", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx30013-ulXvn0lzWvsz.jpg" },
            new Manga { Id = 20, MyAnimeListId = 147450, AniListId = 139572, TitleRomaji = "Na Honja Necromancer", TitleEnglish = "The Lone Necromancer", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx139572-e3vwLcOVQISn.jpg" },
            new Manga { Id = 21, MyAnimeListId = 111225, AniListId = 100693, TitleRomaji = "Nozomanu Fushi no Boukensha", TitleEnglish = "The Unwanted Undead Adventurer", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/nx100693-SwgtbDgQosE7.jpg" },
            new Manga { Id = 22, MyAnimeListId = 150210, AniListId = 153284, TitleRomaji = "Man Nyeon Man-e Gwihwanhan Player", TitleEnglish = "After Ten Millennia in Hell", CoverUrl = "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx153284-roAlRmlRM7Vs.png" }
        );
        
        builder.Entity<MangaSource>().HasData(
            new MangaSource { Id = 2, MangaId = 2, SourceId = 1, Url = "fef2e4da-36f9-48e9-8317-2516f4b6ab14" },
            new MangaSource { Id = 3, MangaId = 3, SourceId = 1, Url = "a2320293-f00e-43a0-8d08-1110cf26a894" },
            new MangaSource { Id = 4, MangaId = 4, SourceId = 1, Url = "89ed3ec2-ebe6-4d6b-92eb-d753a8bb365e" },
            new MangaSource { Id = 5, MangaId = 5, SourceId = 2, Url = "/comics/revenge-of-the-iron-blooded-sword-hound-5abb513e" },
            new MangaSource { Id = 6, MangaId = 6, SourceId = 2, Url = "/comics/swordmasters-youngest-son-5abb513e" },
            new MangaSource { Id = 7, MangaId = 7, SourceId = 2, Url = "/comics/return-of-the-sss-class-ranker-5abb513e" },
            new MangaSource { Id = 8, MangaId = 8, SourceId = 2, Url = "/comics/the-max-level-hero-has-returned-5abb513e" },
            new MangaSource { Id = 9, MangaId = 9, SourceId = 2, Url = "/comics/i-obtained-a-mythic-item-5abb513e" },
            new MangaSource { Id = 10, MangaId = 10, SourceId = 2, Url = "/comics/pick-me-up-infinite-gacha-5abb513e" },
            new MangaSource { Id = 12, MangaId = 12, SourceId = 2, Url = "/comics/absolute-necromancer-5abb513e" },
            new MangaSource { Id = 13, MangaId = 13, SourceId = 2, Url = "/comics/player-who-cant-level-up-5abb513e" },
            new MangaSource { Id = 14, MangaId = 14, SourceId = 2, Url = "/comics/solo-max-level-newbie-5abb513e" },
            new MangaSource { Id = 15, MangaId = 15, SourceId = 1, Url = "0b171f64-89a5-4c37-b5f9-75cca57e8787" },
            new MangaSource { Id = 16, MangaId = 16, SourceId = 1, Url = "37f5cce0-8070-4ada-96e5-fa24b1bd4ff9" },
            new MangaSource { Id = 17, MangaId = 17, SourceId = 3, Url = "214" },
            new MangaSource { Id = 21, MangaId = 21, SourceId = 1, Url = "6e44705b-9f80-42f6-9ebb-1141fbe8320e" }
        );
    }
}
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Module = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Exception = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mangas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MyAnimeListId = table.Column<int>(type: "integer", nullable: false),
                    AniListId = table.Column<int>(type: "integer", nullable: false),
                    TitleRomaji = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TitleEnglish = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CoverUrl = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mangas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BaseUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    EngineType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "HtmlXPath"),
                    RequestMode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "HttpGet"),
                    RequiresBrowser = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DefaultUserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RateLimitMilliseconds = table.Column<int>(type: "integer", nullable: true),
                    QueueName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    SupportsPagination = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "user"),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MangaId = table.Column<int>(type: "integer", nullable: false),
                    SourceId = table.Column<int>(type: "integer", nullable: false),
                    OriginalNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NumberMajor = table.Column<int>(type: "integer", nullable: false),
                    NumberMinor = table.Column<int>(type: "integer", nullable: false),
                    NumberSuffix = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Url = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapters_Mangas_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Mangas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Chapters_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MangaSources",
                columns: table => new
                {
                    MangaId = table.Column<int>(type: "integer", nullable: false),
                    SourceId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AdditionalInfo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaSources", x => new { x.MangaId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_MangaSources_Mangas_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Mangas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MangaSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SourceApiProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    EndpointTemplate = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    HttpMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "GET"),
                    DataRootPath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChapterNumberPath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChapterDatePath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChapterUrlPath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ResultUrlTemplate = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PaginationMode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OffsetParameterName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LimitParameterName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NextPagePath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ResultLimit = table.Column<int>(type: "integer", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceApiProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceApiProfiles_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SourceRequestProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "GET"),
                    UrlTemplate = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    HeadersJson = table.Column<string>(type: "text", nullable: true),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: true),
                    UseCookies = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptLanguage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Referrer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceRequestProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceRequestProfiles_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SourceScrapingProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ChapterNodesXPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ChapterUrlXPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ChapterUrlAttribute = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ChapterNumberXPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ChapterNumberAttribute = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ChapterNumberRegex = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ChapterDateXPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ChapterDateAttribute = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ChapterDateRegex = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DateParseMode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DateCulture = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DateFormatPrimary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DateFormatSecondary = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelativeDateRegex = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IgnoreTextContains1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IgnoreTextContains2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IgnoreTextContains3 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UrlPrefix = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UrlJoinMode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DeduplicationKeyMode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ChapterSortMode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaginationNextPageXPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PaginationUrlTemplate = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ResultLimit = table.Column<int>(type: "integer", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceScrapingProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceScrapingProfiles_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Mangas",
                columns: new[] { "Id", "AniListId", "CoverUrl", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[,]
                {
                    { 1, 109957, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx109957-EgJWdR7l9TBG.jpg", 147324, "Second Life Ranker", "Dubeon Saneun Ranker" },
                    { 2, 121753, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx121753-vhvIdfxdaEdF.jpg", 127781, "As a Reincarnated Aristocrat, I’ll Use My Appraisal Skill to Rise in the World", "Tensei Kizoku, Kantei Skill de Nariagaru: Jakushou Ryouchi wo Uketsuida node, Yuushuu na Jinzai wo Fuyashiteitara, Saikyou Ryouchi ni Natteta" },
                    { 3, 114048, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx114048-4HEtdYDcXI8r.jpg", 123456, "The Ossan Newbie Adventurer, Trained to Death by the Most Powerful Party, Became Invincible", "Shinmai Ossan Bouken-sha, Saikyou Party ni Shinu Hodo Kitaerarete Muteki ni Naru." },
                    { 4, 101715, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/nx101715-4yYFDOadUtnC.jpg", 111466, "Saving 80,000 Gold in Another World for my Retirement", "Rougo ni Sonaete Isekai de 8-manmai no Kinka wo Tamemasu" },
                    { 5, 163824, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx163824-KiablxybJD6i.jpg", 157888, "Revenge of the Baskerville Bloodhound", "Cheolhyeolgeomga Sanyanggaeui Hoegwi" },
                    { 6, 149332, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx149332-adkSyOFY3c5U.jpg", 146949, "The Swordmaster's Son", "Geomsul Myeongga Mangnaeadeul" },
                    { 7, 153883, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx153883-thHiGEnqxFoB.jpg", 151483, "The SSS-Ranker Returns", "SSS-geup Ranker Hoegwihada" },
                    { 8, 125636, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx125636-g0gkyLZbo3Tz.png", 147322, "The Max Level Hero Strikes Back!", "Man-Level Yeongung-nim-kkeseo Gwihwan Hasinda!" },
                    { 9, 151025, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx151025-j7nZBNb46cv9.jpg", 150561, "Mythic Item Obtained", "Sinhwa-geup Gwisok Item-eul Son-e Neoeotda" },
                    { 10, 159441, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx159441-n919hUzb0j44.jpg", 154587, "Pick Me Up", "Pick Me Up!, Infinite Gacha" },
                    { 11, 167318, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx167318-fmcRXTsFE99i.jpg", 159916, "The Extra Is Too Powerful", "Extra-ga Neomu Gangham" },
                    { 12, 166635, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx166635-6Y7R6AZe52Fv.jpg", 160118, "All-Master Necromancer", "Absolute Necromancer" },
                    { 13, 130511, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx130511-4O6dF8oaiVJh.jpg", 147995, "The Player Who Can't Level Up", "Level Up Mothaneun Player" },
                    { 14, 137280, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx137280-C8kbBitLxlwR.png", 147392, "I'm the Max-Level Newbie", "Na Honja Man-Level Newbie" },
                    { 15, 110989, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx110989-DKLKwQ5ojqXD.jpg", 122650, "Failure Frame: I Became the Strongest and Annihilated Everything With Low-Level Spells", "Hazurewaku no \"Joutai Ijou Skill\" de Saikyou ni Natta Ore ga Subete wo Juurin suru made" },
                    { 16, 86635, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx86635-EdaLQmsn86Fy.png", 90125, "Kaguya-sama: Love Is War", "Kaguya-sama wa Kokurasetai: Tensai-tachi no Renai Zunousen" },
                    { 17, 151457, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx151457-7v5jKk5yAnAc.png", 148458, "The Overpowered Newbie", "Newbie-ga Neomu Gangham" },
                    { 18, 122063, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx122063-zq7rF3cdgxpX.jpg", 130331, "Shangri-La Frontier", "Shangri-La Frontier: Kusoge Hunter, Kamige ni Idoman to su" },
                    { 19, 30013, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx30013-ulXvn0lzWvsz.jpg", 13, "One Piece", "One Piece" },
                    { 20, 139572, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx139572-e3vwLcOVQISn.jpg", 147450, "The Lone Necromancer", "Na Honja Necromancer" },
                    { 21, 100693, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/nx100693-SwgtbDgQosE7.jpg", 111225, "The Unwanted Undead Adventurer", "Nozomanu Fushi no Boukensha" },
                    { 22, 153284, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx153284-roAlRmlRM7Vs.png", 150210, "After Ten Millennia in Hell", "Man Nyeon Man-e Gwihwanhan Player" }
                });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "BaseUrl", "DefaultUserAgent", "EngineType", "IsEnabled", "Name", "QueueName", "RateLimitMilliseconds", "RequestMode", "Slug", "SupportsPagination" },
                values: new object[] { 1, "https://api.mangadex.org/manga/", "MangaUpdater/1.0", "JsonApi", true, "MangaDex", "get-chapters-Mangadex", null, "HttpGet", "mangadex", true });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "BaseUrl", "DefaultUserAgent", "EngineType", "IsEnabled", "Name", "QueueName", "RateLimitMilliseconds", "RequestMode", "Slug" },
                values: new object[] { 2, "https://asurascans.com", null, "HtmlXPath", true, "AsuraScans", "get-chapters-AsuraScans", null, "HttpGet", "asurascans" });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "BaseUrl", "DefaultUserAgent", "EngineType", "IsEnabled", "Name", "QueueName", "RateLimitMilliseconds", "RequestMode", "Slug", "SupportsPagination" },
                values: new object[] { 3, "https://vortexscans.org/api/chapters?postId=", null, "JsonApi", true, "VortexScans", "get-chapters-VortexScans", null, "HttpGet", "vortexscans", true });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "AdditionalInfo", "Id", "Url" },
                values: new object[,]
                {
                    { 2, 1, null, 2, "fef2e4da-36f9-48e9-8317-2516f4b6ab14" },
                    { 3, 1, null, 3, "a2320293-f00e-43a0-8d08-1110cf26a894" },
                    { 4, 1, null, 4, "89ed3ec2-ebe6-4d6b-92eb-d753a8bb365e" },
                    { 5, 2, null, 5, "/comics/revenge-of-the-iron-blooded-sword-hound-5abb513e" },
                    { 6, 2, null, 6, "/comics/swordmasters-youngest-son-5abb513e" },
                    { 7, 2, null, 7, "/comics/return-of-the-sss-class-ranker-5abb513e" },
                    { 8, 2, null, 8, "/comics/the-max-level-hero-has-returned-5abb513e" },
                    { 9, 2, null, 9, "/comics/i-obtained-a-mythic-item-5abb513e" },
                    { 10, 2, null, 10, "/comics/pick-me-up-infinite-gacha-5abb513e" },
                    { 12, 2, null, 12, "/comics/absolute-necromancer-5abb513e" },
                    { 13, 2, null, 13, "/comics/player-who-cant-level-up-5abb513e" },
                    { 14, 2, null, 14, "/comics/solo-max-level-newbie-5abb513e" },
                    { 15, 1, null, 15, "0b171f64-89a5-4c37-b5f9-75cca57e8787" },
                    { 16, 1, null, 16, "37f5cce0-8070-4ada-96e5-fa24b1bd4ff9" },
                    { 17, 3, null, 17, "214" },
                    { 21, 1, null, 21, "6e44705b-9f80-42f6-9ebb-1141fbe8320e" }
                });

            migrationBuilder.InsertData(
                table: "SourceApiProfiles",
                columns: new[] { "Id", "ChapterDatePath", "ChapterNumberPath", "ChapterUrlPath", "DataRootPath", "EndpointTemplate", "HttpMethod", "IsActive", "LimitParameterName", "NextPagePath", "OffsetParameterName", "PaginationMode", "ResultLimit", "ResultUrlTemplate", "SourceId", "Version" },
                values: new object[,]
                {
                    { 1, "attributes.createdAt", "attributes.chapter", "id", "data", "{BaseUrl}{MangaUrlPart}/feed?translatedLanguage[]=en&limit=199&order[chapter]=asc&limit=200&offset={Offset}", "GET", true, null, null, "offset", "Offset", 200, "https://mangadex.org/chapter/{Value}", 1, 1 },
                    { 2, "createdAt", "number", null, "post.chapters", "{BaseUrl}{MangaUrlPart}&skip={Offset}&take=200&order=desc", "GET", true, "take", null, "skip", "Offset", 200, null, 3, 1 }
                });

            migrationBuilder.InsertData(
                table: "SourceRequestProfiles",
                columns: new[] { "Id", "AcceptLanguage", "HeadersJson", "IsActive", "Method", "Referrer", "SourceId", "TimeoutSeconds", "UrlTemplate", "UseCookies", "Version" },
                values: new object[,]
                {
                    { 1, "en-US", "{\"User-Agent\":\"MangaUpdater/1.0\"}", true, "GET", null, 1, 30, "{BaseUrl}{MangaUrlPart}/feed?translatedLanguage[]=en&limit=199&order[chapter]=asc&limit=200&offset={Offset}", false, 1 },
                    { 2, null, null, true, "GET", null, 2, 30, "{BaseUrl}{MangaUrlPart}", false, 1 },
                    { 3, null, null, true, "GET", null, 3, 30, "{BaseUrl}{MangaUrlPart}&skip={Offset}&take=200&order=desc", false, 1 }
                });

            migrationBuilder.InsertData(
                table: "SourceScrapingProfiles",
                columns: new[] { "Id", "ChapterDateAttribute", "ChapterDateRegex", "ChapterDateXPath", "ChapterNodesXPath", "ChapterNumberAttribute", "ChapterNumberRegex", "ChapterNumberXPath", "ChapterSortMode", "ChapterUrlAttribute", "ChapterUrlXPath", "DateCulture", "DateFormatPrimary", "DateFormatSecondary", "DateParseMode", "DeduplicationKeyMode", "IgnoreTextContains1", "IgnoreTextContains2", "IgnoreTextContains3", "IsActive", "PaginationNextPageXPath", "PaginationUrlTemplate", "RelativeDateRegex", "ResultLimit", "SourceId", "UrlJoinMode", "UrlPrefix", "Version" },
                values: new object[] { 1, null, "((?:(?:\\d+|a|an|one)\\s+(?:second|sec|minute|min|hour|hr|day|week|wk|month|mo|year|yr)(?:s)?\\s+ago)|today|yesterday|last\\s+(?:week|month|year)|(?:[A-Za-z]{3}\\s+\\d{1,2},\\s+\\d{4}))", ".", "//a[contains(@href, '/chapter/')]", "href", "chapter/(\\d+(\\.\\d+)?)", null, "NumericAscending", "href", ".", "en-US", "MMM dd, yyyy", null, "RelativeOrFormat", "ChapterNumber", "First Chapter", "Latest Chapter", null, true, null, null, "((?:\\d+|a|an|one))\\s+(second|sec|minute|min|hour|hr|day|week|wk|month|mo|year|yr)s?\\s+ago", 500, 2, "BaseUrlPrefix", null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_MangaId_SourceId_OriginalNumber",
                table: "Chapters",
                columns: new[] { "MangaId", "SourceId", "OriginalNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_SourceId",
                table: "Chapters",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_AniListId",
                table: "Mangas",
                column: "AniListId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_MyAnimeListId",
                table: "Mangas",
                column: "MyAnimeListId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MangaSources_SourceId",
                table: "MangaSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceApiProfiles_SourceId_Version",
                table: "SourceApiProfiles",
                columns: new[] { "SourceId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SourceRequestProfiles_SourceId_Version",
                table: "SourceRequestProfiles",
                columns: new[] { "SourceId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Slug",
                table: "Sources",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SourceScrapingProfiles_SourceId_Version",
                table: "SourceScrapingProfiles",
                columns: new[] { "SourceId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "LogEvents");

            migrationBuilder.DropTable(
                name: "MangaSources");

            migrationBuilder.DropTable(
                name: "SourceApiProfiles");

            migrationBuilder.DropTable(
                name: "SourceRequestProfiles");

            migrationBuilder.DropTable(
                name: "SourceScrapingProfiles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Mangas");

            migrationBuilder.DropTable(
                name: "Sources");
        }
    }
}

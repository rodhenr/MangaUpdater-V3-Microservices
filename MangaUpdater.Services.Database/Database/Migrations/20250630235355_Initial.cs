using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    BaseUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
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
                    AditionalInfo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                columns: new[] { "Id", "BaseUrl", "Name" },
                values: new object[,]
                {
                    { 1, "https://api.mangadex.org/manga/", "MangaDex" },
                    { 2, "https://asuracomic.net/series/", "AsuraScans" },
                    { 3, "https://vortexscans.org/api/chapters?postId=", "VortexScans" },
                    { 4, "https://xbato.com/title/", "Batoto" },
                    { 5, "https://www.snowmtl.ru/comics/", "SnowMachine" },
                    { 6, "https://api.comick.fun/comic/", "Comick" }
                });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "AditionalInfo", "Id", "Url" },
                values: new object[,]
                {
                    { 1, 6, "l_Vjpvkq", 1, "01-second-life-ranker" },
                    { 2, 1, null, 2, "fef2e4da-36f9-48e9-8317-2516f4b6ab14" },
                    { 3, 1, null, 3, "a2320293-f00e-43a0-8d08-1110cf26a894" },
                    { 4, 1, null, 4, "89ed3ec2-ebe6-4d6b-92eb-d753a8bb365e" },
                    { 5, 2, null, 5, "revenge-of-the-iron-blooded-sword-hound-da0c5e71" },
                    { 6, 2, null, 6, "swordmasters-youngest-son-e6946e27" },
                    { 7, 2, null, 7, "return-of-the-sss-class-ranker-f6fde482" },
                    { 8, 2, null, 8, "the-max-level-hero-has-returned-cc806d84" },
                    { 9, 2, null, 9, "i-obtained-a-mythic-item-5c23ef60" },
                    { 10, 2, null, 10, "pick-me-up-infinite-gacha-e764ac18" },
                    { 11, 5, null, 11, "the-extra-is-too-powerful" },
                    { 12, 2, null, 12, "absolute-necromancer-f3d79560" },
                    { 13, 2, null, 13, "player-who-cant-level-up-6937decb" },
                    { 14, 2, null, 14, "solo-max-level-newbie-6fb35ee2" },
                    { 15, 1, null, 15, "0b171f64-89a5-4c37-b5f9-75cca57e8787" },
                    { 16, 1, null, 16, "37f5cce0-8070-4ada-96e5-fa24b1bd4ff9" },
                    { 17, 3, null, 17, "214" },
                    { 18, 4, null, 18, "81512-shangri-la-frontier-official" },
                    { 19, 4, null, 19, "83510-one-piece-official" },
                    { 20, 5, null, 20, "the-lone-necromancer" },
                    { 21, 1, null, 21, "6e44705b-9f80-42f6-9ebb-1141fbe8320e" },
                    { 22, 6, "54Zwh6iY", 22, "00-player-who-returned-10-000-years-later" }
                });

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
                name: "Mangas");

            migrationBuilder.DropTable(
                name: "Sources");
        }
    }
}

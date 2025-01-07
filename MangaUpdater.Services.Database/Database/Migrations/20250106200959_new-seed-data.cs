using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class newseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 2, 1 },
                columns: new[] { "Id", "Url" },
                values: new object[] { 2, "fef2e4da-36f9-48e9-8317-2516f4b6ab14" });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "Id", "Url" },
                values: new object[] { 3, 1, 3, "a2320293-f00e-43a0-8d08-1110cf26a894" });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AniListId", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[] { 121753, 127781, "As a Reincarnated Aristocrat, I’ll Use My Appraisal Skill to Rise in the World", "Tensei Kizoku, Kantei Skill de Nariagaru: Jakushou Ryouchi wo Uketsuida node, Yuushuu na Jinzai wo Fuyashiteitara, Saikyou Ryouchi ni Natteta" });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AniListId", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[] { 114048, 123456, "The Ossan Newbie Adventurer, Trained to Death by the Most Powerful Party, Became Invincible", "Shinmai Ossan Bouken-sha, Saikyou Party ni Shinu Hodo Kitaerarete Muteki ni Naru." });

            migrationBuilder.InsertData(
                table: "Mangas",
                columns: new[] { "Id", "AniListId", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[,]
                {
                    { 4, 101715, 111466, "Saving 80,000 Gold in Another World for my Retirement", "Rougo ni Sonaete Isekai de 8-manmai no Kinka wo Tamemasu" },
                    { 5, 163824, 157888, "Revenge of the Baskerville Bloodhound", "Cheolhyeolgeomga Sanyanggaeui Hoegwi" }
                });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "Id", "Url" },
                values: new object[,]
                {
                    { 4, 1, 4, "89ed3ec2-ebe6-4d6b-92eb-d753a8bb365e" },
                    { 5, 2, 5, "revenge-of-the-iron-blooded-sword-hound-da0c5e71" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 2, 1 },
                columns: new[] { "Id", "Url" },
                values: new object[] { 3, "a1c7c817-4e59-43b7-9365-09675a149a6f" });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "Id", "Url" },
                values: new object[] { 3, 2, 4, "revenge-of-the-iron-blooded-sword-hound-da0c5e71" });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AniListId", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[] { 30013, 13, "One Piece", "One Piece" });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AniListId", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[] { 163824, 157888, "Revenge of the Baskerville Bloodhound", "Cheolhyeolgeomga Sanyanggaeui Hoegwi" });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMangaSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Mangas",
                columns: new[] { "Id", "AniListId", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[,]
                {
                    { 1, 109957, 147324, "Second Life Ranker", "Dubeon Saneun Ranker" },
                    { 2, 30013, 13, "One Piece", "One Piece" },
                    { 3, 163824, 157888, "Revenge of the Baskerville Bloodhound", "Cheolhyeolgeomga Sanyanggaeui Hoegwi" }
                });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "Id", "Url" },
                values: new object[,]
                {
                    { 1, 1, 1, "1ffca916-3ad7-46d2-9591-a9b39e639971" },
                    { 2, 1, 3, "a1c7c817-4e59-43b7-9365-09675a149a6f" },
                    { 3, 2, 4, "revenge-of-the-iron-blooded-sword-hound-da0c5e71" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}

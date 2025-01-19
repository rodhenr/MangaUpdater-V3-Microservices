using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class addtwonewmanga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Mangas",
                columns: new[] { "Id", "AniListId", "CoverUrl", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[,]
                {
                    { 20, 139572, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx139572-e3vwLcOVQISn.jpg", 147450, "The Lone Necromancer", "Na Honja Necromancer" },
                    { 21, 100693, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/nx100693-SwgtbDgQosE7.jpg", 111225, "The Unwanted Undead Adventurer ", "Nozomanu Fushi no Boukensha" }
                });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "Id", "Url" },
                values: new object[,]
                {
                    { 20, 5, 20, "the-lone-necromancer" },
                    { 21, 1, 21, "6e44705b-9f80-42f6-9ebb-1141fbe8320e" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 20, 5 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 21, 1 });

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 21);
        }
    }
}

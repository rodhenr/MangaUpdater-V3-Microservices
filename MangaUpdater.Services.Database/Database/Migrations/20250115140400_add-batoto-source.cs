using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class addbatotosource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Mangas",
                columns: new[] { "Id", "AniListId", "CoverUrl", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[] { 18, 122063, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx122063-zq7rF3cdgxpX.jpg", 130331, "Shangri-La Frontier", "Shangri-La Frontier: Kusoge Hunter, Kamige ni Idoman to su" });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "BaseUrl", "Name" },
                values: new object[] { 4, "https://xbato.com/title/", "Batoto" });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "Id", "Url" },
                values: new object[] { 18, 4, 18, "81512-shangri-la-frontier-official" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 18, 4 });

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}

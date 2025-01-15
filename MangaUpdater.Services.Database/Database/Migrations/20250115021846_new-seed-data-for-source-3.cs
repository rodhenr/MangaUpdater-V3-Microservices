using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class newseeddataforsource3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Mangas",
                columns: new[] { "Id", "AniListId", "CoverUrl", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[] { 17, 151457, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx151457-7v5jKk5yAnAc.png", 148458, "The Overpowered Newbie", "Newbie-ga Neomu Gangham" });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "BaseUrl", "Name" },
                values: new object[] { 3, "https://vortexscans.org/api/chapters?postId=", "VortexScans" });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "Id", "Url" },
                values: new object[] { 17, 3, 17, "214" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 17, 3 });

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}

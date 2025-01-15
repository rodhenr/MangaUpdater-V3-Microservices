using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class addmangaonepiece : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Mangas",
                columns: new[] { "Id", "AniListId", "CoverUrl", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[] { 19, 30013, "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx30013-ulXvn0lzWvsz.jpg", 13, "One Piece", "One Piece" });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "Id", "Url" },
                values: new object[] { 19, 4, 19, "83510-one-piece-official" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 19, 4 });

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 19);
        }
    }
}

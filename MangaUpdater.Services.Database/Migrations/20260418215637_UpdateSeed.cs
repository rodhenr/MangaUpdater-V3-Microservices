using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaUpdater.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 14, 2 },
                column: "Url",
                value: "solo-max-level-newbie-5abb513e");

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 2,
                column: "BaseUrl",
                value: "https://asurascans.com/comics/");

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 6,
                column: "BaseUrl",
                value: "https://api.comick.dev/comic/");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 14, 2 },
                column: "Url",
                value: "solo-max-level-newbie-6fb35ee2");

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 2,
                column: "BaseUrl",
                value: "https://asuracomic.net/series/");

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 6,
                column: "BaseUrl",
                value: "https://api.comick.fun/comic/");
        }
    }
}

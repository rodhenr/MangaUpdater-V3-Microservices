using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaUpdater.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedAsura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 5, 2 },
                column: "Url",
                value: "/comics/revenge-of-the-iron-blooded-sword-hound-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 6, 2 },
                column: "Url",
                value: "/comics/swordmasters-youngest-son-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 7, 2 },
                column: "Url",
                value: "/comics/return-of-the-sss-class-ranker-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 8, 2 },
                column: "Url",
                value: "/comics/the-max-level-hero-has-returned-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 9, 2 },
                column: "Url",
                value: "/comics/i-obtained-a-mythic-item-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 10, 2 },
                column: "Url",
                value: "/comics/pick-me-up-infinite-gacha-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 12, 2 },
                column: "Url",
                value: "/comics/absolute-necromancer-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 13, 2 },
                column: "Url",
                value: "/comics/player-who-cant-level-up-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 14, 2 },
                column: "Url",
                value: "/comics/solo-max-level-newbie-5abb513e");

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 2,
                column: "BaseUrl",
                value: "https://asurascans.com");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 5, 2 },
                column: "Url",
                value: "/revenge-of-the-iron-blooded-sword-hound-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 6, 2 },
                column: "Url",
                value: "/swordmasters-youngest-son-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 7, 2 },
                column: "Url",
                value: "/return-of-the-sss-class-ranker-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 8, 2 },
                column: "Url",
                value: "/the-max-level-hero-has-returned-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 9, 2 },
                column: "Url",
                value: "/i-obtained-a-mythic-item-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 10, 2 },
                column: "Url",
                value: "/pick-me-up-infinite-gacha-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 12, 2 },
                column: "Url",
                value: "/absolute-necromancer-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 13, 2 },
                column: "Url",
                value: "/player-who-cant-level-up-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 14, 2 },
                column: "Url",
                value: "/solo-max-level-newbie-5abb513e");

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 2,
                column: "BaseUrl",
                value: "https://asurascans.com/comics");
        }
    }
}

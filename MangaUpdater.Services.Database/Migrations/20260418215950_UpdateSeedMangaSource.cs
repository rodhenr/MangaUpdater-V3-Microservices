using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaUpdater.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedMangaSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 5, 2 },
                column: "Url",
                value: "revenge-of-the-iron-blooded-sword-hound-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 6, 2 },
                column: "Url",
                value: "swordmasters-youngest-son-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 7, 2 },
                column: "Url",
                value: "return-of-the-sss-class-ranker-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 8, 2 },
                column: "Url",
                value: "the-max-level-hero-has-returned-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 9, 2 },
                column: "Url",
                value: "i-obtained-a-mythic-item-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 10, 2 },
                column: "Url",
                value: "pick-me-up-infinite-gacha-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 12, 2 },
                column: "Url",
                value: "absolute-necromancer-5abb513e");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 13, 2 },
                column: "Url",
                value: "player-who-cant-level-up-5abb513e");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 5, 2 },
                column: "Url",
                value: "revenge-of-the-iron-blooded-sword-hound-da0c5e71");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 6, 2 },
                column: "Url",
                value: "swordmasters-youngest-son-e6946e27");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 7, 2 },
                column: "Url",
                value: "return-of-the-sss-class-ranker-f6fde482");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 8, 2 },
                column: "Url",
                value: "the-max-level-hero-has-returned-cc806d84");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 9, 2 },
                column: "Url",
                value: "i-obtained-a-mythic-item-5c23ef60");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 10, 2 },
                column: "Url",
                value: "pick-me-up-infinite-gacha-e764ac18");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 12, 2 },
                column: "Url",
                value: "absolute-necromancer-f3d79560");

            migrationBuilder.UpdateData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 13, 2 },
                column: "Url",
                value: "player-who-cant-level-up-6937decb");
        }
    }
}

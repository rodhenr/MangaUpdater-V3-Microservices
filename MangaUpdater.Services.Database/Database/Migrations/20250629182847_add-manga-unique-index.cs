using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class addmangauniqueindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Mangas_AniListId",
                table: "Mangas");

            migrationBuilder.DropIndex(
                name: "IX_Mangas_MyAnimeListId",
                table: "Mangas");
        }
    }
}

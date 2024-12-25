using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MangaSources",
                table: "MangaSources");

            migrationBuilder.DropIndex(
                name: "IX_MangaSources_MangaId",
                table: "MangaSources");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_MangaId",
                table: "Chapters");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sources",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BaseUrl",
                table: "Sources",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MangaSources",
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId" });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "BaseUrl", "Name" },
                values: new object[,]
                {
                    { 1, "https://api.mangadex.org/manga/", "MangaDex" },
                    { 2, "https://asuracomic.net/series/", "AsuraScans" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_MangaId_SourceId_Number",
                table: "Chapters",
                columns: new[] { "MangaId", "SourceId", "Number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MangaSources",
                table: "MangaSources");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_MangaId_SourceId_Number",
                table: "Chapters");

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sources",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "BaseUrl",
                table: "Sources",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MangaSources",
                table: "MangaSources",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MangaSources_MangaId",
                table: "MangaSources",
                column: "MangaId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_MangaId",
                table: "Chapters",
                column: "MangaId");
        }
    }
}

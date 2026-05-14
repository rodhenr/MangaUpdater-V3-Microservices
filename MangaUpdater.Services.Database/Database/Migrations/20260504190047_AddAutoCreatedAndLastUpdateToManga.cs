using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoCreatedAndLastUpdateToManga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TitleEnglish",
                table: "Mangas",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "MyAnimeListId",
                table: "Mangas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoCreated",
                table: "Mangas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Mangas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "IsAutoCreated", "LastUpdate" },
                values: new object[] { false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoCreated",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Mangas");

            migrationBuilder.AlterColumn<string>(
                name: "TitleEnglish",
                table: "Mangas",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MyAnimeListId",
                table: "Mangas",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}

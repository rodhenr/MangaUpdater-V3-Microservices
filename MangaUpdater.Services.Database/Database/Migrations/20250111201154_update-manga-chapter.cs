using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class updatemangachapter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "Mangas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Chapters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx109957-EgJWdR7l9TBG.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx121753-vhvIdfxdaEdF.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx114048-4HEtdYDcXI8r.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/nx101715-4yYFDOadUtnC.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx163824-KiablxybJD6i.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 6,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx149332-adkSyOFY3c5U.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 7,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx153883-thHiGEnqxFoB.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 8,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx125636-g0gkyLZbo3Tz.png");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 9,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx151025-j7nZBNb46cv9.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 10,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx159441-n919hUzb0j44.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 11,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx167318-fmcRXTsFE99i.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 12,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx166635-6Y7R6AZe52Fv.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 13,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx130511-4O6dF8oaiVJh.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 14,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx137280-C8kbBitLxlwR.png");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 15,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx110989-DKLKwQ5ojqXD.jpg");

            migrationBuilder.UpdateData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 16,
                column: "CoverUrl",
                value: "https://s4.anilist.co/file/anilistcdn/media/manga/cover/large/bx86635-EdaLQmsn86Fy.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Chapters");
        }
    }
}

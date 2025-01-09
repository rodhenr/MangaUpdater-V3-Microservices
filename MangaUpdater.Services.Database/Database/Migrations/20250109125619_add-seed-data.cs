using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MangaUpdater.Services.Database.Database.Migrations
{
    /// <inheritdoc />
    public partial class addseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Mangas",
                columns: new[] { "Id", "AniListId", "MyAnimeListId", "TitleEnglish", "TitleRomaji" },
                values: new object[,]
                {
                    { 6, 149332, 146949, "The Swordmaster's Son", "Geomsul Myeongga Mangnaeadeul" },
                    { 7, 153883, 151483, "The SSS-Ranker Returns", "SSS-geup Ranker Hoegwihada" },
                    { 8, 125636, 147322, "The Max Level Hero Strikes Back!", "Man-Level Yeongung-nim-kkeseo Gwihwan Hasinda!" },
                    { 9, 151025, 150561, "Mythic Item Obtained", "Sinhwa-geup Gwisok Item-eul Son-e Neoeotda" },
                    { 10, 159441, 154587, "Pick Me Up", "Pick Me Up!, Infinite Gacha" },
                    { 11, 167318, 159916, "The Extra Is Too Powerful", "Extra-ga Neomu Gangham" },
                    { 12, 166635, 160118, "All-Master Necromancer", "Absolute Necromancer" },
                    { 13, 130511, 147995, "The Player Who Can't Level Up", "Level Up Mothaneun Player" },
                    { 14, 137280, 147392, "I'm the Max-Level Newbie", "Na Honja Man-Level Newbie" },
                    { 15, 110989, 122650, "Failure Frame: I Became the Strongest and Annihilated Everything With Low-Level Spells", "Hazurewaku no \"Joutai Ijou Skill\" de Saikyou ni Natta Ore ga Subete wo Juurin suru made" },
                    { 16, 86635, 90125, "Kaguya-sama: Love Is War", "Kaguya-sama wa Kokurasetai: Tensai-tachi no Renai Zunousen" }
                });

            migrationBuilder.InsertData(
                table: "MangaSources",
                columns: new[] { "MangaId", "SourceId", "Id", "Url" },
                values: new object[,]
                {
                    { 6, 2, 6, "swordmasters-youngest-son-e6946e27" },
                    { 7, 2, 7, "return-of-the-sss-class-ranker-f6fde482" },
                    { 8, 2, 8, "the-max-level-hero-has-returned-cc806d84" },
                    { 9, 2, 9, "i-obtained-a-mythic-item-5c23ef60" },
                    { 10, 2, 10, "pick-me-up-infinite-gacha-e764ac18" },
                    { 11, 2, 11, "the-extra-is-too-strong-ac4babd7" },
                    { 12, 2, 12, "absolute-necromancer-f3d79560" },
                    { 13, 2, 13, "player-who-cant-level-up-6937decb" },
                    { 14, 2, 14, "solo-max-level-newbie-6fb35ee2" },
                    { 15, 1, 15, "0b171f64-89a5-4c37-b5f9-75cca57e8787" },
                    { 16, 1, 16, "37f5cce0-8070-4ada-96e5-fa24b1bd4ff9" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 8, 2 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 9, 2 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 10, 2 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 11, 2 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 12, 2 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 13, 2 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 14, 2 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 15, 1 });

            migrationBuilder.DeleteData(
                table: "MangaSources",
                keyColumns: new[] { "MangaId", "SourceId" },
                keyValues: new object[] { 16, 1 });

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Mangas",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}

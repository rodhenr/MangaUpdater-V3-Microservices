﻿// <auto-generated />
using System;
using MangaUpdater.Services.Database.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MangaUpdater.Services.Database.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250106200959_new-seed-data")]
    partial class newseeddata
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MangaUpdater.Services.Database.Entities.Chapter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MangaId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Number")
                        .HasColumnType("numeric");

                    b.Property<int>("SourceId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SourceId");

                    b.HasIndex("MangaId", "SourceId", "Number")
                        .IsUnique();

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("MangaUpdater.Services.Database.Entities.Manga", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AniListId")
                        .HasColumnType("integer");

                    b.Property<int>("MyAnimeListId")
                        .HasColumnType("integer");

                    b.Property<string>("TitleEnglish")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("TitleRomaji")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.ToTable("Mangas");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AniListId = 109957,
                            MyAnimeListId = 147324,
                            TitleEnglish = "Second Life Ranker",
                            TitleRomaji = "Dubeon Saneun Ranker"
                        },
                        new
                        {
                            Id = 2,
                            AniListId = 121753,
                            MyAnimeListId = 127781,
                            TitleEnglish = "As a Reincarnated Aristocrat, I’ll Use My Appraisal Skill to Rise in the World",
                            TitleRomaji = "Tensei Kizoku, Kantei Skill de Nariagaru: Jakushou Ryouchi wo Uketsuida node, Yuushuu na Jinzai wo Fuyashiteitara, Saikyou Ryouchi ni Natteta"
                        },
                        new
                        {
                            Id = 3,
                            AniListId = 114048,
                            MyAnimeListId = 123456,
                            TitleEnglish = "The Ossan Newbie Adventurer, Trained to Death by the Most Powerful Party, Became Invincible",
                            TitleRomaji = "Shinmai Ossan Bouken-sha, Saikyou Party ni Shinu Hodo Kitaerarete Muteki ni Naru."
                        },
                        new
                        {
                            Id = 4,
                            AniListId = 101715,
                            MyAnimeListId = 111466,
                            TitleEnglish = "Saving 80,000 Gold in Another World for my Retirement",
                            TitleRomaji = "Rougo ni Sonaete Isekai de 8-manmai no Kinka wo Tamemasu"
                        },
                        new
                        {
                            Id = 5,
                            AniListId = 163824,
                            MyAnimeListId = 157888,
                            TitleEnglish = "Revenge of the Baskerville Bloodhound",
                            TitleRomaji = "Cheolhyeolgeomga Sanyanggaeui Hoegwi"
                        });
                });

            modelBuilder.Entity("MangaUpdater.Services.Database.Entities.MangaSource", b =>
                {
                    b.Property<int>("MangaId")
                        .HasColumnType("integer");

                    b.Property<int>("SourceId")
                        .HasColumnType("integer");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("MangaId", "SourceId");

                    b.HasIndex("SourceId");

                    b.ToTable("MangaSources");

                    b.HasData(
                        new
                        {
                            MangaId = 1,
                            SourceId = 1,
                            Id = 1,
                            Url = "1ffca916-3ad7-46d2-9591-a9b39e639971"
                        },
                        new
                        {
                            MangaId = 2,
                            SourceId = 1,
                            Id = 2,
                            Url = "fef2e4da-36f9-48e9-8317-2516f4b6ab14"
                        },
                        new
                        {
                            MangaId = 3,
                            SourceId = 1,
                            Id = 3,
                            Url = "a2320293-f00e-43a0-8d08-1110cf26a894"
                        },
                        new
                        {
                            MangaId = 4,
                            SourceId = 1,
                            Id = 4,
                            Url = "89ed3ec2-ebe6-4d6b-92eb-d753a8bb365e"
                        },
                        new
                        {
                            MangaId = 5,
                            SourceId = 2,
                            Id = 5,
                            Url = "revenge-of-the-iron-blooded-sword-hound-da0c5e71"
                        });
                });

            modelBuilder.Entity("MangaUpdater.Services.Database.Entities.Source", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BaseUrl")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Sources");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BaseUrl = "https://api.mangadex.org/manga/",
                            Name = "MangaDex"
                        },
                        new
                        {
                            Id = 2,
                            BaseUrl = "https://asuracomic.net/series/",
                            Name = "AsuraScans"
                        });
                });

            modelBuilder.Entity("MangaUpdater.Services.Database.Entities.Chapter", b =>
                {
                    b.HasOne("MangaUpdater.Services.Database.Entities.Manga", "Manga")
                        .WithMany("Chapters")
                        .HasForeignKey("MangaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MangaUpdater.Services.Database.Entities.Source", "Source")
                        .WithMany("Chapters")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Manga");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("MangaUpdater.Services.Database.Entities.MangaSource", b =>
                {
                    b.HasOne("MangaUpdater.Services.Database.Entities.Manga", "Manga")
                        .WithMany("MangaSources")
                        .HasForeignKey("MangaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MangaUpdater.Services.Database.Entities.Source", "Source")
                        .WithMany("MangaSources")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Manga");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("MangaUpdater.Services.Database.Entities.Manga", b =>
                {
                    b.Navigation("Chapters");

                    b.Navigation("MangaSources");
                });

            modelBuilder.Entity("MangaUpdater.Services.Database.Entities.Source", b =>
                {
                    b.Navigation("Chapters");

                    b.Navigation("MangaSources");
                });
#pragma warning restore 612, 618
        }
    }
}

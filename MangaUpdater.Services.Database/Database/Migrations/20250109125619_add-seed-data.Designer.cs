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
    [Migration("20250109125619_add-seed-data")]
    partial class addseeddata
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

            modelBuilder.Entity("MangaUpdater.Services.Database.Entities.LogEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Exception")
                        .HasColumnType("text");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Module")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("LogEvents");
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
                        },
                        new
                        {
                            Id = 6,
                            AniListId = 149332,
                            MyAnimeListId = 146949,
                            TitleEnglish = "The Swordmaster's Son",
                            TitleRomaji = "Geomsul Myeongga Mangnaeadeul"
                        },
                        new
                        {
                            Id = 7,
                            AniListId = 153883,
                            MyAnimeListId = 151483,
                            TitleEnglish = "The SSS-Ranker Returns",
                            TitleRomaji = "SSS-geup Ranker Hoegwihada"
                        },
                        new
                        {
                            Id = 8,
                            AniListId = 125636,
                            MyAnimeListId = 147322,
                            TitleEnglish = "The Max Level Hero Strikes Back!",
                            TitleRomaji = "Man-Level Yeongung-nim-kkeseo Gwihwan Hasinda!"
                        },
                        new
                        {
                            Id = 9,
                            AniListId = 151025,
                            MyAnimeListId = 150561,
                            TitleEnglish = "Mythic Item Obtained",
                            TitleRomaji = "Sinhwa-geup Gwisok Item-eul Son-e Neoeotda"
                        },
                        new
                        {
                            Id = 10,
                            AniListId = 159441,
                            MyAnimeListId = 154587,
                            TitleEnglish = "Pick Me Up",
                            TitleRomaji = "Pick Me Up!, Infinite Gacha"
                        },
                        new
                        {
                            Id = 11,
                            AniListId = 167318,
                            MyAnimeListId = 159916,
                            TitleEnglish = "The Extra Is Too Powerful",
                            TitleRomaji = "Extra-ga Neomu Gangham"
                        },
                        new
                        {
                            Id = 12,
                            AniListId = 166635,
                            MyAnimeListId = 160118,
                            TitleEnglish = "All-Master Necromancer",
                            TitleRomaji = "Absolute Necromancer"
                        },
                        new
                        {
                            Id = 13,
                            AniListId = 130511,
                            MyAnimeListId = 147995,
                            TitleEnglish = "The Player Who Can't Level Up",
                            TitleRomaji = "Level Up Mothaneun Player"
                        },
                        new
                        {
                            Id = 14,
                            AniListId = 137280,
                            MyAnimeListId = 147392,
                            TitleEnglish = "I'm the Max-Level Newbie",
                            TitleRomaji = "Na Honja Man-Level Newbie"
                        },
                        new
                        {
                            Id = 15,
                            AniListId = 110989,
                            MyAnimeListId = 122650,
                            TitleEnglish = "Failure Frame: I Became the Strongest and Annihilated Everything With Low-Level Spells",
                            TitleRomaji = "Hazurewaku no \"Joutai Ijou Skill\" de Saikyou ni Natta Ore ga Subete wo Juurin suru made"
                        },
                        new
                        {
                            Id = 16,
                            AniListId = 86635,
                            MyAnimeListId = 90125,
                            TitleEnglish = "Kaguya-sama: Love Is War",
                            TitleRomaji = "Kaguya-sama wa Kokurasetai: Tensai-tachi no Renai Zunousen"
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
                        },
                        new
                        {
                            MangaId = 6,
                            SourceId = 2,
                            Id = 6,
                            Url = "swordmasters-youngest-son-e6946e27"
                        },
                        new
                        {
                            MangaId = 7,
                            SourceId = 2,
                            Id = 7,
                            Url = "return-of-the-sss-class-ranker-f6fde482"
                        },
                        new
                        {
                            MangaId = 8,
                            SourceId = 2,
                            Id = 8,
                            Url = "the-max-level-hero-has-returned-cc806d84"
                        },
                        new
                        {
                            MangaId = 9,
                            SourceId = 2,
                            Id = 9,
                            Url = "i-obtained-a-mythic-item-5c23ef60"
                        },
                        new
                        {
                            MangaId = 10,
                            SourceId = 2,
                            Id = 10,
                            Url = "pick-me-up-infinite-gacha-e764ac18"
                        },
                        new
                        {
                            MangaId = 11,
                            SourceId = 2,
                            Id = 11,
                            Url = "the-extra-is-too-strong-ac4babd7"
                        },
                        new
                        {
                            MangaId = 12,
                            SourceId = 2,
                            Id = 12,
                            Url = "absolute-necromancer-f3d79560"
                        },
                        new
                        {
                            MangaId = 13,
                            SourceId = 2,
                            Id = 13,
                            Url = "player-who-cant-level-up-6937decb"
                        },
                        new
                        {
                            MangaId = 14,
                            SourceId = 2,
                            Id = 14,
                            Url = "solo-max-level-newbie-6fb35ee2"
                        },
                        new
                        {
                            MangaId = 15,
                            SourceId = 1,
                            Id = 15,
                            Url = "0b171f64-89a5-4c37-b5f9-75cca57e8787"
                        },
                        new
                        {
                            MangaId = 16,
                            SourceId = 1,
                            Id = 16,
                            Url = "37f5cce0-8070-4ada-96e5-fa24b1bd4ff9"
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

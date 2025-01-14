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
    [Migration("20241225191823_Migration2")]
    partial class Migration2
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

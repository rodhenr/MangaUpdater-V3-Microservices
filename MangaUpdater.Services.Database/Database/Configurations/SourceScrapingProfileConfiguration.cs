using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaUpdater.Services.Database.Database.Configurations;

public class SourceScrapingProfileConfiguration : IEntityTypeConfiguration<SourceScrapingProfile>
{
    public void Configure(EntityTypeBuilder<SourceScrapingProfile> builder)
    {
        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasOne(x => x.Source)
            .WithMany(x => x.ScrapingProfiles)
            .HasForeignKey(x => x.SourceId);

        builder
            .Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder
            .Property(x => x.Version)
            .HasDefaultValue(1);

        builder
            .Property(x => x.ChapterNodesXPath)
            .HasMaxLength(1000);

        builder
            .Property(x => x.ChapterUrlXPath)
            .HasMaxLength(1000);

        builder
            .Property(x => x.ChapterUrlAttribute)
            .HasMaxLength(100);

        builder
            .Property(x => x.ChapterNumberXPath)
            .HasMaxLength(1000);

        builder
            .Property(x => x.ChapterNumberAttribute)
            .HasMaxLength(100);

        builder
            .Property(x => x.ChapterNumberRegex)
            .HasMaxLength(1000);

        builder
            .Property(x => x.ChapterDateXPath)
            .HasMaxLength(1000);

        builder
            .Property(x => x.ChapterDateAttribute)
            .HasMaxLength(100);

        builder
            .Property(x => x.ChapterDateRegex)
            .HasMaxLength(1000);

        builder
            .Property(x => x.DateParseMode)
            .HasMaxLength(50);

        builder
            .Property(x => x.DateCulture)
            .HasMaxLength(50);

        builder
            .Property(x => x.DateFormatPrimary)
            .HasMaxLength(100);

        builder
            .Property(x => x.DateFormatSecondary)
            .HasMaxLength(100);

        builder
            .Property(x => x.RelativeDateRegex)
            .HasMaxLength(1000);

        builder
            .Property(x => x.IgnoreTextContains1)
            .HasMaxLength(200);

        builder
            .Property(x => x.IgnoreTextContains2)
            .HasMaxLength(200);

        builder
            .Property(x => x.IgnoreTextContains3)
            .HasMaxLength(200);

        builder
            .Property(x => x.UrlPrefix)
            .HasMaxLength(500);

        builder
            .Property(x => x.UrlJoinMode)
            .HasMaxLength(50);

        builder
            .Property(x => x.DeduplicationKeyMode)
            .HasMaxLength(50);

        builder
            .Property(x => x.ChapterSortMode)
            .HasMaxLength(50);

        builder
            .Property(x => x.PaginationNextPageXPath)
            .HasMaxLength(1000);

        builder
            .Property(x => x.PaginationUrlTemplate)
            .HasMaxLength(1000);

        builder
            .Property(x => x.Timestamp)
            .HasDefaultValueSql("timezone('utc', now())");

        builder
            .HasIndex(x => new { x.SourceId, x.Version })
            .IsUnique();
    }
}
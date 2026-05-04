using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaUpdater.Services.Database.Database.Configurations;

public class SourceApiProfileConfiguration : IEntityTypeConfiguration<SourceApiProfile>
{
    public void Configure(EntityTypeBuilder<SourceApiProfile> builder)
    {
        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasOne(x => x.Source)
            .WithMany(x => x.ApiProfiles)
            .HasForeignKey(x => x.SourceId);

        builder
            .Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder
            .Property(x => x.Version)
            .HasDefaultValue(1);

        builder
            .Property(x => x.EndpointTemplate)
            .HasMaxLength(1000);

        builder
            .Property(x => x.HttpMethod)
            .HasMaxLength(10)
            .HasDefaultValue("GET");

        builder
            .Property(x => x.DataRootPath)
            .HasMaxLength(200);

        builder
            .Property(x => x.ChapterNumberPath)
            .HasMaxLength(200);

        builder
            .Property(x => x.ChapterDatePath)
            .HasMaxLength(200);

        builder
            .Property(x => x.ChapterUrlPath)
            .HasMaxLength(200);

        builder
            .Property(x => x.PaginationMode)
            .HasMaxLength(50);

        builder
            .Property(x => x.OffsetParameterName)
            .HasMaxLength(50);

        builder
            .Property(x => x.LimitParameterName)
            .HasMaxLength(50);

        builder
            .Property(x => x.NextPagePath)
            .HasMaxLength(200);

        builder
            .Property(x => x.Timestamp)
            .HasDefaultValueSql("timezone('utc', now())");

        builder
            .HasIndex(x => new { x.SourceId, x.Version })
            .IsUnique();
    }
}
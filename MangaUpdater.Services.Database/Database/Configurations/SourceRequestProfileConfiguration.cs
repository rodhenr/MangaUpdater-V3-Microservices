using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaUpdater.Services.Database.Database.Configurations;

public class SourceRequestProfileConfiguration : IEntityTypeConfiguration<SourceRequestProfile>
{
    public void Configure(EntityTypeBuilder<SourceRequestProfile> builder)
    {
        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasOne(x => x.Source)
            .WithMany(x => x.RequestProfiles)
            .HasForeignKey(x => x.SourceId);

        builder
            .Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder
            .Property(x => x.Version)
            .HasDefaultValue(1);

        builder
            .Property(x => x.Method)
            .HasMaxLength(10)
            .HasDefaultValue("GET");

        builder
            .Property(x => x.UrlTemplate)
            .HasMaxLength(1000);

        builder
            .Property(x => x.HeadersJson)
            .HasColumnType("text");

        builder
            .Property(x => x.AcceptLanguage)
            .HasMaxLength(50);

        builder
            .Property(x => x.Referrer)
            .HasMaxLength(500);

        builder
            .Property(x => x.Timestamp)
            .HasDefaultValueSql("timezone('utc', now())");

        builder
            .HasIndex(x => new { x.SourceId, x.Version })
            .IsUnique();
    }
}
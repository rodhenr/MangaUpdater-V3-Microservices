using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaUpdater.Services.Database.Database.Configurations;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder
            .Property(ms => ms.Id)
            .ValueGeneratedOnAdd();
        
        builder
            .HasIndex(ch => new { ch.MangaId, ch.SourceId, ch.OriginalNumber })
            .IsUnique();

        builder.HasOne(ch => ch.Manga)
            .WithMany(ch => ch.Chapters)
            .HasForeignKey(ch => ch.MangaId);

        builder.HasOne(ch => ch.Source)
            .WithMany(ch => ch.Chapters)
            .HasForeignKey(ch => ch.SourceId);
        
        builder
            .Property(m => m.Timestamp)
            .HasDefaultValueSql("timezone('utc', now())");
    }
}
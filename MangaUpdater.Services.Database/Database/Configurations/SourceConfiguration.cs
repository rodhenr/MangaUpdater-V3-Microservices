using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaUpdater.Services.Database.Database.Configurations;

public class SourceConfiguration : IEntityTypeConfiguration<Source>
{
    public void Configure(EntityTypeBuilder<Source> builder)
    {
        builder
            .Property(ms => ms.Id)
            .ValueGeneratedOnAdd();
        
        builder
            .Property(s => s.Name)
            .HasMaxLength(50);

        builder
            .Property(s => s.Slug)
            .HasMaxLength(100);
        
        builder
            .Property(s => s.BaseUrl)
            .HasMaxLength(100);

        builder
            .Property(s => s.EngineType)
            .HasMaxLength(50)
            .HasDefaultValue("HtmlXPath");

        builder
            .Property(s => s.RequestMode)
            .HasMaxLength(50)
            .HasDefaultValue("HttpGet");

        builder
            .Property(s => s.DefaultUserAgent)
            .HasMaxLength(500);

        builder
            .Property(s => s.QueueName)
            .HasMaxLength(150);

        builder
            .Property(s => s.IsEnabled)
            .HasDefaultValue(true);

        builder
            .Property(s => s.RequiresBrowser)
            .HasDefaultValue(false);

        builder
            .Property(s => s.SupportsPagination)
            .HasDefaultValue(false);
        
        builder
            .Property(m => m.Timestamp)
            .HasDefaultValueSql("timezone('utc', now())");

        builder
            .HasIndex(s => s.Slug)
            .IsUnique();
    }
}
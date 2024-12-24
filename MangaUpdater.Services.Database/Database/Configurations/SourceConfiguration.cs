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
            .Property(s => s.BaseUrl)
            .HasMaxLength(100);
    }
}
using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaUpdater.Services.Database.Database.Configurations;

public class MangaConfiguration : IEntityTypeConfiguration<Manga>
{
    public void Configure(EntityTypeBuilder<Manga> builder)
    {
        builder
            .Property(ms => ms.Id)
            .ValueGeneratedOnAdd();
        
        builder
            .Property(m => m.TitleEnglish)
            .HasMaxLength(200);

        builder
            .Property(m => m.TitleRomaji)
            .HasMaxLength(200);
    }
}
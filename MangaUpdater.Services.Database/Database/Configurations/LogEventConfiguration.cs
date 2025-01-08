using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaUpdater.Services.Database.Database.Configurations;

public class LogEventConfiguration : IEntityTypeConfiguration<LogEvent>
{
    public void Configure(EntityTypeBuilder<LogEvent> builder)
    {
        builder
            .Property(ms => ms.Id)
            .ValueGeneratedOnAdd();
        
        builder
            .Property(m => m.Module)
            .HasMaxLength(100);
    }
}
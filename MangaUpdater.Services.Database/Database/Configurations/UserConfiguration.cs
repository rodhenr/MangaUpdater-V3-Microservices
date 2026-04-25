using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaUpdater.Services.Database.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .HasIndex(u => u.Username)
            .IsUnique();

        builder
            .Property(u => u.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder
            .Property(u => u.Role)
            .HasMaxLength(50)
            .HasDefaultValue("user");

        builder
            .Property(u => u.Timestamp)
            .HasDefaultValueSql("timezone('utc', now())");
    }
}

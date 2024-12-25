using MangaUpdater.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaUpdater.Services.Database.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Manga> Mangas { get; set; }
    public DbSet<MangaSource> MangaSources { get; set; }
    public DbSet<Source> Sources { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        builder.Entity<Source>().HasData(
            new Source { Id = 1, Name = "MangaDex", BaseUrl = "https://api.mangadex.org/manga/" },
            new Source { Id = 2, Name = "AsuraScans", BaseUrl = "https://asuracomic.net/series/" }
        );
    }
}
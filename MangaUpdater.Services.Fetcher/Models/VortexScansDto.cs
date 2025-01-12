namespace MangaUpdater.Services.Fetcher.Models;

public class VortexScansDto
{
    public Post Post { get; set; }
}

public class Post
{
    public List<VortexScansChapter> Chapters { get; set; }
}

public class VortexScansChapter
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
}
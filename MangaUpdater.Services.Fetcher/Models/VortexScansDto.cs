namespace MangaUpdater.Services.Fetcher.Models;

public class VortexScansDto
{
    public Post Post { get; set; }
}

public class Post
{
    public List<VortexScansChapter> Chapters { get; set; }
    public int TotalChapterCount { get; set; }
}

public class VortexScansChapter
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public int Number { get; set; }
    public string Title { get; set; }
    public CreatedBy CreatedBy { get; set; }
    public DateTime? UnlockAt { get; set; }
    public int Price { get; set; }
    public int MangaPostId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FeaturedImage { get; set; }
    public string ChapterStatus { get; set; }
    public MangaPost MangaPost { get; set; }
    public Count Count { get; set; }
    public bool ChapterPurchased { get; set; }
    public bool? IsLocked { get; set; }
    public bool IsAccessible { get; set; }
}

public class CreatedBy
{
    public string Name { get; set; }
}

public class MangaPost
{
    public string PostTitle { get; set; }
    public string Slug { get; set; }
    public string FeaturedImage { get; set; }
}

public class Count
{
    public int Likes { get; set; }
}
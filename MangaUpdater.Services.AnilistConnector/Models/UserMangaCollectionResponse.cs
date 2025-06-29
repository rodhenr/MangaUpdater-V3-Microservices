namespace MangaUpdater.Services.AnilistConnector.Models;

public class UserMangaCollectionResponse
{
    public MediaListCollection? MediaListCollection { get; set; }
}

public class MediaListCollection
{
    public List<List> Lists { get; set; }
}

public class List
{
    public List<Entry> Entries { get; set; }
}

public class Entry
{
    public int? Progress { get; set; }
    public Media Media { get; set; }
}

public class Media
{
    public string CountryOfOrigin { get; set; }
    public int? Id { get; set; }
    public int? IdMal { get; set; }
    public string SiteUrl { get; set; }
    public CoverImage CoverImage { get; set; }
    public Title Title { get; set; }
    public string Status { get; set; }
    public List<string> Genres { get; set; }
    public int AverageScore { get; set; }
}

public class CoverImage
{
    public string ExtraLarge { get; set; }
}

public class Title
{
    public string Romaji { get; set; }
    public string English { get; set; }
}
using System.Text.Json.Serialization;

namespace ComicApi.Model.Requests;

public class FavoriteComic
{
    public string? UserId { get; set; }
    public string? Comic { get; set; }
    public string? ComicName { get; set; }
    public string? IconUrl { get; set; }
}

public class FavoriteComicLevel
{
    public string? UserId { get; set; }
    public string? Comic { get; set; }
    public int? Level { get; set; }
}

public class FavoriteChapter
{
    public string? UserId { get; set; }
    public string? Comic { get; set; }
    public string? Chapter { get; set; }
    public string? ChapterName { get; set; }
}
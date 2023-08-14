namespace ComicCatcherLib.ComicModels;

public class ComicChapter : ComicBaseProperty
{
    public List<ComicPage> Pages { get; set; } = new List<ComicPage>();
}
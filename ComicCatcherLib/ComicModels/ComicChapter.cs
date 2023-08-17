namespace ComicCatcherLib.ComicModels;

public class ComicChapter : ComicBaseProperty
{
    public List<ComicPage> Pages { get; set; } = new List<ComicPage>();
    public ComicState ListState { get; set; } = ComicState.Created;
}
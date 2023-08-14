namespace ComicCatcherLib.ComicModels;

public class ComicPage : ComicBaseProperty
{
    public int PageNumber { get; set; }
    public string PageFileName { get; set; }
    public string Refer { get; set; }
}
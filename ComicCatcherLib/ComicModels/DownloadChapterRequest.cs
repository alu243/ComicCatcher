namespace ComicCatcherLib.ComicModels;

public class DownloadChapterRequest
{
    public string Name { get; set; }
    public ComicChapter Chapter { get; set; }
    public string Path { get; set; }
    public Action<int, object> ReportProgressAction { get; set; }
}
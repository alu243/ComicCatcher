using ComicCatcher.ComicModels.Domains;

namespace ComicCatcher.ComicModels;

public class DownloadChapterTask
{
    public string Name { get; set; }

    public ComicChapter Chapter { get; set; }
    //public List<string> downloadUrls { get; set; }
    public string Path { get; set; }

    public IComicCatcher Downloader { get; set; }
    //public List<string> downloadFileNames { get; set; }
}
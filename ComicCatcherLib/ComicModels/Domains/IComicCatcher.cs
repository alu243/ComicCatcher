namespace ComicCatcherLib.ComicModels.Domains;

public interface IComicCatcher : IDisposable
{
    ComicRoot GetRoot();
    void LoadPaginations();
    Task LoadComics(ComicPagination pagination, Dictionary<string, string> ignoreComics);
    Task LoadChapters(ComicEntity comic);
    Task LoadPages(ComicChapter chapter);
    Task DownloadChapter(DownloadChapterRequest request);
}
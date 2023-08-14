using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicCatcher.ComicModels.Domains
{
    public interface IComicCatcher : IDisposable
    {
        ComicRoot GetRoot();
        void LoadPaginations();
        Task LoadComics(ComicPagination pagination, Dictionary<string, string> ignoreComics);
        Task LoadChapters(ComicEntity comic);
        Task GetPages(ComicChapter chapter);
        Task DownloadChapter(DownloadChapterRequest request);
    }
}

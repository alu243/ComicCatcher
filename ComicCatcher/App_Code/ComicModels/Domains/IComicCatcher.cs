using System;
using System.Collections.Generic;

namespace ComicCatcher.ComicModels.Domains
{
    public interface IComicCatcher : IDisposable
    {
        ComicRoot GetRoot();
        List<ComicPagination> GetPaginations();
        void LoadComics(ComicPagination pagination);
        void LoadChapters(ComicEntity comic);
        void GetPages(ComicChapter chapter);
        void DownloadChapter(ComicChapter chapter, string downloadPath);
    }
}

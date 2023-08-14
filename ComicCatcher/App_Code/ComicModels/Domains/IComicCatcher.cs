using System;
using System.Collections.Generic;

namespace ComicCatcher.ComicModels.Domains
{
    public interface IComicCatcher : IDisposable
    {
        ComicRoot GetRoot();
        void LoadPaginations();
        void LoadComics(ComicPagination pagination, Dictionary<string, string> ignoreComics);
        void LoadChapters(ComicEntity comic);
        void GetPages(ComicChapter chapter);
        void DownloadChapter(ComicChapter chapter, string downloadPath);
    }
}

using System;
using System.Collections.Generic;

namespace ComicCatcher.ComicModels.Domains
{
    public interface IComicCatcher : IDisposable
    {
        ComicRoot GetRoot();
        List<ComicPagination> GetPaginations();
        List<ComicEntity> GetComics(ComicPagination pagination, bool loadIcon);
        List<ComicChapter> GetChapters(ComicEntity comic);
        List<ComicPage> GetPages(ComicChapter chapter);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Drawing;
using Helpers;
using Utils;
using Models;
using ComicModels;
using System.Threading;
namespace ComicModels
{
    public interface IComicCatcher : IDisposable
    {
        ComicWebRoot GetComicWebRoot();
        List<ComicWebPage> GetComicWebPages();
        List<ComicNameInWebPage> GetComicNames(ComicWebPage cGroup);
        List<ComicChapterInName> GetComicChapters(ComicNameInWebPage cName);
        List<ComicPageInChapter> GetComicPages(ComicChapterInName cChapter);
    }
}

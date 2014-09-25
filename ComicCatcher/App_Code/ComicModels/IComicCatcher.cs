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
    interface IComicCatcher : IDisposable
    {
        ComicRoot GetComicRoot();
        List<ComicGroup> GetComicGroups();
        List<ComicName> GetComicNames(ComicGroup cGroup);
        List<ComicChapter> GetComicChapters(ComicName cName);
        List<ComicPage> GetComicPages(ComicChapter cChapter);
    }
}

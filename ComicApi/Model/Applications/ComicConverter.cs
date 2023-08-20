using ComicApi.Model;
using ComicApi.Model.Repositories;
using ComicApi.Model.Requests;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using ComicCatcherLib.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace ComicApi.Controllers
{
    public class ComicConverter
    {
        public static ChapterModel Convert(string comic, string chapter, ComicEntity comicEntity, ComicChapter comicChapter)
        {
            return new ChapterModel()
            {
                Comic = comic,
                ComicName = comicEntity?.Caption ?? comic,
                Chapter = chapter,
                ChapterName = comicChapter?.Caption ?? chapter,

                CurrChapter = comicChapter,
            };
        }
    }
}


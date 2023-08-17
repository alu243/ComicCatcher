using ComicApi.Model;
using ComicApi.Model.Repositories;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class ComicController : Controller
    {
        private Dm5 dm5;
        private IHostingEnvironment env;
        private ComicApiRepository repo;
        private IMemoryCache cache;
        private MemoryCacheEntryOptions cacheOptions;

        public ComicController(Dm5 comic,
            IHostingEnvironment hostEnvironment,
            ComicApiRepository repository,
            IMemoryCache memoryCache)
        {
            dm5 = comic;
            env = hostEnvironment;
            repo = repository;
            cache = memoryCache;
            cacheOptions = new();
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(Config.CacheMinute));
        }

        [HttpPost("{comic}/{chapter}")]
        public async Task<ResponseModel> AddComicChapter(string comic, string chapter)
        {
            //dm5.GetRoot().Paginations[0].Comics.Add(Dm5.CreateComic(comicUrl));

            var comicChapter = new ComicChapter()
            {
                Caption = "",
                Pages = new List<ComicPage>(),
                Url = new Uri(new Uri(new Uri(dm5.GetRoot().Url), comic), chapter).ToString()
            };
            await dm5.LoadPages(comicChapter);
            await dm5.DownloadChapter(new DownloadChapterRequest()
            {
                Path = Path.Combine(env.WebRootPath, Config.ComicPath, comic, chapter),
                Chapter = comicChapter,
                Name = "",
                ReportProgressAction = null,
            });

            //var chapterLocalPath = ApiChapterDao.SaveChapter(comic, chapter);
            return ResponseModel.Ok();
        }


        [HttpGet("{comic}")]
        public async Task<IActionResult> ShowComic(string comic)
        {
            var comicEntity = await this.GetComic(comic);
            await dm5.LoadChapters(comicEntity);
            return View(new ComicModel() { CurrComic = comicEntity, Comic = comic });

        }

        private async Task<ComicEntity> GetComic(string comic)
        {
            string key = $"comic_{comic}";
            if (!cache.TryGetValue(key, out ComicEntity comicEntity))
            {
                //comicEntity = await repo.GetComic(comic);
                //if (comicEntity == null)
                //{
                var url = new Uri(new Uri(dm5.GetRoot().Url), comic).ToString();
                comicEntity = await dm5.GetSingleComicName(url);
                //}
                repo.SaveComic(comicEntity);

                if (comicEntity == null) return null;

                cache.Set(key, comicEntity, cacheOptions);
            }
            return comicEntity;
        }

        [HttpGet("{comic}/{chapter}")]
        public async Task<IActionResult> ShowComicChapter(string comic, string chapter)
        {
            var comicChapter = await this.GetComicChapter(comic, chapter);
            var comicEnity = await repo.GetComic(comic);
            return View(new ChapterModel()
            {
                Comic = comic,
                Chapter = chapter,
                CurrChapter = comicChapter,
                ComicName = comicEnity?.Caption ?? comic
            });
        }

        private async Task<ComicChapter> GetComicChapter(string comic, string chapter)
        {
            string key = $"comic_{comic}_chatper_{chapter}";
            if (!cache.TryGetValue(key, out ComicChapter comicChapter))
            {
                var url = new Uri(new Uri(new Uri(dm5.GetRoot().Url), comic), chapter).ToString();
                comicChapter = await repo.GetComicChapterWithPages(comic, chapter);
                if (comicChapter == null) comicChapter = await this.GetChapterFromComic(comic, chapter);

                if (comicChapter == null) return null;

                if (comicChapter.ListState == ComicState.Created)
                {
                    await dm5.LoadPages(comicChapter);
                }

                repo.SaveComicChapter(comic, chapter, comicChapter);
                repo.SaveComicPages(comic, chapter, comicChapter.Pages);

                cache.Set(key, comicChapter, cacheOptions);
            }
            return comicChapter;
        }

        private async Task<ComicChapter> GetChapterFromComic(string comic, string chapter)
        {
            var comicEntity = await this.GetComic(comic);
            await dm5.LoadChapters(comicEntity);
            var comicChapter = comicEntity.Chapters.FirstOrDefault(c => new Uri(c.Url).LocalPath.Trim('/').Equals(chapter));
            return comicChapter;
        }


        //[HttpGet("{comic}/{chapter}/{pageFile}")]
        //public FileResult GetComicPage(string comic, string chapter, string pageFile)
        //{
        //    var path = Path.Combine(env.WebRootPath,
        //        Config.ComicPath,
        //        comic,
        //        chapter,
        //        pageFile);
        //    return new FileStreamResult(new FileStream(path, FileMode.Open), "image/jpeg");
        //}
    }
}


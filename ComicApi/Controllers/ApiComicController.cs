using System.Collections;
using ComicApi.Model;
using ComicApi.Model.Repositories;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using ComicCatcherLib.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Controllers
{
    [Route("api/comic")]
    [ApiController]
    public class ApiComicController : ControllerBase
    {
        private Dm5 dm5;
        private IHostingEnvironment env;
        private ComicApplication app;


        public ApiComicController(Dm5 comic,
            IHostingEnvironment hostEnvironment,
            ComicApplication comicApplication)
        {
            dm5 = comic;
            env = hostEnvironment;
            app = comicApplication;
        }

        [HttpPost("{comic}/{chapter}")]
        public async Task<ResponseModel> AddComicChapter(string comic, string chapter)
        {
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
        public async Task<ComicModel> ShowComic(string comic)
        {
            var comicEntity = await app.GetComic(comic);
            await dm5.LoadChapters(comicEntity);
            var pageNumber = app.GetPagnationNumber(comic); 
            return new ComicModel() { CurrComic = comicEntity, Comic = comic, PageNumber = pageNumber };

        }

        #region Show ComicPages In ComicChapter
        [HttpGet("{comic}/{chapter}")]
        public async Task<ChapterModel> ShowComicPagesInChapter(string comic, string chapter)
        {
            var comicEntity = await app.GetComic(comic);
            var comicChapter = await app.GetComicChapter(comic, chapter);
            comicChapter.Pages = await app.GetComicPages(comic, chapter);
            return ComicConverter.Convert(comic, chapter, comicEntity, comicChapter);
        }

        [HttpGet("{comic}/{chapter}/next")]
        public async Task<ChapterModel> ShowComicPagesInNextChapter(string comic, string chapter)
        {
            var comicEntity = await app.GetComic(comic);
            if (comicEntity.Chapters.Count <= 0)
            {
                comicEntity.ListState = ComicState.Created;
                await dm5.LoadChapters(comicEntity);
            }
            var newChapterIndex = comicEntity.Chapters.FindIndex(c => c.Url.GetUrlDirectoryName().Equals(chapter, StringComparison.CurrentCultureIgnoreCase));

            if (newChapterIndex <= 0) return null;
            chapter = comicEntity.Chapters[newChapterIndex - 1].Url.GetUrlDirectoryName();
            var comicChapter = await app.GetComicChapter(comic, chapter);
            comicChapter.Pages = await app.GetComicPages(comic, chapter);
            return ComicConverter.Convert(comic, chapter, comicEntity, comicChapter);
        }

        [HttpPut("{comic}/{chapter}")]
        public async Task<ChapterModel> ReloadComicPagesInChapter(string comic, string chapter)
        {
            var comicChapter = await app.GetComicChapter(comic, chapter);
            comicChapter.Pages.Clear();
            comicChapter.Pages = await app.ReloadComicPages(comic, chapter);
            var comicEntity = await app.GetComic(comic);
            return ComicConverter.Convert(comic, chapter, comicEntity, comicChapter);
        }
        #endregion
    }
}


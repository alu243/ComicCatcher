using ComicApi.Model;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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

            var userId = Request.Cookies["userid"] ?? "";
            var favChapter = await app.GetFavoriteChapter(userId, comic);
            //var comicChapter = comicEntity.Chapters?.FirstOrDefault(c => c.Url.GetUrlDirectoryName().Equals(favChapter?.Chapter));
            return new ComicModel()
            {
                CurrComic = comicEntity,
                Comic = comic,
                PageNumber = pageNumber,
                ReadedChapter = favChapter?.Chapter ?? ""
            };

        }

        #region Show ComicPages In ComicChapter
        [HttpGet("{comic}/{chapter}")]
        public async Task<ChapterModel> ShowComicPagesInChapter(string comic, string chapter)
        {
            var comicEntity = await app.GetComic(comic);
            var comicChapter = await app.GetComicChapterWithPage(comic, chapter);

            // 預讀下一章
            Task.Run(async () =>
            {
                var nextChapter = await app.GetNextChapter(comicEntity, chapter);
                if (false == string.IsNullOrWhiteSpace(nextChapter))
                {
                    await app.GetComicChapterWithPage(comic, nextChapter);
                }
            });
            return ComicConverter.Convert(comic, chapter, comicEntity, comicChapter);
        }

        [HttpGet("{comic}/{chapter}/next")]
        public async Task<ChapterModel> ShowComicPagesInNextChapter(string comic, string chapter)
        {
            var comicEntity = await app.GetComic(comic);

            var nextChapter = await app.GetNextChapter(comicEntity, chapter);
            if (string.IsNullOrWhiteSpace(nextChapter)) return null;
            ComicChapter comicChapter = await app.GetComicChapterWithPage(comic, nextChapter);

            // 預讀下下一章
            Task.Run(async () =>
            {
                var nextnextChapter = await app.GetNextChapter(comicEntity, nextChapter);
                if (false == string.IsNullOrWhiteSpace(nextChapter))
                {
                    await app.GetComicChapterWithPage(comic, nextnextChapter);
                }
            });
            return ComicConverter.Convert(comic, nextChapter, comicEntity, comicChapter);
        }

        [HttpPut("{comic}/{chapter}")]
        public async Task<ChapterModel> ReloadComicPagesInChapter(string comic, string chapter)
        {
            var comicEntity = await app.GetComic(comic);
            var comicChapter = await app.GetChapterAndReloadComicPages(comic, chapter);
            return ComicConverter.Convert(comic, chapter, comicEntity, comicChapter);
        }
        #endregion
    }
}


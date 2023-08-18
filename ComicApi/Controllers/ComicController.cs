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
        private ComicApplication app;

        public ComicController(Dm5 comic,
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
        public async Task<IActionResult> ShowChaptersInComic(string comic)
        {
            var comicEntity = await app.GetComic(comic);
            await dm5.LoadChapters(comicEntity);
            return View(new ComicModel() { CurrComic = comicEntity, Comic = comic });
        }

        [HttpGet("{comic}/{chapter}")]
        public async Task<IActionResult> ShowPagesInChapter(string comic, string chapter)
        {
            var comicEnity = await app.GetComic(comic);
            return View(comicEnity);
        }
    }
}


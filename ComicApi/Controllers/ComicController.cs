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
        private ComicApplication app;

        public ComicController(ComicApplication comicApplication)
        {
            app = comicApplication;
        }

        [HttpGet("{comic}")]
        public async Task<IActionResult> ShowChaptersInComic(string comic)
        {
            return View("ShowChaptersInComic");
        }

        [HttpGet("{comic}/{chapter}")]
        public async Task<IActionResult> ShowPagesInChapter(string comic, string chapter)
        {
            var comicEnity = await app.GetComic(comic);
            return View("ShowPagesInChapter", comicEnity);
        }
    }
}


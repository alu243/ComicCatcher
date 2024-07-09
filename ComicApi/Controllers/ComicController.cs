using System.Net;
using Microsoft.AspNetCore.Mvc;

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


        [HttpGet("{comic}/{chapter}/{url}")]
        public async Task<IActionResult> GetImage(string comic, string chapter, string url)
        {
            try
            {
                url = Uri.UnescapeDataString(url);

                var content = await app.GetImage(comic, chapter, url);
                return File(content, "image/jpeg");
            }
            catch (WebException we)
            {
                return StatusCode(int.Parse(we.Message));
            }
            catch (Exception e)
            {
                return StatusCode((int)500);
            }
        }
    }
}


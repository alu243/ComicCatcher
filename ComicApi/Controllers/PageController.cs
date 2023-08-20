using ComicCatcherLib.ComicModels.Domains;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class PageController : Controller
    {
        public PageController() { }

        [HttpGet("{page}")]
        public async Task<IActionResult> ShowComics(int page)
        {
            return View("ShowComics");
        }
    }
}

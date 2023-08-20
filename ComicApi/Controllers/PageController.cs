using System.Text;
using ComicApi.Model;
using ComicApi.Model.Repositories;
using ComicApi.Model.Requests;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using ComicCatcherLib.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class PageController : Controller
    {
        private Dm5 dm5;
        private IHostingEnvironment env;
        private ComicApplication app;
        public PageController(
            Dm5 dm5,
            IHostingEnvironment hostEnvironment,
            ComicApplication comicApplication)
        {
            this.dm5 = dm5;
            env = hostEnvironment;
            app = comicApplication;
        }

        [HttpGet("{page}")]
        public async Task<IActionResult> ShowComics(int page)
        {
            return View();
        }
    }
}

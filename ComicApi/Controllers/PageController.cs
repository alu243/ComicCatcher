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

        [HttpGet("{page}/{showAll:bool?}")]
        public async Task<IActionResult> ShowComics(int page, bool? showAll)
        {
            var pagination = await app.GetPagnitation(page);
            if (page > 1) app.GetPagnitation(page - 1);
            if (page < 300) app.GetPagnitation(page + 1);

            var userId = Request.Cookies["userid"] ?? "";
            var ignoreComics = await app.GetIgnoreComics(userId);
 
            var showComics = pagination.Comics;
            if (!(true == showAll || string.IsNullOrEmpty(userId)))
            {
                showComics = pagination.Comics.Where(c => false == ignoreComics.ContainsKey(c.Url.GetUrlDirectoryName())).ToList();
            }

            var favorites = await app.GetFavoriteComicDic(userId);
            return View(new PageModel(dm5.GetRoot().Paginations, pagination, showComics, ignoreComics, favorites));
        }
    }
}

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
        private ComicApiRepository repo;
        private IMemoryCache cache;
        private MemoryCacheEntryOptions cacheOptions;

        public PageController(
            Dm5 dm5,
            IHostingEnvironment hostEnvironment,
            ComicApiRepository repository,
            IMemoryCache memoryCache)
        {
            this.dm5 = dm5;
            env = hostEnvironment;
            repo = repository;
            cache = memoryCache;
            cacheOptions = new();
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(Config.CacheMinute));
        }

        [HttpGet("{page}/{showAll:bool?}")]
        public async Task<IActionResult> ShowComics(int page, bool? showAll)
        {
            var pagination = await this.GetPagnitation(page);
            if (page > 1) this.GetPagnitation(page - 1);
            if (page < 300) this.GetPagnitation(page + 1);

            var userId = Request.Cookies["userid"] ?? "";
            var ignoreComics = await repo.GetIgnoreComics(userId);
            var filteredComics = pagination.Comics;
            if (!(true == showAll || string.IsNullOrEmpty(userId)))
            {
                filteredComics = pagination.Comics.Where(c => false == ignoreComics.ContainsKey(c.Url.GetUrlDirectoryName())).ToList();
            }

            return View(new PageModel(dm5.GetRoot().Paginations, pagination, filteredComics, ignoreComics));
        }

        private async Task<ComicPagination> GetPagnitation(int page)
        {
            var key = $"Pagination_{page}";
            if (!cache.TryGetValue(key, out ComicPagination pagination))
            {
                pagination = dm5.GetRoot().Paginations[page - 1];
                await dm5.LoadComicsForWeb(pagination);
                pagination.Comics.ForEach(c => repo.SaveComic(c));
                cache.Set(key, pagination, cacheOptions);
            }

            if (pagination.ListState != ComicState.ListLoaded)
            {
                pagination.ListState = ComicState.ListLoaded;
                await dm5.LoadComicsForWeb(pagination);
                pagination.Comics.ForEach(c => repo.SaveComic(c));
                cache.Set(key, pagination, cacheOptions);
            }

            return pagination;
        }
    }
}

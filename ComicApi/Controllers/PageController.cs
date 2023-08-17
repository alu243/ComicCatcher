using System.Text;
using ComicApi.Model;
using ComicApi.Model.Repositories;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
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

        [HttpGet("{page}")]
        public async Task<IActionResult> ShowComics(int page = 1)
        {
            var pagination = await this.GetPagnitation(page);
            if (page > 1) this.GetPagnitation(page - 1);
            if (page < 300) this.GetPagnitation(page + 1);

            return View(new PageModel() { Paginations = dm5.GetRoot().Paginations, CurrPagination = pagination });
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

            return pagination;
        }
    }


}

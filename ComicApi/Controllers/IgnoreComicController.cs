﻿using ComicApi.Model.Repositories;
using ComicApi.Model.Requests;
using ComicCatcherLib.ComicModels.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IgnoreComicController : Controller
    {
        private Dm5 dm5;
        private IHostingEnvironment env;
        private ComicApiRepository repo;
        private IMemoryCache cache;
        private MemoryCacheEntryOptions cacheOptions;

        public IgnoreComicController(
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

        [HttpPost]
        public async Task<IActionResult> AddIgnoreComics(IgnoreComicRequest request)
        {
            try
            {
                request.UserId = Request.Cookies["userid"] ?? "";
                await repo.AddIgnoreComic(request);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteIgnoreComics(IgnoreComicRequest request)
        {
            request.UserId = Request.Cookies["userid"] ?? "";
            await repo.DeleteIgnoreComic(request);
            return Ok();
        }
    }
}

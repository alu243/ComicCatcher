﻿using ComicApi.Model;
using ComicApi.Model.Repositories;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Controllers
{
    public class ComicApplication
    {
        private Dm5 dm5;
        //private IHostingEnvironment env;
        private ComicApiRepository repo;
        private IMemoryCache cache;
        private MemoryCacheEntryOptions cacheOptions;

        public ComicApplication(Dm5 comic,
            //IHostingEnvironment hostEnvironment,
            ComicApiRepository repository,
            IMemoryCache memoryCache)
        {
            dm5 = comic;
            //env = hostEnvironment;
            repo = repository;
            cache = memoryCache;
            cacheOptions = new();
            cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(Config.CacheMinute));
        }

        ////////public async Task<ResponseModel> DownloadComicChapter(string comic, string chapter)
        ////////{
        ////////    var comicChapter = new ComicChapter()
        ////////    {
        ////////        Caption = "",
        ////////        Pages = new List<ComicPage>(),
        ////////        Url = new Uri(new Uri(new Uri(dm5.GetRoot().Url), comic), chapter).ToString()
        ////////    };
        ////////    await dm5.LoadPages(comicChapter);
        ////////    await dm5.DownloadChapter(new DownloadChapterRequest()
        ////////    {
        ////////        Path = Path.Combine(env.WebRootPath, Config.ComicPath, comic, chapter),
        ////////        Chapter = comicChapter,
        ////////        Name = "",
        ////////        ReportProgressAction = null,
        ////////    });

        ////////    //var chapterLocalPath = ApiChapterDao.SaveChapter(comic, chapter);
        ////////    return ResponseModel.Ok();
        ////////}

        ////////[HttpGet("{comic}/{chapter}/{pageFile}")]
        ////////public FileResult GetComicPage(string comic, string chapter, string pageFile)
        ////////{
        ////////    var path = Path.Combine(env.WebRootPath,
        ////////        Config.ComicPath,
        ////////        comic,
        ////////        chapter,
        ////////        pageFile);
        ////////    return new FileStreamResult(new FileStream(path, FileMode.Open), "image/jpeg");
        ////////}

        public async Task<ComicEntity> GetComic(string comic)
        {
            string key = $"comic_{comic}";
            if (!cache.TryGetValue(key, out ComicEntity comicEntity))
            {
                // 因為 comicentity 會有最後更新時間，所以這個方法不從資料庫拿出來
                var url = new Uri(new Uri(dm5.GetRoot().Url), comic).ToString();
                comicEntity = await dm5.GetSingleComicName(url);
                repo.SaveComic(comicEntity);

                if (comicEntity == null) return null;

                cache.Set(key, comicEntity, cacheOptions);
            }
            return comicEntity;
        }

        public async Task<ComicChapter> GetComicChapter(string comic, string chapter)
        {
            string key = $"comic_{comic}_chatper_{chapter}";
            if (!cache.TryGetValue(key, out ComicChapter comicChapter))
            {
                comicChapter = await repo.GetComicChapter(comic, chapter);
                if (comicChapter == null) comicChapter = await this.GetChapterFromComic(comic, chapter);
                if (comicChapter == null) return null;
                repo.SaveComicChapter(comic, chapter, comicChapter);
                cache.Set(key, comicChapter, cacheOptions);
            }
            return comicChapter;
        }

        private async Task<ComicChapter> GetChapterFromComic(string comic, string chapter)
        {
            var comicEntity = await this.GetComic(comic);
            await dm5.LoadChapters(comicEntity);
            var comicChapter = comicEntity.Chapters.FirstOrDefault(c => new Uri(c.Url).LocalPath.Trim('/').Equals(chapter));
            return comicChapter;
        }

        public async Task<List<ComicPage>> GetComicPages(string comic, string chapter)
        {
            string key = $"comic_{comic}_chatper_{chapter}_pages";
            if (!cache.TryGetValue(key, out List<ComicPage> comicPages) || comicPages.Count <= 0)
            {
                comicPages = await repo.GetComicPages(comic, chapter);
                if (comicPages.Count <= 0)
                {
                    var comicChapter = await this.GetComicChapter(comic, chapter);
                    comicChapter.ListState = ComicState.Created;
                    await dm5.LoadPages(comicChapter);
                    comicPages.AddRange(comicChapter.Pages);
                }
                repo.SaveComicPages(comic, chapter, comicPages);
                cache.Set(key, comicPages, cacheOptions);
            }
            return comicPages;
        }

        public async Task<List<ComicPage>> ReloadComicPages(string comic, string chapter)
        {
            await this.repo.DeleteComicPages(comic, chapter);
            return await this.GetComicPages(comic, chapter);
        }

    }
}

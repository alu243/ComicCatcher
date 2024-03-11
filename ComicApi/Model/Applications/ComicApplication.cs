﻿using System.Collections;
using ComicApi.Model;
using ComicApi.Model.Repositories;
using ComicApi.Model.Requests;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using ComicCatcherLib.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace ComicApi.Controllers
{
    public class ComicApplication
    {
        private Dm5 dm5;
        //private IHostingEnvironment env;
        private ComicApiRepository repo;
        private IMemoryCache cache;
        private MemoryCacheEntryOptions cacheOptions;
        private MemoryCacheEntryOptions pageCacheOptions;

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
            pageCacheOptions = new();
            pageCacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(Config.PageCacheMinute));
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

        public int GetPagnationNumber(string comic)
        {
            for (int i = 1; i <= 300; i++)
            {
                var key = $"Pagination_{i}";
                if (!cache.TryGetValue(key, out ComicPagination pagination)) continue;
                var comicEntity = pagination.Comics?.FirstOrDefault(c => comic.Equals(c.Url.GetUrlDirectoryName(), StringComparison.CurrentCultureIgnoreCase));
                if (comicEntity == null) continue;
                return i;
            }

            return 1;
        }

        public async Task<ComicPagination> GetPagnitation(int page)
        {
            var key = $"Pagination_{page}";


            if (!cache.TryGetValue(key, out ComicPagination pagination))
            {
                pagination = dm5.GetRoot().Paginations[page - 1];
                pagination.ListState = ComicState.Created;
                await dm5.LoadComicsForWeb(pagination);
                repo.SaveComics(pagination.Comics);
                cache.Set(key, pagination, cacheOptions);
            }

            if (pagination.ListState != ComicState.ListLoaded)
            {
                pagination.ListState = ComicState.Created;
                await dm5.LoadComicsForWeb(pagination);
                repo.SaveComics(pagination.Comics);
                cache.Set(key, pagination, cacheOptions);
            }

            return pagination;
        }

        public async Task<ComicEntity> GetComic(string comic)
        {
            string key = $"comic_{comic}";
            if (!cache.TryGetValue(key, out ComicEntity comicEntity))
            {
                // 因為 comicentity 會有最後更新時間，所以這個方法不從資料庫拿出來
                var url = new Uri(new Uri(dm5.GetRoot().Url), comic).ToString();
                comicEntity = await dm5.GetSingleComicName(url);
                repo.SaveComics(new List<ComicEntity>() { comicEntity });

                if (comicEntity == null) return null;

                cache.Set(key, comicEntity, cacheOptions);
            }
            return comicEntity;
        }

        public async Task<ComicChapter> GetComicChapterWithPage(string comic, string chapter)
        {
            var comicChapter = await this.GetComicChapter(comic, chapter);
            comicChapter.Pages = await this.GetComicPages(comic, chapter);
            return comicChapter;
        }

        private async Task<ComicChapter> GetComicChapter(string comic, string chapter)
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

        public async Task<string> GetNextChapter(ComicEntity comicEntity, string chapter)
        {
            if (comicEntity.Chapters.Count <= 0)
            {
                comicEntity.ListState = ComicState.Created;
                await dm5.LoadChapters(comicEntity);
            }
            var newChapterIndex = comicEntity.Chapters.FindIndex(c => c.Url.GetUrlDirectoryName().Equals(chapter, StringComparison.CurrentCultureIgnoreCase));

            if (newChapterIndex <= 0) return string.Empty;
            return comicEntity.Chapters[newChapterIndex - 1].Url.GetUrlDirectoryName();
        }


        private async Task<ComicChapter> GetChapterFromComic(string comic, string chapter)
        {
            var comicEntity = await this.GetComic(comic);
            await dm5.LoadChapters(comicEntity);
            var comicChapter = comicEntity.Chapters.FirstOrDefault(c => new Uri(c.Url).LocalPath.Trim('/').Equals(chapter));
            return comicChapter;
        }

        private async Task<List<ComicPage>> GetComicPages(string comic, string chapter)
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
                    repo.SaveComicPages(comic, chapter, comicPages);
                }
                else
                {
                    // compare if dm5 has new comicPage's data
                    Task.Run(async () => await CompareComicPagesBG(comic, chapter, comicPages));
                }
                cache.Set(key, comicPages, pageCacheOptions);
            }
            return comicPages;
        }

        private async Task CompareComicPagesBG(string comic, string chapter, List<ComicPage> dbPages)
        {
            var comicChapter = await this.GetComicChapter(comic, chapter);
            comicChapter.ListState = ComicState.Created;
            await dm5.LoadPages(comicChapter);
            var newPages = comicChapter.Pages;

            if (newPages.Count != dbPages.Count ||
                newPages.FirstOrDefault()?.Url != dbPages.FirstOrDefault()?.Url ||
                newPages.LastOrDefault()?.Url != dbPages.LastOrDefault()?.Url)
            {
                repo.SaveComicPages(comic, chapter, newPages);
            }
        }

        public async Task<ComicChapter> GetChapterAndReloadComicPages(string comic, string chapter)
        {
            var comicChapter = await this.GetComicChapter(comic, chapter);
            comicChapter.Pages.Clear();
            await this.repo.DeleteComicPages(comic, chapter);
            comicChapter.Pages = await this.GetComicPages(comic, chapter);
            return comicChapter;
        }

        public async Task<List<ComicViewModel>> GetComicsAreFavorite(string userId, int? level)
        {
            var comics = await this.repo.GetComicsAreFavorite(userId);
            // nullComics = 有記 favorite 但沒有記 comic
            var nullComics = comics.Where(c => string.IsNullOrEmpty(c.Url)).ToList();
            var comicEntities = new List<ComicEntity>();
            foreach (var nullComic in nullComics)
            {
                var comicEntity = await dm5.GetSingleComicName($"{dm5.GetRoot().Url}{nullComic.Comic}/");
                nullComic.Caption = comicEntity.Caption;
                nullComic.IconUrl = comicEntity.IconUrl;
                nullComic.Url = comicEntity.Url;
                nullComic.LastUpdateChapter = comicEntity.LastUpdateChapter;
                nullComic.LastUpdateDate = comicEntity.LastUpdateDate;
                //nullComic.Comic = nullComic.Comic;
                //nullComic.IsFavorite = true;
                //nullComic.IsIgnore = false;
                //nullComic.ReadedChapter = nullComic.ReadedChapter;
                comicEntities.Add(comicEntity);
            }
            this.repo.SaveComics(comicEntities);

            if (level > 0)
            {
                comics = comics.Where(c => c.Level == level).ToList();
            }
            return comics;
        }

        public async Task<List<ComicViewModel>> RefreshAllComicsAreFavorite()
        {
            var comics = await this.repo.GetAllComicsAreFavorite();
            var comicEntities = new List<ComicEntity>();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {comics.Count} comics are start refresh");
            foreach (var comic in comics)
            {
                var comicEntity = await dm5.GetSingleComicName($"{dm5.GetRoot().Url}{comic.Comic}/");
                await dm5.LoadChapters(comicEntity);
                string key = $"comic_{comic.Comic}";
                MemoryCacheEntryOptions options = new();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(80));
                cache.Remove(key);
                cache.Set(key, comicEntity, options);
                comicEntities.Add(comicEntity);
            }
            await this.repo.SaveComics(comicEntities, true);
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {comics.Count} comics are refreshed");
            Task.Run(async () => await this.RefreshAllUnReadedChapters(comics, comicEntities));
            //this.RefreshAllUnReadedChapters(comics, comicEntities);
            return comics;
        }

        private async Task RefreshAllUnReadedChapters(List<ComicViewModel> comics, List<ComicEntity> comicEntities)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {comicEntities.Count} comics are start cache unreaded chapters");
            int count = 0;
            foreach (var comic in comics)
            {
                var comicEntity = comicEntities.FirstOrDefault(e => e.Url.Contains(comic.Comic));
                if (comicEntity == null) { continue; }

                foreach (var chapterEntity in comicEntity.Chapters)
                {
                    var chapterList = chapterEntity.Url.Split('/');
                    if (chapterList.Length <= 3) continue;
                    var chapter = chapterList[3];
                    if (chapter.Equals(comic.ReadedChapter ?? "")) break;

                    await this.GetComicPages(comic.Comic, chapter);
                    count++;
                }
            }
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {count} chapters of comic are cached unreaded chapters");
        }

        public async Task<bool> AddFavoriteComic(FavoriteComic request)
        {
            return await this.repo.AddFavoriteComic(request);
        }

        public async Task<bool> UpdateFavoriteComicLevel(FavoriteComicLevel request)
        {
            return await this.repo.UpdateFavoriteComicLevel(request);
        }

        public async Task<bool> DeleteFavoriteComic(FavoriteComic request)
        {
            return await this.repo.DeleteFavoriteComic(request);
        }

        public async Task<Dictionary<string, string>> GetIgnoreComics(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return new Dictionary<string, string>();
            var dic = await this.repo.GetIgnoreComics(userId);
            return dic;
        }

        public async Task<Dictionary<string, string>> GetFavoriteComicDic(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return new Dictionary<string, string>();
            var list = await this.repo.GetFavoriteComics(userId);
            var dic = new Dictionary<string, string>();
            foreach (var item in list)
            {
                dic.TryAdd(item.Comic, item.ComicName);
            }

            return dic;
        }

        public async Task<bool> AddFavoriteChapter(FavoriteChapter request)
        {
            return await this.repo.AddFavoriteChapter(request);
        }

        public async Task<FavoriteChapter> GetFavoriteChapter(string userId, string comic)
        {
            return await this.repo.GetFavoriteChapter(userId, comic);
        }

        public async Task<List<FavoriteChapter>> GetFavoriteChapters(string userId)
        {
            return await this.repo.GetFavoriteChapters(userId);
        }
    }
}


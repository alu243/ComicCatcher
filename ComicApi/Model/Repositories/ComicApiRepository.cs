﻿using ComicApi.Model.Requests;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.DbModel;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Model.Repositories;

public class ComicApiRepository
{
    public ComicApiRepository(IHostingEnvironment hostEnvironment)
    {
        var env = hostEnvironment;
        ApiSQLiteHelper.SetDbPath(env.WebRootPath);
        this.CreateComicOnFly().Wait();
        this.CreateApiChapterOnFly().Wait();
        this.CreateIgnoreComicOnFly().Wait();
        this.CreateFavoriteComicOnFly().Wait();
        this.CreateFavoriteChapterOnFly().Wait();
    }

    #region Comic
    private async Task CreateComicOnFly()
    {
        await ApiSQLiteHelper.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS ApiComic(
Comic NVARCHAR(200) not NULL,
Caption NVARCHAR(200) not NULL,
Url NVARCHAR(200) not NULL,
IconUrl NVARCHAR(200) not NULL,
ListState INT no NULL,
LastUpdateChapter NVARCHAR(100),
LastUpdateDate NVARCHAR(100),
UNIQUE (Comic) ON CONFLICT REPLACE
);");
    }

    public async Task<ComicEntity> GetComic(string comic)
    {
        var sql = $"SELECT * FROM ApiComic WHERE Comic = '{comic}'";
        var table = await ApiSQLiteHelper.GetTable(sql);
        if (table.Rows.Count <= 0) return null;
        return new ComicEntity()
        {
            Url = table.Rows[0].GetValue<string>("Url"),
            ImageState = ComicState.Created,
            Caption = table.Rows[0].GetValue<string>("Caption"),
            Chapters = new List<ComicChapter>(),
            IconImage = null,
            IconUrl = table.Rows[0].GetValue<string>("IconUrl"),
            ListState = (ComicState)table.Rows[0].GetValue<int>("ListState"),
            LastUpdateChapter = table.Rows[0].GetValue<string>("LastUpdateChapter"),
            LastUpdateDate = table.Rows[0].GetValue<string>("LastUpdateDate")
        };
    }

    public async Task<bool> SaveComics(List<ComicEntity> comics)
    {
        var result = 0;
        foreach (var comic in comics)
        {
            var comicUrl = (new Uri(comic.Url)).LocalPath.Trim('/');
            var sql = $@"INSERT OR REPLACE INTO ApiComic (Comic,Caption,Url,IconUrl, ListState, LastUpdateChapter, LastUpdateDate) VALUES 
('{comicUrl}','{comic.Caption}','{comic.Url}','{comic.IconUrl}', {(int)comic.ListState}, '{comic.LastUpdateChapter}', '{comic.LastUpdateDate}');";
            result += await ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
        return result == comics.Count;
    }


    public async Task<bool> SaveComic(ComicEntity comic)
    {
        var comicUrl = (new Uri(comic.Url)).LocalPath.Trim('/');
        var sql = $@"INSERT OR REPLACE INTO ApiComic (Comic,Caption,Url,IconUrl, ListState, LastUpdateChapter, LastUpdateDate) VALUES 
('{comicUrl}','{comic.Caption}','{comic.Url}','{comic.IconUrl}', {(int)comic.ListState}, '{comic.LastUpdateChapter}', '{comic.LastUpdateDate}');";
        var result = await ApiSQLiteHelper.ExecuteNonQuery(sql);
        return result > 0;
    }
    #endregion

    #region Chapter
    private async Task CreateApiChapterOnFly()
    {
        await ApiSQLiteHelper.ExecuteNonQuery(@"
BEGIN;
CREATE TABLE IF NOT EXISTS ApiChapter(
Comic NVARCHAR(200) not NULL,
Chapter NVARCHAR(200) not NULL,
Caption NVARCHAR(200) not NULL,
Url NVARCHAR(200) not NULL);
CREATE UNIQUE INDEX IF NOT EXISTS ux_ApiChapter ON ApiChapter (Comic, Chapter);
COMMIT;");
        await ApiSQLiteHelper.ExecuteNonQuery(@"
BEGIN;
CREATE TABLE IF NOT EXISTS ApiPage(
Comic NVARCHAR(200) not NULL,
Chapter NVARCHAR(200) not NULL,
PageNumber INT not NULL,
Caption NVARCHAR(200) not NULL,
Url NVARCHAR(200) not NULL,
PageFileName NVARCHAR(20) not NULL,
Refer NVARCHAR(200) not NULL);
CREATE UNIQUE INDEX IF NOT EXISTS ux_ApiPage ON ApiPage (Comic, Chapter, PageNumber);
COMMIT;");
    }

    public async Task<ComicChapter> GetComicChapter(string comic, string chapter)
    {
        var sql = $"SELECT * FROM ApiChapter WHERE Comic = '{comic}' AND Chapter = '{chapter}'";
        var table = await ApiSQLiteHelper.GetTable(sql);
        if (table.Rows.Count <= 0) return null;
        var comicChapter = new ComicChapter()
        {
            Url = table.Rows[0].GetValue<string>("Url"),
            Caption = table.Rows[0].GetValue<string>("Caption"),
            ListState = ComicState.Created
        };
        return comicChapter;
    }

    public async Task<List<ComicPage>> GetComicPages(string comic, string chapter)
    {
        var sql = $"SELECT * FROM ApiPage WHERE Comic = '{comic}' AND Chapter = '{chapter}' ORDER BY PageNumber";
        await ApiSQLiteHelper.ExecuteNonQuery(sql);

        var table = await ApiSQLiteHelper.GetTable(sql);

        var pages = new List<ComicPage>();
        foreach (var row in table.Rows)
        {
            var page = new ComicPage()
            {
                Url = table.Rows[0].GetValue<string>("Url"),
                Caption = table.Rows[0].GetValue<string>("Caption"),
                PageFileName = table.Rows[0].GetValue<string>("PageFileName"),
                PageNumber = table.Rows[0].GetValue<int>("PageNumber"),
                Refer = table.Rows[0].GetValue<string>("Refer"),
            };
            pages.Add(page);
        }
        return pages;
    }

    public async Task<bool> SaveComicChapter(string comic, string chapter, ComicChapter comicChapter)
    {
        string sql = $@"INSERT OR REPLACE INTO ApiChapter (Comic, Chapter, Caption, Url) VALUES
    ('{comic}', '{chapter}', '{comicChapter.Caption}', '{comicChapter.Url}');";
        var result = await ApiSQLiteHelper.ExecuteNonQuery(sql);
        return result > 0;
    }

    public async Task<bool> DeleteComicPages(string comic, string chapter)
    {
        var sql = $"DELETE FROM ApiPage WHERE Comic = '{comic}' AND Chapter = '{chapter}'";
        var result = await ApiSQLiteHelper.ExecuteNonQuery(sql);
        return result > 0;
    }

    public async Task<bool> SaveComicPages(string comic, string chapter, List<ComicPage> pages)
    {
        var sql = $"DELETE FROM ApiPage WHERE Comic = '{comic}' AND Chapter = '{chapter}'";
        await ApiSQLiteHelper.ExecuteNonQuery(sql);
        var result = 0;
        foreach (var page in pages)
        {
            sql = $@"INSERT OR REPLACE INTO ApiComic (Comic, Chapter, PageNumber, Caption, Url, PageFileName, Refer) VALUES
    ('{comic}', '{chapter}', {page.PageNumber}, '{page.Caption}', '{page.Url}', '{page.PageFileName}', '{page.Refer}');";
            result += await ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
        return result > 0;
    }

    public async Task<string> GetLocalPath(string comic, string chapter)
    {
        var sql = "SELECT LocalPath AS cnt FROM ApiChapter WHERE Comic = @Comic AND Chapter = @Chapter";
        var path = await ApiSQLiteHelper.ExecuteScalar<string>(sql);
        return path;
    }
    #endregion

    #region UserProfiles
    private async Task CreateIgnoreComicOnFly()
    {
        await ApiSQLiteHelper.ExecuteNonQuery(@"
BEGIN;
CREATE TABLE IF NOT EXISTS UserIgnoreComic(
UserId NVARCHAR(20) not NULL,
Comic NVARCHAR(200) not NULL,
ComicName NVARCHAR(50) not NULL);
CREATE UNIQUE INDEX IF NOT EXISTS ux_UserIgnoreComic ON UserIgnoreComic (UserId, Comic);
COMMIT;");
    }
    public async Task<Dictionary<string, string>> GetIgnoreComics(string userId)
    {
        var sql = $"SELECT * FROM UserIgnoreComic WHERE UserId = '{userId}'";
        var result = await ApiSQLiteHelper.GetTable(sql);
        var dic = new Dictionary<string, string>();
        foreach (DataRow row in result.Rows)
        {
            string url = row.GetValue<string>("Comic")?.Trim();
            string name = row.GetValue<string>("ComicName")?.Trim();
            dic.TryAdd(url, name);
        }

        return dic;
    }
    public async Task AddIgnoreComic(IgnoreComicRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return;
        var sql = $"INSERT OR REPLACE INTO UserIgnoreComic (UserId, Comic, ComicName) VALUES ('{request.UserId}', '{request.Comic}', '{request.ComicName}')";
        await ApiSQLiteHelper.ExecuteNonQuery(sql);
    }
    public async Task DeleteIgnoreComic(IgnoreComicRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return;

        var sql = $"DELETE FROM UserIgnoreComic WHERE UserId = '{request.UserId}' AND Comic = '{request.Comic}'";
        await ApiSQLiteHelper.GetTable(sql);
    }
    public async Task ClearIgnoreComics(string userId)
    {
        var sql = $"DELETE FROM UserIgnoreComic WHERE UserId = '{userId}'";
        await ApiSQLiteHelper.GetTable(sql);
    }
    #endregion

    #region Favorite
    private async Task CreateFavoriteComicOnFly()
    {
        await ApiSQLiteHelper.ExecuteNonQuery(@"
BEGIN;
CREATE TABLE IF NOT EXISTS UserFavoriteComic(
UserId NVARCHAR(20) not NULL,
Comic NVARCHAR(200) not NULL,
ComicName NVARCHAR(50) not NULL,
IconUrl NVARCHAR(200) not NULL);
CREATE UNIQUE INDEX IF NOT EXISTS ux_UserFavoriteComic ON UserFavoriteComic (UserId, Comic);
COMMIT;");
    }

    private async Task CreateFavoriteChapterOnFly()
    {
        await ApiSQLiteHelper.ExecuteNonQuery(@"
BEGIN;
CREATE TABLE IF NOT EXISTS UserFavoriteChapter(
UserId NVARCHAR(20) not NULL,
Comic NVARCHAR(200) not NULL,
Chapter NVARCHAR(200) not NULL,
ChapterName NVARCHAR(50) not NULL);
CREATE UNIQUE INDEX IF NOT EXISTS ux_UserFavoriteChapter ON UserFavoriteChapter (UserId, Comic);
COMMIT;");
    }

    public async Task<bool> AddFavoriteComic(FavoriteComic request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return false;
        var sql = $"INSERT OR REPLACE INTO UserFavoriteComic (UserId, Comic, ComicName, IconUrl) VALUES ('{request.UserId}', '{request.Comic}', '{request.ComicName}', '{request.IconUrl}')";
        var result = await ApiSQLiteHelper.ExecuteNonQuery(sql);
        return result > 0;
    }

    public async Task<bool> DeleteFavoriteComic(FavoriteComic request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return false;

        var sql = $"DELETE FROM UserFavoriteComic WHERE UserId = '{request.UserId}' AND Comic = '{request.Comic}'";
        var result = await ApiSQLiteHelper.ExecuteNonQuery(sql);

        sql = $"DELETE FROM UserFavoriteChapter WHERE UserId = '{request.UserId}' AND Comic = '{request.Comic}'";
        result += await ApiSQLiteHelper.ExecuteNonQuery(sql);
        return result > 0;
    }

    public async Task<List<FavoriteComic>> GetFavoriteComics(string userId)
    {
        var sql = $"SELECT * FROM UserFavoriteComic WHERE UserId = '{userId}'";
        var result = await ApiSQLiteHelper.GetTable(sql);
        var list = new List<FavoriteComic>();
        foreach (DataRow row in result.Rows)
        {
            var favorite = new FavoriteComic()
            {
                Comic = row.GetValue<string>("Comic")?.Trim(),
                IconUrl = row.GetValue<string>("IconUrl")?.Trim(),
                ComicName = row.GetValue<string>("ComicName")?.Trim(),
                UserId = row.GetValue<string>("UserId")?.Trim(),
            };
            list.Add(favorite);
        }
        return list;
    }

    public async Task<bool> AddFavoriteChapter(FavoriteChapter request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return false;
        var sql = $@"INSERT OR REPLACE INTO UserFavoriteChapter (UserId, Comic, Chapter, ChapterName) VALUES 
            ('{request.UserId}', '{request.Comic}', '{request.Chapter}', '{request.ChapterName}')";
        var result = await ApiSQLiteHelper.ExecuteNonQuery(sql);
        return result > 0;
    }

    public async Task<FavoriteChapter> GetFavoriteChapter(string userId, string comic)
    {
        var sql = $"SELECT * FROM UserFavoriteChapter WHERE UserId = '{userId}' AND comic = '{comic}'";
        var result = await ApiSQLiteHelper.GetTable(sql);
        var list = new List<FavoriteChapter>();
        foreach (DataRow row in result.Rows)
        {
            var favorite = new FavoriteChapter()
            {
                Comic = row.GetValue<string>("Comic")?.Trim(),
                Chapter = row.GetValue<string>("Chapter")?.Trim(),
                ChapterName = row.GetValue<string>("ChapterName")?.Trim(),
                UserId = row.GetValue<string>("UserId")?.Trim(),
            };
            list.Add(favorite);
        }
        return list.FirstOrDefault();
    }


    public async Task<List<FavoriteChapter>> GetFavoriteChapters(string userId)
    {
        var sql = $"SELECT * FROM UserFavoriteChapter WHERE UserId = '{userId}'";
        var result = await ApiSQLiteHelper.GetTable(sql);
        var list = new List<FavoriteChapter>();
        foreach (DataRow row in result.Rows)
        {
            var favorite = new FavoriteChapter()
            {
                Comic = row.GetValue<string>("Comic")?.Trim(),
                Chapter = row.GetValue<string>("Chapter")?.Trim(),
                ChapterName = row.GetValue<string>("ChapterName")?.Trim(),
                UserId = row.GetValue<string>("UserId")?.Trim(),
            };
            list.Add(favorite);
        }
        return list;
    }
    #endregion

}
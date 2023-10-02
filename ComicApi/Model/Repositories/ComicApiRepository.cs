using ComicApi.Model.Requests;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.DbModel;
using System.Data;
using ComicCatcherLib.Utils;
using Quartz.Impl.Matchers;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using ComicCatcherLib.ComicModels.Domains;

namespace ComicApi.Model.Repositories;

public class ComicApiRepository
{
    public ComicApiRepository(IHostingEnvironment hostEnvironment)
    {
        var env = hostEnvironment;
        if (!Directory.Exists(Path.Combine(env.ContentRootPath, "db")))
            Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "db"));
        ApiSQLiteHelper.SetDbPath(Path.Combine(env.ContentRootPath, "db"));
        this.CreateComicOnFly().Wait();
        this.CreateComicLastUpdateChapterLinkOnFly().Wait();
        this.CreateApiChapterOnFly().Wait();
        this.CreateIgnoreComicOnFly().Wait();
        this.CreateFavoriteComicOnFly().Wait();
        this.CreateFavoriteComicLevelOnFly().Wait();
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
LastUpdateChapterLink NVARCHAR(100),
LastUpdateDate NVARCHAR(100),
UNIQUE (Comic) ON CONFLICT REPLACE
);");
    }

    private async Task CreateComicLastUpdateChapterLinkOnFly()
    {
        string sql = "SELECT LastUpdateChapterLink FROM ApiComic limit 1";
        try
        {
            await ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
        catch (Exception a)
        {
            sql = "ALTER TABLE ApiComic ADD LastUpdateChapterLink NVARCHAR(100);";
            await ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
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

    public async Task<bool> SaveComics(List<ComicEntity> comics, bool updateLastChapterLink = false)
    {
        var result = 0;

        await using var conn = await ApiSQLiteHelper.GetConnection();
        await using var cmd = conn.CreateCommand();
        await conn.OpenAsync();
        var tran = conn.BeginTransaction();
        cmd.Transaction = tran;
        string sql = "";
        try
        {
            foreach (var comic in comics)
            {
                var lastUpdateChapterLink = comic.Chapters?.FirstOrDefault()?.Url?.GetUrlDirectoryName() ?? "";
                var comicUrl = (new Uri(comic.Url)).LocalPath.Trim('/');
                if (updateLastChapterLink)
                {
                    sql = $@"INSERT OR REPLACE INTO ApiComic (Comic,Caption,Url,IconUrl, ListState, LastUpdateChapter, LastUpdateChapterLink, LastUpdateDate) VALUES 
('{comicUrl}','{comic.Caption}','{comic.Url}','{comic.IconUrl}', {(int)comic.ListState}, '{comic.LastUpdateChapter}', '{lastUpdateChapterLink}', '{comic.LastUpdateDate}');";
                }
                else
                {
                    sql = $@"INSERT OR REPLACE INTO ApiComic (Comic,Caption,Url,IconUrl, ListState, LastUpdateChapter, LastUpdateDate) VALUES 
('{comicUrl}','{comic.Caption}','{comic.Url}','{comic.IconUrl}', {(int)comic.ListState}, '{comic.LastUpdateChapter}', '{comic.LastUpdateDate}');";
                }
                cmd.CommandText = sql;
                result += await cmd.ExecuteNonQueryAsync();
            }
            await tran.CommitAsync();
        }
        catch (Exception e)
        {
            await tran.RollbackAsync();
            Console.WriteLine(e);
            throw;
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

    public async Task<bool> UpdateComicLastUpdateChapterLink(string comic, string lastUpdateChapterLink)
    {
        string sql = $@"UPDATE ApiComic SET LastUpdateChapterLink = '{lastUpdateChapterLink}' WHERE Comic = '{comic}';";
        return await ApiSQLiteHelper.ExecuteNonQuery(sql) > 0;
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
        await using var conn = await ApiSQLiteHelper.GetConnection();
        await using var cmd = conn.CreateCommand();
        await conn.OpenAsync();
        var tran = conn.BeginTransaction();
        cmd.Transaction = tran;
        try
        {
            var sql = $"DELETE FROM ApiPage WHERE Comic = '{comic}' AND Chapter = '{chapter}'";
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();

            var result = 0;
            foreach (var page in pages)
            {
                sql = $@"INSERT OR REPLACE INTO ApiComic (Comic, Chapter, PageNumber, Caption, Url, PageFileName, Refer) VALUES
    ('{comic}', '{chapter}', {page.PageNumber}, '{page.Caption}', '{page.Url}', '{page.PageFileName}', '{page.Refer}');";

                cmd.CommandText = sql;
                result += await cmd.ExecuteNonQueryAsync();
            }
            await tran.CommitAsync();
            return result == pages.Count;
        }
        catch (Exception e)
        {
            await tran.RollbackAsync();
            Console.WriteLine(e);
            throw;
        }
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
IconUrl NVARCHAR(200) not NULL,
Level INT not NULL DEFAULT 1);
CREATE UNIQUE INDEX IF NOT EXISTS ux_UserFavoriteComic ON UserFavoriteComic (UserId, Comic);
COMMIT;");
    }

    private async Task CreateFavoriteComicLevelOnFly()
    {
        string sql = "SELECT Level FROM UserFavoriteComic limit 1";
        try
        {
            await ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
        catch (Exception a)
        {
            sql = "ALTER TABLE UserFavoriteComic ADD Level INT NOT NULL DEFAULT 1;";
            await ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
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

    public async Task<bool> UpdateFavoriteComicLevel(FavoriteComicLevel request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return false;
        var sql = $"UPDATE UserFavoriteComic SET Level = {request.Level} WHERE UserId = '{request.UserId}' AND Comic = '{request.Comic}'";
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

    public async Task<List<FavoriteComic>> GetFavoriteComics(string userId, int? level = null)
    {
        var sql = $"SELECT * FROM UserFavoriteComic WHERE UserId = '{userId}'";
        if (level != null) sql += $" AND Level = {level}";
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

    public async Task<List<ComicViewModel>> GetComicsAreFavorite(string userId)
    {
        var sql = @$"SELECT distinct f.Comic,
                    f.Level,
                    IFNULL(c.Caption, '') Caption, 
                    IFNULL(c.Url, '') Url, 
                    IFNULL(c.IconUrl, '') IconUrl, 
                    IFNULL(c.ListState, 0) ListState, 
                    IFNULL(c.LastUpdateChapter, '') LastUpdateChapter,
                    IFNULL(c.LastUpdateChapterLink, '') LastUpdateChapterLink,
                    IFNULL(c.LastUpdateDate, '') LastUpdateDate
                    FROM UserFavoriteComic f
                    LEFT JOIN ApiComic c on f.Comic = c.Comic WHERE f.UserId = '{userId}'
                    ORDER BY c.LastUpdateDate DESC";
        var results = await this.GetComicViews(sql);
        var chapters = await this.GetFavoriteChapters(userId);
        foreach (var comic in results)
        {
            var favoriteChapter = chapters.FirstOrDefault(chapter => chapter.Comic.Equals(comic.Comic, StringComparison.CurrentCultureIgnoreCase));
            comic.ReadedChapterLink = favoriteChapter?.Chapter ?? "";
            comic.ReadedChapter = favoriteChapter?.ChapterName ?? "";
        }
        return results;
    }

    public async Task<List<ComicViewModel>> GetAllComicsAreFavorite()
    {
        var sql = @$"SELECT distinct f.Comic, IFNULL(c.Caption, '') Caption, IFNULL(c.Url, '') Url, IFNULL(c.IconUrl, '') IconUrl, IFNULL(c.ListState, 0) ListState, 
                    IFNULL(c.LastUpdateChapter, '') LastUpdateChapter, IFNULL(c.LastUpdateDate, '') LastUpdateDate
                    FROM UserFavoriteComic f
                    LEFT JOIN ApiComic c on f.Comic = c.Comic
                    ORDER BY c.LastUpdateDate DESC";
        var results = await this.GetComicViews(sql);
        results = results.GroupBy(r => r.Comic).Select(g => g.First()).ToList();
        return results;
    }

    private async Task<List<ComicViewModel>> GetComicViews(string sql)
    {
        var result = await ApiSQLiteHelper.GetTable(sql);
        var list = new List<ComicViewModel>();
        foreach (DataRow row in result.Rows)
        {
            var comic = new ComicViewModel()
            {
                Level = row.GetValue<int>("Level"),
                Comic = row.GetValue<string>("Comic"),
                Url = row.GetValue<string>("Url"),
                Caption = row.GetValue<string>("Caption"),
                IconUrl = row.GetValue<string>("IconUrl"),
                LastUpdateChapter = row.GetValue<string>("LastUpdateChapter"),
                LastUpdateDate = row.GetValue<string>("LastUpdateDate"),
                LastUpdateChapterLink = row.GetValue<string>("LastUpdateChapterLink"),
                IsFavorite = true,
                IsIgnore = false,
                ReadedChapter = null,
            };
            list.Add(comic);
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
using ComicApi.Model.Requests;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.DbModel;
using ComicCatcherLib.Utils;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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
        await ApiSQLiteHelper.ExecuteNonQueryAsync(@"
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
            await ApiSQLiteHelper.ExecuteNonQueryAsync(sql);
        }
        catch (Exception a)
        {
            sql = "ALTER TABLE ApiComic ADD LastUpdateChapterLink NVARCHAR(100);";
            await ApiSQLiteHelper.ExecuteNonQueryAsync(sql);
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

    private static object lockObj = new object();
    public async Task<bool> SaveComics(List<ComicEntity> comics, bool updateLastChapterLink = false)
    {
        var result = 0;

        await using var conn = await ApiSQLiteHelper.GetConnection();
        await using var cmd = conn.CreateCommand();
        string sql = "";
        lock (lockObj)
        {
            try
            {
                conn.Open();
                int i = 0;
                int size = 20;
                var batch = comics.Skip(i * size).ToList();
                while (batch.Count > 0)
                {
                    sql = "";
                    foreach (var comic in batch)
                    {
                        var lastUpdateChapterLink = comic.Chapters?.FirstOrDefault()?.Url?.GetUrlDirectoryName() ?? "";
                        var comicUrl = (new Uri(comic.Url)).LocalPath.Trim('/');
                        if (updateLastChapterLink)
                        {
                            sql += $@"INSERT INTO ApiComic (Comic,Caption,Url,IconUrl, ListState, LastUpdateChapter, LastUpdateChapterLink, LastUpdateDate) VALUES 
('{comicUrl}','{comic.Caption}','{comic.Url}','{comic.IconUrl}', {(int)comic.ListState}, '{comic.LastUpdateChapter}', '{lastUpdateChapterLink}', '{comic.LastUpdateDate}')
ON CONFLICT(Comic) DO UPDATE SET 
Caption = '{comic.Caption}',Url = '{comic.Url}', IconUrl = '{comic.IconUrl}',ListState = {(int)comic.ListState}, LastUpdateChapter = '{comic.LastUpdateChapter}', LastUpdateChapterLink = '{lastUpdateChapterLink}', LastUpdateDate = '{comic.LastUpdateDate}';";
                        }
                        else
                        {
                            sql += $@"INSERT INTO ApiComic (Comic,Caption,Url,IconUrl, ListState, LastUpdateChapter, LastUpdateDate) VALUES 
('{comicUrl}','{comic.Caption}','{comic.Url}','{comic.IconUrl}', {(int)comic.ListState}, '{comic.LastUpdateChapter}', '{comic.LastUpdateDate}')
ON CONFLICT(Comic) DO UPDATE SET 
Caption = '{comic.Caption}',Url = '{comic.Url}', IconUrl = '{comic.IconUrl}',ListState = {(int)comic.ListState}, LastUpdateChapter = '{comic.LastUpdateChapter}', LastUpdateDate = '{comic.LastUpdateDate}';";
                        }
                    }

                    cmd.CommandText = sql;
                    result += cmd.ExecuteNonQuery();

                    i++;
                    batch = comics.Skip(i * size).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
        return result == comics.Count;
    }

    public async Task<bool> UpdateComicLastUpdateChapterLink(string comic, string lastUpdateChapterLink)
    {
        string sql = $@"UPDATE ApiComic SET LastUpdateChapterLink = '{lastUpdateChapterLink}' WHERE Comic = '{comic}';";
        lock (lockObj)
        {
            var result = ApiSQLiteHelper.ExecuteNonQuery(sql);
            return result > 0;
        }
    }
    #endregion

    #region Chapter
    private async Task CreateApiChapterOnFly()
    {
        await ApiSQLiteHelper.ExecuteNonQueryAsync(@"
BEGIN;
CREATE TABLE IF NOT EXISTS ApiChapter(
Comic NVARCHAR(200) not NULL,
Chapter NVARCHAR(200) not NULL,
Caption NVARCHAR(200) not NULL,
Url NVARCHAR(200) not NULL);
CREATE UNIQUE INDEX IF NOT EXISTS ux_ApiChapter ON ApiChapter (Comic, Chapter);
COMMIT;");
        await ApiSQLiteHelper.ExecuteNonQueryAsync(@"
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
CREATE INDEX IF NOT EXISTS ix_ApiPages ON ApiPage (Comic, Chapter);
COMMIT;");
    }

    public async Task<ComicChapter> GetComicChapter(string comic, string chapter)
    {
        var sql = $"SELECT * FROM ApiChapter WHERE Comic = '{comic}' AND Chapter = '{chapter}'";
        var results= await ApiSQLiteHelper.GetList<ComicChapter>(sql);
        if (results.Count <= 0) return null;
        foreach (var item in results)
        {
            item.ListState = ComicState.Created;
        }
        return results.FirstOrDefault();
    }

    public async Task<List<ComicPage>> GetComicPages(string comic, string chapter)
    {
        var sql = $"SELECT * FROM ApiPage WHERE Comic = '{comic}' AND Chapter = '{chapter}'";
        //var table = await ApiSQLiteHelper.GetListLog(sql);

        //var pages = new List<ComicPage>();
        //foreach (DataRow row in table.Rows)
        //{
        //    var page = new ComicPage()
        //    {
        //        Url = row.GetValue<string>("Url"),
        //        Caption = row.GetValue<string>("Caption"),
        //        PageFileName = row.GetValue<string>("PageFileName"),
        //        PageNumber = Convert.ToInt32(row.GetValue<long>("PageNumber")),
        //        Refer = row.GetValue<string>("Refer"),
        //    };
        //    pages.Add(page);
        //}
        var pages = await ApiSQLiteHelper.GetList<ComicPage>(sql);
        pages = pages.OrderBy(p => p.PageNumber).ToList();
        return pages;
    }

    public async Task<bool> SaveComicChapter(string comic, string chapter, ComicChapter comicChapter)
    {
        string sql = $@"INSERT OR REPLACE INTO ApiChapter (Comic, Chapter, Caption, Url) VALUES
            ('{comic}', '{chapter}', '{comicChapter.Caption}', '{comicChapter.Url}');";

        lock (lockObj)
        {
            var result = ApiSQLiteHelper.ExecuteNonQuery(sql);
            return result > 0;
        }
    }

    public async Task<bool> DeleteComicPages(string comic, string chapter)
    {
        var sql = $"DELETE FROM ApiPage WHERE Comic = '{comic}' AND Chapter = '{chapter}'";
        lock (lockObj)
        {
            var result = ApiSQLiteHelper.ExecuteNonQuery(sql);
            return result > 0;
        }
    }

    public async Task<bool> SaveComicPages(string comic, string chapter, List<ComicPage> pages)
    {
        await using var conn = await ApiSQLiteHelper.GetConnection();
        await using var cmd = conn.CreateCommand();
        lock (lockObj)
        {
            try
            {
                conn.Open();
                var sql = $"DELETE FROM ApiPage WHERE Comic = '{comic}' AND Chapter = '{chapter}'";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                var result = 0;

                int i = 0;
                int size = 20;
                var batch = pages.Skip(size * i).Take(size).ToList();
                while (batch.Count > 0)
                {
                    sql = "";
                    foreach (var page in batch)
                    {
                        sql += $@"INSERT OR REPLACE INTO ApiPage (Comic, Chapter, PageNumber, Caption, Url, PageFileName, Refer) VALUES
                        ('{comic}', '{chapter}', {page.PageNumber}, '{page.Caption}', '{page.Url}', '{page.PageFileName}', '{page.Refer}');";

                    }
                    cmd.CommandText = sql;
                    result += cmd.ExecuteNonQuery();

                    i++;
                    batch = pages.Skip(size * i).Take(size).ToList();
                }
                return result == pages.Count;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
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
        await ApiSQLiteHelper.ExecuteNonQueryAsync(@"
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
        var results = await ApiSQLiteHelper.GetList<UserIgnoreComic>(sql);
        var dic = new Dictionary<string, string>();
        foreach (var row in results)
        {
            string url = row.Comic.Trim();
            string name = row.ComicName.Trim();
            dic.TryAdd(url, name);
        }

        return dic;
    }
    public async Task AddIgnoreComic(IgnoreComicRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return;

        var sql = $"INSERT OR REPLACE INTO UserIgnoreComic (UserId, Comic, ComicName) VALUES ('{request.UserId}', '{request.Comic}', '{request.ComicName}')";
        lock (lockObj)
        {
            ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
    }
    public async Task DeleteIgnoreComic(IgnoreComicRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return;

        var sql = $"DELETE FROM UserIgnoreComic WHERE UserId = '{request.UserId}' AND Comic = '{request.Comic}'";
        lock (lockObj)
        {
            ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
    }
    public async Task ClearIgnoreComics(string userId)
    {
        var sql = $"DELETE FROM UserIgnoreComic WHERE UserId = '{userId}'";
        lock (lockObj)
        {
            ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
    }
    #endregion

    #region Favorite
    private async Task CreateFavoriteComicOnFly()
    {
        await ApiSQLiteHelper.ExecuteNonQueryAsync(@"
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
            await ApiSQLiteHelper.ExecuteNonQueryAsync(sql);
        }
        catch (Exception a)
        {
            sql = "ALTER TABLE UserFavoriteComic ADD Level INT NOT NULL DEFAULT 1;";
            await ApiSQLiteHelper.ExecuteNonQueryAsync(sql);
        }
    }
    private async Task CreateFavoriteChapterOnFly()
    {
        await ApiSQLiteHelper.ExecuteNonQueryAsync(@"
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
        lock (lockObj)
        {
            var result = ApiSQLiteHelper.ExecuteNonQuery(sql);
            return result > 0;
        }
    }

    public async Task<bool> UpdateFavoriteComicLevel(FavoriteComicLevel request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return false;

        var sql = $"UPDATE UserFavoriteComic SET Level = {request.Level} WHERE UserId = '{request.UserId}' AND Comic = '{request.Comic}'";
        lock (lockObj)
        {
            var result = ApiSQLiteHelper.ExecuteNonQuery(sql);
            return result > 0;
        }
    }

    public async Task<bool> DeleteFavoriteComic(FavoriteComic request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return false;

        var sql = $"DELETE FROM UserFavoriteComic WHERE UserId = '{request.UserId}' AND Comic = '{request.Comic}';";
        sql += $"DELETE FROM UserFavoriteChapter WHERE UserId = '{request.UserId}' AND Comic = '{request.Comic}';";
        lock (lockObj)
        {
            var result = ApiSQLiteHelper.ExecuteNonQuery(sql);
            return result > 0;
        }
    }

    public async Task<List<FavoriteComic>> GetFavoriteComics(string userId, int? level = null)
    {
        var sql = $"SELECT * FROM UserFavoriteComic WHERE UserId = '{userId}'";
        if (level != null) sql += $" AND Level = {level}";
        var list = await ApiSQLiteHelper.GetList<FavoriteComic>(sql);
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
                    IFNULL(c.LastUpdateDate, '') LastUpdateDate,
                    true IsFavorite,
                    false IsIgnore,
                    null ReadedChapter,
                    null ReadedChapterLink
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
        var sql = @$"SELECT distinct f.Comic,
                    f.Level,
                    IFNULL(c.Caption, '') Caption, 
                    IFNULL(c.Url, '') Url, 
                    IFNULL(c.IconUrl, '') IconUrl, 
                    IFNULL(c.ListState, 0) ListState, 
                    IFNULL(c.LastUpdateChapter, '') LastUpdateChapter, 
                    IFNULL(c.LastUpdateChapterLink, '') LastUpdateChapterLink,
                    IFNULL(c.LastUpdateDate, '') LastUpdateDate,
                    true IsFavorite,
                    false IsIgnore,
                    null ReadedChapter,
                    null ReadedChapterLink
                    FROM UserFavoriteComic f
                    LEFT JOIN ApiComic c on f.Comic = c.Comic
                    ORDER BY c.LastUpdateDate DESC";
        var results = await this.GetComicViews(sql);
        results = results.GroupBy(r => r.Comic).Select(g => g.First()).ToList();
        return results;
    }

    private async Task<List<ComicViewModel>> GetComicViews(string sql)
    {
        var list = await ApiSQLiteHelper.GetList<ComicViewModel>(sql);
        list.ForEach(l => l.ReadedChapter = null);
        list.ForEach(l => l.IsFavorite = true);
        return list;
    }

    public async Task<bool> AddFavoriteChapter(FavoriteChapter request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) return false;

        var sql = $@"INSERT OR REPLACE INTO UserFavoriteChapter (UserId, Comic, Chapter, ChapterName) VALUES 
            ('{request.UserId}', '{request.Comic}', '{request.Chapter}', '{request.ChapterName}')";
        lock (lockObj)
        {
            var result = ApiSQLiteHelper.ExecuteNonQuery(sql);
            return result > 0;
        }
    }

    public async Task<FavoriteChapter> GetFavoriteChapter(string userId, string comic)
    {
        var sql = $"SELECT * FROM UserFavoriteChapter WHERE UserId = '{userId}' AND comic = '{comic}'";
        var list = await ApiSQLiteHelper.GetList<FavoriteChapter>(sql);
        return list.FirstOrDefault();
    }


    public async Task<List<FavoriteChapter>> GetFavoriteChapters(string userId)
    {
        var sql = $"SELECT * FROM UserFavoriteChapter WHERE UserId = '{userId}'";
        var list = await ApiSQLiteHelper.GetList<FavoriteChapter>(sql);
        return list;
    }
    #endregion

}
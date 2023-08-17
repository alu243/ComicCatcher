using ComicCatcherLib.ComicModels;
using ComicCatcherLib.DbModel;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Model.Repositories;

public class ComicApiRepository
{
    public ComicApiRepository(IHostingEnvironment hostEnvironment)
    {
        var env = hostEnvironment;
        ApiSQLiteHelper.SetDbPath(env.WebRootPath);
        this.CreateComicOnFly();
        this.CreateApiChapterOnFly();
    }

    private void CreateComicOnFly()
    {
        ApiSQLiteHelper.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS ApiComic(
Comic NVARCHAR(200) not NULL,
Caption NVARCHAR(200) not NULL,
Url NVARCHAR(200) not NULL,
IconUrl NVARCHAR(200) not NULL,
ListState INT no NULL,
LastUpdateChapter NVARCHAR(100),
LastUpdateDate NVARCHAR(100)
);").Wait(); ;
        ApiSQLiteHelper.ExecuteNonQuery(@"CREATE UNIQUE INDEX IF NOT EXISTS idx_ApiComic ON ApiComic (Comic);").Wait();
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

    public async Task SaveComic(ComicEntity comic)
    {
        var comicUrl = (new Uri(comic.Url)).LocalPath.Trim('/');
        var sql = $@"INSERT OR REPLACE INTO ApiComic (Comic,Caption,Url,IconUrl, ListState, LastUpdateChapter, LastUpdateDate) VALUES 
('{comicUrl}','{comic.Caption}','{comic.Url}',{comic.IconUrl}, {(int)comic.ListState}, '{comic.LastUpdateChapter}', '{comic.LastUpdateDate}');";
        await ApiSQLiteHelper.ExecuteNonQuery(sql);
    }


    #region Chapter
    private void CreateApiChapterOnFly()
    {
        ApiSQLiteHelper.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS ApiChapter(
Comic NVARCHAR(200) not NULL,
Chapter NVARCHAR(200) not NULL,
Caption NVARCHAR(200) not NULL,
Url NVARCHAR(200) not NULL
);").Wait();
        ApiSQLiteHelper.ExecuteNonQuery(@"CREATE UNIQUE INDEX IF NOT EXISTS idx_ApiChapter ON ApiChapter (Comic, Chapter);").Wait();
        ApiSQLiteHelper.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS ApiPage(
Comic NVARCHAR(200) not NULL,
Chapter NVARCHAR(200) not NULL,
PageNumber INT not NULL,
Caption NVARCHAR(200) not NULL,
Url NVARCHAR(200) not NULL,
PageFileName NVARCHAR(20) not NULL,
Refer NVARCHAR(200) not NULL
);").Wait();
        ApiSQLiteHelper.ExecuteNonQuery(@"CREATE UNIQUE INDEX IF NOT EXISTS idx_ApiPage ON ApiPage (Comic, Chapter, PageNumber);").Wait();
    }

    public async Task<ComicChapter> GetComicChapterWithPages(string comic, string chapter)
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

        comicChapter.Pages = await this.GetComicPages(comic, chapter);
        if (comicChapter.Pages.Count <= 0) comicChapter.ListState = ComicState.ListLoaded;
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
        await ApiSQLiteHelper.ExecuteNonQuery(sql);
        return true;
    }

    public async Task<bool> SaveComicPages(string comic, string chapter, List<ComicPage> pages)
    {
        var sql = "DELETE FROM ApiPage WHERE Comic = @Comic AND Chapter = @Chapter";
        await ApiSQLiteHelper.ExecuteNonQuery(sql);
        foreach (var page in pages)
        {
            sql = $@"INSERT OR REPLACE INTO ApiComic (Comic, Chapter, PageNumber, Caption, Url, PageFileName, Refer) VALUES
    ('{comic}', '{chapter}', {page.PageNumber}, '{page.Caption}', '{page.Url}', '{page.PageFileName}', '{page.Refer}');";
            await ApiSQLiteHelper.ExecuteNonQuery(sql);
        }
        return true;
    }

    public async Task<string> GetLocalPath(string comic, string chapter)
    {
        var sql = "SELECT LocalPath AS cnt FROM ApiChapter WHERE Comic = @Comic AND Chapter = @Chapter";
        var path = await ApiSQLiteHelper.ExecuteScalar<string>(sql);
        return path;
    }
    #endregion
}
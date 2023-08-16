namespace ComicCatcherLib.DbModel;

public class ApiChapterDao
{
    public static void ApiChapterTableOnFly()
    {
        var sql = @"
CREATE TABLE IF NOT EXISTS ApiChapter(
Comic NVARCHAR(200) not NULL,
Chapter NVARCHAR(200) not NULL,
Url NVARCHAR(400) not NULL,
LocalPath (200) not NULL,
State int not NULL
);";
        try
        {
            SQLiteHelper.ExecuteNonQuery(sql);
        }
        catch { /* doNothing */ }
    }

    public static string GetLocalPath(string comic, string chapter)
    {
        var sql = "SELECT LocalPath AS cnt FROM ApiChapter WHERE Comic = @Comic AND Chapter = @Chapter";
        var path = ApiSQLiteHelper.ExecuteScalar<string>(sql);

        return path;
    }
}
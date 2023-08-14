namespace ComicCatcherLib.DbModel;

public class DownloadedDao
{
    public static void CreateTableOnFly()
    {
        string sql = @"
CREATE TABLE DownloadedList(
ComicWeb NVARCHAR(50) not NULL,
ComicName NVARCHAR(50) not NULL,
ComicVolumn NVARCHAR(50) not NULL,
UNIQUE (ComicWeb, ComicName, ComicVolumn) ON CONFLICT REPLACE
);";
        try
        {
            SQLiteHelper.ExecuteNonQuery(sql);
        }
        catch { /* doNothing */ }

        sql = "CREATE INDEX IX_DList_ComicName ON DownloadedList(ComicWeb, ComicName, ComicVolumn)";
        try
        {
            SQLiteHelper.ExecuteNonQuery(sql);
        }
        catch { /* doNothing */ }
    }
    public static bool InDownloaded(string comicWeb, string comicName, string comicVolumn)
    {
        var sql = $"SELECT count(1) as cnt from DownloadedList where ComicWeb = '{comicWeb}' and ComicName = '{comicName}' and ComicVolumn = '{comicVolumn}' LIMIT 0, 1";
        return SQLiteHelper.ExecuteScalar<long>(sql) > 0;
    }

    public static int AddDownloaded(string comicWeb, string comicName, string comicVolumn)
    {
        try
        {
            string sql =
                "INSERT INTO DownloadedList (ComicWeb, ComicName, ComicVolumn) values ('{0}' , '{1}', '{2}')";
            return SQLiteHelper.ExecuteNonQuery(string.Format(sql, comicWeb, comicName, comicVolumn));
        }
        catch (Exception ex)
        {
            //NLogger.Error("新增下載名單到資料庫時發生錯誤：" + ex.ToString());
            return 0;
        }
    }

}
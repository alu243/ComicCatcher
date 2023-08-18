using System.Data;

namespace ComicCatcherLib.DbModel;

public class IgnoreComicDao
{
    public static async Task<DataTable> GetTable()
    {
        string sql = "select * from IgnoreComic";
        var table = await SQLiteHelper.GetTable(sql);
        return table;
    }

    public static async Task CreateTableOnFly()
    {
        string sql = @"
CREATE TABLE IgnoreComic (
ComicUrl NVARCHAR(200) not NULL,
ComicName NVARCHAR(50) not NULL,
UNIQUE (ComicUrl) ON CONFLICT REPLACE
);";
        try
        {
            await SQLiteHelper.ExecuteNonQuery(sql);
        }
        catch { /* doNothing */ }
    }

    public static async Task<bool> AddIgnoreComic(string url, string name)
    {
        try
        {
            string sql = $"INSERT INTO IgnoreComic (ComicUrl, ComicName) values ('{url}' , '{name}')";
            return await SQLiteHelper.ExecuteNonQuery(sql) > 0;
        }
        catch (Exception ex)
        {
            //NLogger.Error("新增例外名單到資料庫時發生錯誤：" + ex.ToString());
            return false;
        }
    }
    public static async Task<bool> UpdateIgnoreComic(string url, string name)
    {
        try
        {
            var sql = $"UPDATE IgnoreComic SET ComicName = '{name}' WHERE ComicUrl = '{url}'";
            return await SQLiteHelper.ExecuteNonQuery(sql) > 0;
        }
        catch (Exception ex)
        {
            //NLogger.Error("更新例外名單到資料庫時發生錯誤：" + ex.ToString());
            return false;
        }
    }

    public static async Task<bool> DeleteIgnoreComic(string url)
    {
        try
        {
            var sql = $"DELETE FROM IgnoreComic WHERE ComicUrl = '{url}'";
            return await SQLiteHelper.ExecuteNonQuery(sql) > 0;
        }
        catch (Exception ex)
        {
            //NLogger.Error("刪除例外名單到資料庫時發生錯誤：" + ex.ToString());
            return false;
        }
    }


    public static async Task ApplyChangesToDatabase(DataTable dataTable)
    {
        foreach (DataRow row in dataTable.Rows)
        {
            if (row.RowState == DataRowState.Modified)
            {
                // 更新操作
                var url = Convert.ToString(row["ComicUrl"])?.Trim();
                var name = Convert.ToString(row["ComicName"])?.Trim();
                await UpdateIgnoreComic(url, name);
            }
            else if (row.RowState == DataRowState.Deleted)
            {
                // 删除操作
                var url = Convert.ToString(row["ComicUrl", DataRowVersion.Original])?.Trim();
                //var name = Convert.ToString(row["ComicName"])?.Trim();
                await DeleteIgnoreComic(url);
            }
            else if (row.RowState == DataRowState.Added)
            {
                var url = Convert.ToString(row["ComicUrl"])?.Trim();
                var name = Convert.ToString(row["ComicName"])?.Trim();
                await AddIgnoreComic(url, name);
            }
        }
    }
}
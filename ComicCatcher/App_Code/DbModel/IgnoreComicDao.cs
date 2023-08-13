using System;
using System.Data;
using ComicCatcher.Helpers;

namespace ComicCatcher.DbModel
{
    public class IgnoreComicDao
    {
        public static DataTable GetTable()
        {
            string sql = "select * from IgnoreComic";
            var table = SQLiteHelper.GetTable(sql);
            return table;
        }

        public static void CreateTableOnFly()
        {
            string sql = @"
CREATE TABLE IgnoreComic (
ComicUrl NVARCHAR(200) not NULL,
ComicName NVARCHAR(50) not NULL,
UNIQUE (ComicUrl) ON CONFLICT REPLACE
);";
            try
            {
                SQLiteHelper.ExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }
        }

        public static bool AddIgnoreComic(string url, string name)
        {
            try
            {
                string sql = $"INSERT INTO IgnoreComic (ComicUrl, ComicName) values ('{url}' , '{name}')";
                return SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                NLogger.Error("新增例外名單到資料庫時發生錯誤：" + ex.ToString());
                return false;
            }
        }
        public static bool UpdateIgnoreComic(string url, string name)
        {
            try
            {
                var sql = $"UPDATE IgnoreComic SET ComicName = '{name}' WHERE ComicUrl = '{url}'";
                return SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                NLogger.Error("更新例外名單到資料庫時發生錯誤：" + ex.ToString());
                return false;
            }
        }

        public static bool DeleteIgnoreComic(string url)
        {
            try
            {
                var sql = $"DELETE FROM IgnoreComic WHERE ComicUrl = '{url}'";
                return SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                NLogger.Error("刪除例外名單到資料庫時發生錯誤：" + ex.ToString());
                return false;
            }
        }


        public static void ApplyChangesToDatabase(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                if (row.RowState == DataRowState.Modified)
                {
                    // 更新操作
                    var url = Convert.ToString(row["ComicUrl"])?.Trim();
                    var name = Convert.ToString(row["ComicName"])?.Trim();
                    UpdateIgnoreComic(url, name);
                }
                else if (row.RowState == DataRowState.Deleted)
                {
                    // 删除操作
                    var url = Convert.ToString(row["ComicUrl", DataRowVersion.Original])?.Trim();
                    //var name = Convert.ToString(row["ComicName"])?.Trim();
                    DeleteIgnoreComic(url);
                }
                else if (row.RowState == DataRowState.Added)
                {
                    var url = Convert.ToString(row["ComicUrl"])?.Trim();
                    var name = Convert.ToString(row["ComicName"])?.Trim();
                    AddIgnoreComic(url, name);
                }
            }
        }
    }
}

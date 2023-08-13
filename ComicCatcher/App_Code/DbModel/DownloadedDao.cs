using System;
using System.Collections.Generic;
using ComicCatcher.App_Code.DbModel;
using Helpers;

namespace ComicCatcher.DbModel
{
    public class DownloadedDao
    {
        public static void CreateTableOnFly()
        {
            string sql = @"
CREATE TABLE DownloadedList(
ComicWeb NVARCHAR(50) not NULL,
ComicEntity NVARCHAR(50) not NULL,
ComicVolumn NVARCHAR(50) not NULL,
UNIQUE (ComicWeb, ComicEntity, ComicVolumn) ON CONFLICT REPLACE
);";
            try
            {
                SQLiteHelper.ExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }

            sql = "CREATE INDEX IX_DList_ComicName ON DownloadedList(ComicWeb, ComicEntity, ComicVolumn)";
            try
            {
                SQLiteHelper.ExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }
        }
        public static bool InDownloaded(string comicWeb, string comicName, string comicVolumn)
        {
            var sql = $"SELECT count(1) as cnt from DownloadedList where ComicWeb = '{comicWeb}' and ComicEntity = '{comicName}' and ComicVolumn = '{comicVolumn}' LIMIT 0, 1";
            return SQLiteHelper.ExecuteScalar<long>(sql) > 0;
        }

        public static int AddDownloaded(string comicWeb, string comicName, string comicVolumn)
        {
            try
            {
                string sql = "INSERT INTO DownloadedList (ComicWeb, ComicEntity, ComicVolumn) values ('{0}' , '{1}', '{2}')";
                return SQLiteHelper.ExecuteNonQuery(string.Format(sql, comicWeb, comicName, comicVolumn));
            }
            catch (Exception ex) { NLogger.Error("新增下載名單到資料庫時發生錯誤：" + ex.ToString()); return 0; }
        }

    }
}

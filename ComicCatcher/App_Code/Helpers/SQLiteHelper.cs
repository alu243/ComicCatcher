using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;
using System.Data;
namespace Helpers
{
    public static class SQLiteHelper
    {
        private static string connStr = "Data Source=ComicCatcher.s3db;Pooling=true;Page Size=8192;Journal Mode=off;UTF8Encoding=True;";
        private static SQLiteConnection CreateConn()
        {
            return new SQLiteConnection(connStr);
        }

        public static void CreateSettingsTableOnFly()
        {
            string sql = @"
CREATE TABLE IF NOT EXISTS Settings(
SettingName NVARCHAR(30) not NULL,
SettingValue NVARCHAR(300) not NULL
);";
            SqlExecuteNonQuery(sql);
        }

        public static void CreateDownladedListTableOnFly()
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
                SqlExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }

            sql = "CREATE INDEX IX_DList_ComicName ON DownloadedList(ComicWeb, ComicName, ComicVolumn)";
            try
            {
                SqlExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }
        }

        public static void CreatePathGroupTableOnFly()
        {
            //string sql = @"drop TABLE PathGroup";
            string sql = @"
CREATE TABLE PathGroup (
GroupName NVARCHAR(50) not NULL,
ComicName1 NVARCHAR(50) not NULL,
ComicName2 NVARCHAR(50) ,
ComicName3 NVARCHAR(50) ,
ComicName4 NVARCHAR(50) ,
ComicName5 NVARCHAR(50) ,
ComicName6 NVARCHAR(50) ,
ComicName7 NVARCHAR(50) ,
ComicName8 NVARCHAR(50) ,
ComicName9 NVARCHAR(50) ,
ComicName10 NVARCHAR(50) ,
UNIQUE (GroupName) ON CONFLICT REPLACE
);";
            try
            {
                SqlExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }

            sql = "CREATE INDEX IX_PG_GroupName ON PathGroup(GroupName)";
            try
            {
                SqlExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }
        }


        public static DataTable GetPathGroup()
        {
            string sql = "select * from PathGroup";
            return SqlSelect(sql);
        }

        public static SQLiteDataAdapter GetPathGroupAdapter()
        {
            SQLiteConnection conn = new SQLiteConnection(connStr);
            string sql = "select * from PathGroup";
            SQLiteDataAdapter sda = new SQLiteDataAdapter(sql, conn);
            SQLiteCommandBuilder builder = new SQLiteCommandBuilder(sda);
            sda.UpdateCommand = builder.GetUpdateCommand();
            sda.InsertCommand = builder.GetInsertCommand();
            sda.DeleteCommand = builder.GetDeleteCommand();
            return sda;
        }

        public static int SqlExecuteNonQuery(string sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = sql;
                int affectRecord = cmd.ExecuteNonQuery();
                return affectRecord;
            }
        }

        public static DataTable SqlSelect(string sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                SQLiteDataAdapter sda = new SQLiteDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        public static int InsertComicVolumn(string comicWeb, string comicName, string comicVolumn)
        {
            try
            {
                string sql = "INSERT INTO DownloadedList (ComicWeb, ComicName, ComicVolumn) values ('{0}' , '{1}', '{2}')";
                return SqlExecuteNonQuery(string.Format(sql, comicWeb, comicName, comicVolumn));
            }
            catch (Exception ex) { NLogger.Error("新增下載名單到資料庫時發生錯誤：" + ex.ToString()); return 0; }
        }

        public static bool IsInDownloadedList(string comicWeb, string comicName, string comicVolumn)
        {
            string sql = "SELECT * from DownloadedList where ComicWeb = '{0}' and ComicName = '{1}' and ComicVolumn = '{2}' LIMIT 0, 1";
            return SqlSelect(string.Format(sql, comicWeb, comicName, comicVolumn)).Rows.Count > 0;
        }
    }
}

using Microsoft.Data.Sqlite;
using System.Data;

namespace ComicCatcherLib.DbModel;

public static class SQLiteHelper
{
    //private static string connStr = "Data Source=ComicCatcher.s3db;Pooling=true;Page Size=8192;Journal Mode=off;UTF8Encoding=True;";
    private static string connStr = "Data Source=ComicCatcher.s3db;Pooling=true;";
    private static SqliteConnection CreateConn()
    {
        var conn = new SqliteConnection(connStr);
        return conn;
    }

    public static DataTable GetTable(string sql)
    {
        using (var conn = CreateConn())
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            DataTable dt = new DataTable();
            using (var reader = cmd.ExecuteReader())
            {
                dt.Load(reader);
            }
            return dt;
        }
    }

    public static int ExecuteNonQuery(string sql)
    {
        using (var conn = CreateConn())
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            int affectRecord = cmd.ExecuteNonQuery();
            return affectRecord;
        }
    }

    public static T ExecuteScalar<T>(string sql)
    {
        using (var conn = CreateConn())
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            T scalar = (T)cmd.ExecuteScalar();
            return scalar;
        }
    }


    public static void VACCUM()
    {
        try
        {
            string sql = "VACUUM";
            ExecuteNonQuery(sql);
        }
        catch (Exception ex)
        {
            //NLogger.Error("清除空間時發生錯誤：" + ex.ToString());
        }
    }
}
using Microsoft.Data.Sqlite;
using System.Data;
using Jint.Parser.Ast;

namespace ComicCatcherLib.DbModel;

public static class ApiSQLiteHelper
{
    //private static string connStr = "Data Source=ComicCatcher.s3db;Pooling=true;Page Size=8192;Journal Mode=off;UTF8Encoding=True;";
    private static string connStr = "";

    public static void SetDbPath(string path)
    {
        connStr = $"Data Source={Path.Combine(path, "ComicApi.s3db")};Pooling=true;";
    }


    private static SqliteConnection CreateConn()
    {
        var conn = new SqliteConnection(connStr);
        return conn;
    }

    public static async Task<DataTable> GetTable(string sql)
    {
        using (var conn = CreateConn())
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            DataTable dt = new DataTable();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                dt.Load(reader);
            }
            return dt;
        }
    }

    public static async Task<int> ExecuteNonQuery(string sql)
    {
        using (var conn = CreateConn())
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            int affectRecord = await cmd.ExecuteNonQueryAsync();
            return affectRecord;
        }
    }

    public static async Task<T> ExecuteScalar<T>(string sql)
    {
        using (var conn = CreateConn())
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            T scalar = (T)(await cmd.ExecuteScalarAsync());
            return scalar;
        }
    }


    public static async Task VACCUM()
    {
        try
        {
            string sql = "VACUUM";
            await ExecuteNonQuery(sql);
        }
        catch (Exception ex)
        {
            //NLogger.Error("清除空間時發生錯誤：" + ex.ToString());
        }
    }
}

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

    public static async Task<DataTable> GetTable(string sql)
    {
        await using var conn = new SqliteConnection(connStr);
        await conn.OpenAsync();
        await using var cmd = new SqliteCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        DataTable dt = new DataTable();
        dt.Load(reader);
        return dt;
    }

    public static async Task<int> ExecuteNonQuery(string sql)
    {
        await using var conn = new SqliteConnection(connStr);
        await conn.OpenAsync();
        await using var cmd = new SqliteCommand(sql, conn);
        int affectRecord = await cmd.ExecuteNonQueryAsync();
        return affectRecord;
    }

    public static async Task<T> ExecuteScalar<T>(string sql)
    {
        await using var conn = new SqliteConnection(connStr);
        await conn.OpenAsync();
        await using var cmd = new SqliteCommand(sql, conn);
        T scalar = (T)(await cmd.ExecuteScalarAsync())!;
        return scalar;
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

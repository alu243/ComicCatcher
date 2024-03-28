using Microsoft.Data.Sqlite;
using System.Data;
using Jint.Parser.Ast;
using Quartz.Util;

namespace ComicCatcherLib.DbModel;

public static class ApiSQLiteHelper
{
    //private static string connStr = "Data Source=ComicCatcher.s3db;Pooling=true;Page Size=8192;Journal Mode=off;UTF8Encoding=True;";
    private static string connStr = "";

    public static void SetDbPath(string path)
    {
        connStr = $"Data Source={Path.Combine(path, "ComicApi.s3db")};Pooling=true;";
    }

    public static async Task<SqliteConnection> GetConnection()
    {
        await using var conn = new SqliteConnection(connStr);
        return conn;
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
    public static async Task<DataTable> GetTableLog(string sql)
    {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: [GetComicPagesDb] start");
        await using var conn = new SqliteConnection(connStr);
        await conn.OpenAsync();
        await using var cmd = new SqliteCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: [GetComicPagesDb] executed reader");
        DataTable dt = new DataTable();
        dt.Load(reader);
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: [GetComicPagesDb] got table");
        return dt;
    }

    public static async Task<int> ExecuteNonQueryAsync(string sql)
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

    public static int ExecuteNonQuery(string sql)
    {
        using var conn = new SqliteConnection(connStr);
        using var cmd = new SqliteCommand(sql, conn);
        try
        {
            conn.Open();
            int affectRecord = cmd.ExecuteNonQuery();
            return affectRecord;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            if (conn.State == ConnectionState.Open) conn.Close();
        }
    }


    public static async Task VACCUM()
    {
        try
        {
            string sql = "VACUUM";
            await ExecuteNonQueryAsync(sql);
        }
        catch (Exception ex)
        {
            //NLogger.Error("清除空間時發生錯誤：" + ex.ToString());
        }
    }
}

namespace ComicCatcherLib.DbModel;

public class SettingsDao
{
    public static void CreateSettingsTableOnFly()
    {
        var sql = @"
CREATE TABLE IF NOT EXISTS ComicSettings(
SettingValue NVARCHAR(1000) not NULL
);";
        try
        {
            SQLiteHelper.ExecuteNonQuery(sql).Wait();
        }
        catch { /* doNothing */ }
    }

    public static async Task<bool> SaveSettings(string settingsJson)
    {
        var countSql = "SELECT count(1) AS cnt FROM ComicSettings";
        var count = await SQLiteHelper.ExecuteScalar<long>(countSql);
        string sql;
        if (count > 0)
        {
            sql = "DELETE FROM ComicSettings";
            await SQLiteHelper.ExecuteNonQuery(sql);
        }
        sql = $"INSERT INTO ComicSettings (SettingValue) VALUES ('{settingsJson}')";
        return await SQLiteHelper.ExecuteNonQuery(sql) > 0;
    }

    public static async Task<string> GetSettings()
    {
        var sql = "SELECT SettingValue AS cnt FROM ComicSettings";
        var table = await SQLiteHelper.GetTable(sql);
        if (table.Rows.Count <= 0) return "";

        return table.Rows[0][0].ToString();
    }
}
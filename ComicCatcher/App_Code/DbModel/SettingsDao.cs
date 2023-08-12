using ComicCatcher.App_Code.DbModel;

namespace Models
{
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
                SQLiteHelper.ExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }
        }

        public static bool SaveSettings(string settingsJson)
        {
            var countSql = "SELECT count(1) AS cnt FROM ComicSettings";
            var count = SQLiteHelper.ExecuteScalar<long>(countSql);
            string sql;
            if (count > 0)
            {
                sql = "DELETE FROM ComicSettings";
                SQLiteHelper.ExecuteNonQuery(sql);
            }
            sql = $"INSERT INTO ComicSettings (SettingValue) VALUES ({settingsJson})";
            return SQLiteHelper.ExecuteNonQuery(sql) > 0;
        }

        public static string GetSettings()
        {
            var sql = "SELECT SettingValue AS cnt FROM ComicSettings";
            var table = SQLiteHelper.GetTable(sql);
            if (table.Rows.Count <= 0) return "";

            return table.Rows[0][0].ToString();
        }
    }
}

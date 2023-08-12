using System;
using System.Data;
using Helpers;

namespace ComicCatcher.App_Code.DbModel
{
    public class PathGroupDao
    {
        public static DataTable GetTable()
        {
            string sql = "select * from PathGroup";
            var table = SQLiteHelper.GetTable(sql);
            return table;
        }

        public static void CreateTableOnFly()
        {
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
                SQLiteHelper.ExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }

            sql = "CREATE INDEX IX_PG_GroupName ON PathGroup(GroupName)";
            try
            {
                SQLiteHelper.ExecuteNonQuery(sql);
            }
            catch { /* doNothing */ }
        }

        public static bool AddPathGroup(string groupName, string name1, string name2, string name3, string name4, string name5, string name6, string name7, string name8, string name9, string name10)
        {
            try
            {
                string sql = $@"INSERT INTO PathGroup (GroupName, ComicName1, ComicName2, ComicName3, ComicName4, ComicName5, ComicName6, ComicName7, ComicName8, ComicName9, ComicName10) 
                values ('{groupName}', '{name1}', '{name2}', '{name3}', '{name4}', '{name5}', '{name6}', '{name7}', '{name8}', '{name9}', '{name10}')";
                return SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                NLogger.Error("新增群組名單到資料庫時發生錯誤：" + ex.ToString());
                return false;
            }
        }
        public static bool UpdatePathGroup(string groupName, string name1, string name2, string name3, string name4, string name5, string name6, string name7, string name8, string name9, string name10)
        {
            try
            {
                var sql = $@"UPDATE PathGroup SET ComicName1 = '{name1}', ComicName2 = '{name2}', ComicName3 = '{name3}',
ComicName4 = '{name4}', ComicName5 = '{name5}',ComicName6 = '{name6}', ComicName7= '{name7}', ComicName8 = '{name8}',
ComicName9 = '{name9}', ComicName10 = '{name10}' WHERE GroupName = '{groupName}'";
                return SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                NLogger.Error("更新群組名單到資料庫時發生錯誤：" + ex.ToString());
                return false;
            }
        }

        public static bool DeletePathGroup(string groupName)
        {
            try
            {
                var sql = $"DELETE FROM PathGroup WHERE GroupName = '{groupName}'";
                return SQLiteHelper.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception ex)
            {
                NLogger.Error("刪除群組名單到資料庫時發生錯誤：" + ex.ToString());
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
                    var groupName = Convert.ToString(row["GroupName"])?.Trim();
                    var name1 = Convert.ToString(row["ComicName1"])?.Trim();
                    var name2 = Convert.ToString(row["ComicName2"])?.Trim();
                    var name3 = Convert.ToString(row["ComicName3"])?.Trim();
                    var name4 = Convert.ToString(row["ComicName4"])?.Trim();
                    var name5 = Convert.ToString(row["ComicName5"])?.Trim();
                    var name6 = Convert.ToString(row["ComicName6"])?.Trim();
                    var name7 = Convert.ToString(row["ComicName7"])?.Trim();
                    var name8 = Convert.ToString(row["ComicName8"])?.Trim();
                    var name9 = Convert.ToString(row["ComicName9"])?.Trim();
                    var name10 = Convert.ToString(row["ComicName10"])?.Trim();
                    UpdatePathGroup(groupName, name1, name2, name3, name4, name5, name6, name7, name8, name9, name10);
                }
                else if (row.RowState == DataRowState.Deleted)
                {
                    // 删除操作
                    var url = Convert.ToString(row["ComicUrl"])?.Trim();
                    DeletePathGroup(url);
                }
                else if (row.RowState == DataRowState.Added)
                {
                    var groupName = Convert.ToString(row["GroupName"])?.Trim();
                    var name1 = Convert.ToString(row["ComicName1"])?.Trim();
                    var name2 = Convert.ToString(row["ComicName2"])?.Trim();
                    var name3 = Convert.ToString(row["ComicName3"])?.Trim();
                    var name4 = Convert.ToString(row["ComicName4"])?.Trim();
                    var name5 = Convert.ToString(row["ComicName5"])?.Trim();
                    var name6 = Convert.ToString(row["ComicName6"])?.Trim();
                    var name7 = Convert.ToString(row["ComicName7"])?.Trim();
                    var name8 = Convert.ToString(row["ComicName8"])?.Trim();
                    var name9 = Convert.ToString(row["ComicName9"])?.Trim();
                    var name10 = Convert.ToString(row["ComicName10"])?.Trim();
                    AddPathGroup(groupName, name1, name2, name3, name4, name5, name6, name7, name8, name9, name10);
                }
            }
        }
    }
}

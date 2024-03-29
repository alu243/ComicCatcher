﻿using System.Data;

namespace ComicCatcherLib.DbModel
{
    public class PathGroupDic
    {
        private Dictionary<string, string> dic;

        private PathGroupDic()
        {
            IgnoreComicDao.CreateTableOnFly().Wait();
            dic = new Dictionary<string, string>();
        }

        public string GetGroupName(string cName)
        {
            if (false == dic.ContainsKey(cName)) return cName;
            return dic[cName];
        }

        public static async Task<PathGroupDic> Load()
        {
            try
            {
                var pg = new PathGroupDic();
                DataTable result = await PathGroupDao.GetTable();
                foreach (DataRow row in result.Rows)
                {
                    string groupName = row["GroupName"].ToString().Trim();
                    for (int i = 1; i <= 10; i++)
                    {
                        string name = row[$"ComicName{i}"].ToString().Trim();
                        pg.dic.TryAdd(name, groupName);
                    }
                }

                return pg;
            }
            catch
            {
                return new PathGroupDic();
            }
        }
    }
}

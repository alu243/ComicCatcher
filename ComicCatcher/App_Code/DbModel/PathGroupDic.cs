using System.Collections.Generic;
using System.Data;

namespace ComicCatcher.DbModel
{
    public class PathGroupDic
    {
        private Dictionary<string, string> dic;

        private PathGroupDic()
        {
            IgnoreComicDao.CreateTableOnFly();
            dic = new Dictionary<string, string>();
        }

        public string GetGroupName(string cName)
        {
            if (false == dic.ContainsKey(cName)) return cName;
            return dic[cName];
        }

        public static PathGroupDic Load()
        {
            try
            {
                var pg = new PathGroupDic();
                DataTable result = PathGroupDao.GetTable();
                foreach (DataRow row in result.Rows)
                {
                    string groupName = row["GroupName"].ToString().Trim();
                    for (int i = 1; i <= 10; i++)
                    {
                        string name = row["ComicName" + i.ToString()].ToString().Trim();
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

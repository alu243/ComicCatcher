using System.Collections.Generic;
using System.Data;

namespace ComicCatcher.App_Code.DbModel
{
    public class IgnoreComicDic
    {
        private Dictionary<string, string> dic;

        private IgnoreComicDic()
        {
            IgnoreComicDao.CreateTableOnFly();
            dic = new Dictionary<string, string>();
        }
        //public string GetIgnoreComic(string url)
        //{
        //    if (false == ContainsKey(url)) return url;
        //    return this[url];
        //}

        public bool Add(string url, string name)
        {
            if (true == string.IsNullOrEmpty(url)) return false;
            IgnoreComicDao.AddIgnoreComic(url, name);
            return dic.TryAdd(url, name);
        }

        public bool IsIgnored(string url) => dic.ContainsKey(url);

        public static IgnoreComicDic Load()
        {
            try
            {
                var ig = new IgnoreComicDic();

                var result = IgnoreComicDao.GetTable();
                foreach (DataRow row in result.Rows)
                {
                    string url = row["ComicUrl"].ToString().Trim();
                    string name = row["ComicEntity"].ToString().Trim();
                    ig.dic.TryAdd(url, name);
                }

                return ig;
            }
            catch
            {
                return new IgnoreComicDic();
            }
        }
    }
}

using System.Data;

namespace ComicCatcherLib.DbModel;

public class IgnoreComicDic
{
    private Dictionary<string, string> dic;

    private IgnoreComicDic()
    {
        IgnoreComicDao.CreateTableOnFly().Wait();
        dic = new Dictionary<string, string>();
    }

    public Dictionary<string, string> GetDictionary() => dic.ToDictionary(k => k.Key, v => v.Value);

    public async Task<bool> Add(string url, string name)
    {
        if (true == string.IsNullOrEmpty(url)) return false;
        await IgnoreComicDao.AddIgnoreComic(url, name);
        return dic.TryAdd(url, name);
    }

    public bool IsIgnored(string url) => dic.ContainsKey(url);

    public static async Task<IgnoreComicDic> Load()
    {
        try
        {
            var ig = new IgnoreComicDic();

            var result = await IgnoreComicDao.GetTable();
            foreach (DataRow row in result.Rows)
            {
                string url = row["ComicUrl"].ToString().Trim();
                string name = row["ComicName"].ToString().Trim();
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
namespace ComicCatcherLib.Utils;

public static class StringExtension
{
    public static string GetUrlDirectoryName(this string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        var uri = new Uri(s);
        return uri.LocalPath.Trim('/');
    }

    public static string CombineUrl(this string s, params string[] directories)
    {
        var uri = new Uri(s);
        foreach (var directory in directories)
        {
            if (false == string.IsNullOrEmpty(directory))
            {
                uri = new Uri(uri, directory);
            }
        }
        return uri.ToString();
    }


    /// <summary>
    /// 清除有可能會造錯成錯誤目之字串
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string TrimEscapeString(this string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        string ss = s;
        ss = ss.Replace("?", "");
        ss = ss.Replace("*", "");
        ss = ss.Replace(":", "：");
        ss = ss.Replace("\\", "");
        ss = ss.Replace("/", "");
        ss = ss.Replace(">", "＞");
        ss = ss.Replace("<", "＜");
        ss = ss.Replace("|", "");
        ss = ss.Replace("\"", "");
        ss = ss.Replace("...", "…");
        ss = ss.Replace(@"""", " ");
        ss = ss.Replace("..", "").Replace("..", "").Replace("..", "").Replace("..", "").Replace("..", "").Replace("..", "");
        ss = ss.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
        return ss.Trim().Trim('.').Trim().Trim('.');
    }

    public static string GetRefererString(this string s)
    {
        return "http://" + new Uri(s).Host.ToString();
        //return new Uri(s).Host.ToString();
    }

    public static string MakeUrlAbsolute(this string url, string rootUrl)
    {
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute) == false)
        {
            url = new Uri(new Uri(rootUrl), url).ToString();
        }
        return url;
    }

    public static string TrimComicName(this string name, string comicName)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        if (string.IsNullOrWhiteSpace(comicName)) return name;
        name = name?.Replace(comicName, string.Empty).Trim();
        return name;
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using System.IO;
using Utils;

namespace ComicModels
{
    public class ComicList : ComicBase, IComicPage
    {

        private readonly static Regex rLink = new Regex(@"<a (.|\n)*?>", RegexOptions.Compiled);
        private readonly static Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
        private readonly static Regex rCaption = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);
        //Regex rDesc = new Regex


        public ComicList()
            : base()
        { }
        public ComicList(string url)
        {
            this.Url = url;
        }

        public List<ComicName> getComicNames()
        {
            string htmlContent = HttpUtil.getResponse(this.Url);
            List<string> comicList = GetComicList(htmlContent);

            List<ComicName> result = comicList.Select<string, ComicName>(comic =>
            {
                string sLink = rLink.Match(comic).Value;
                ComicName cn = new ComicName()
                {
                    IconUrl = GetIconUri(comic), // 取得漫畫首頁圖像連結
                    LastUpdateDate = GetLastUpdateDate(comic), // 取得最近更新日期
                    LastUpdateChapter = GetLastUpdateInfo(comic), // 取得最近更新回數
                    Url = rUrl.Match(sLink).Value.Replace("href=", "").Replace(@"""", "").Trim(),
                    Caption = CharsetConvertUtil.ToTraditional(rCaption.Match(sLink).Value.Replace("title=", "").Replace(@"""", "").Trim())
                    //foreach (char c in Path.GetInvalidFileNameChars()) cb.description = cb.description.Replace(c.ToString(), "");
                };
                if (Uri.IsWellFormedUriString(cn.Url, UriKind.Absolute) == false) cn.Url = (new Uri(new Uri(XindmWebSite.WebUrl), cn.Url)).ToString();
                return cn;
            }).ToList();

            return result;
        }

        private static List<string> GetComicList(string htmlContent)
        {
            Regex rTableTag = new Regex(@"<table width=""\d+"" border=""\d+"" cellpadding=""\d+"" cellspacing=""\d+"" bgcolor=""#CDCDCD"">(.|\n)*?</table>", RegexOptions.Compiled);
            //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Regex rComicList = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);

            string sTemp = rTableTag.Match(htmlContent).ToString();
            return rComicList.Matches(sTemp).Cast<Match>().Select(p => p.Value).ToList();
        }

        private static string GetIconUri(string matchedData)
        {
            Regex rIconUri = new Regex(@"<img src=""(.|\n)*?""", RegexOptions.Compiled);
            Uri iconUri = new Uri(new Uri(XindmWebSite.WebUrl),
                rIconUri.Match(matchedData).Value.Replace("<img src=", "").Replace(@"""", "").Trim());
            return iconUri.ToString();
        }

        private static string GetLastUpdateDate(string matchedData)
        {
            Regex rLastUpdateDate = new Regex(@"<span class=""gray font11"">(.|\n)*?</span>", RegexOptions.Compiled);
            return rLastUpdateDate.Match(matchedData).Value.Replace(@"<span class=""gray font11"">", "").Replace("</span>", "");
        }

        private static string GetLastUpdateInfo(string matchedData)
        {
            Regex rLastUpdateInfoOuter = new Regex(@"\[<a href=""(.|\n)*?</a>\]", RegexOptions.Compiled);
            Regex rLastUpdateInfoInner = new Regex(@">(.|\n)+?<", RegexOptions.Compiled);
            return rLastUpdateInfoInner.Match(rLastUpdateInfoOuter.Match(matchedData).Value).Value.Replace("<", "").Replace(">", "");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using System.IO;
using Utils;

namespace ComicModels
{
    public class ComicList : ComicBase, IComicList
    {

        private readonly static Regex rTableTag = new Regex(@"<table width=""\d+"" border=""\d+"" cellpadding=""\d+"" cellspacing=""\d+"" bgcolor=""#CDCDCD"">(.|\n)*?</table>", RegexOptions.Compiled);
        //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private readonly static Regex rComicList = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);
        private readonly static Regex rLink = new Regex(@"<a (.|\n)*?>", RegexOptions.Compiled);
        private readonly static Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
        private readonly static Regex rIconUrl = new Regex(@"<img src=""(.|\n)*?""", RegexOptions.Compiled);
        private readonly static Regex rDesc = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);
        //Regex rDesc = new Regex
        private readonly static Regex rUpdateDate = new Regex(@"<span class=""gray font11"">(.|\n)*?</span>", RegexOptions.Compiled);

        private readonly static Regex rChapter1 = new Regex(@"\[<a href=""(.|\n)*?</a>\]", RegexOptions.Compiled);
        private readonly static Regex rChapter2 = new Regex(@">(.|\n)+?<", RegexOptions.Compiled);

        public ComicList()
            : base()
        { }
        public ComicList(string url)
        {
            this.url = url;
        }

        public List<ComicName> getComicNameList()
        {
            List<ComicName> result = new List<ComicName>();
            string sTemp = getTable();
            foreach (Match data in rComicList.Matches(sTemp))
            {
                string sLink = rLink.Match(data.Value).Value;
                
                // 取得漫畫首頁圖像連結
                string iUrl = new Uri(new Uri(XindmWebSite.PicHost),
                    rIconUrl.Match(data.Value).Value.Replace("<img src=", "").Replace(@"""", "").Trim()).ToString();
               
                // 取得最近更新日期
                string sUpdateDate = rUpdateDate.Match(data.Value).Value.Replace(@"<span class=""gray font11"">", "").Replace("</span>", "");

                // 取得最近更新回數
                string sUpdateChapter = rChapter2.Match(rChapter1.Match(data.Value).Value).Value.Replace("<", "").Replace(">", "");

                ComicName cb = new ComicName()
                {
                    iconUrl = iUrl,
                    LastUpdateDate = sUpdateDate,
                    LastUpdateChapter = sUpdateChapter,
                    url = rUrl.Match(sLink).Value.Replace("href=", "").Replace(@"""", "").Trim(),
                    Caption = CharsetConvertUtil.ToTraditional(rDesc.Match(sLink).Value.Replace("title=", "").Replace(@"""", "").Trim())
                };
                //foreach (char c in Path.GetInvalidFileNameChars()) cb.description = cb.description.Replace(c.ToString(), "");
                result.Add(cb);

            }
            return result;
        }

        private string getTable()
        {
            return rTableTag.Match(this.htmlContent).ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Text.RegularExpressions;
using ComicCatcher.App_Code.Util;
using Utils;
namespace ComicCatcher.App_Code.Comic
{
    public class ComicChapter : ComicBase
    {
        private static Uri webPrefix = new Uri(XindmWebSite.PicHost);
        private static Regex rPages = new Regex(@"var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);

        public ComicChapter(string url)
        {
            this.url = url;
            this.htmlContent = HttpUtil.getResponse(url);
        }

        public List<string> genPictureUrl()
        {
            List<string> pages = new List<string>();
            foreach (Match match in rPages.Matches(this.htmlContent))
            {
                foreach (string tmp in match.Value.Split(','))
                {
                    string page = tmp.Replace(@"var ArrayPhoto=new Array(", "").Replace(@"""", "").Replace(@");", "").Trim();
                    pages.Add(new Uri(webPrefix, page.TrimStart('/')).ToString());
                }
            }
            return pages;
        }
    }
}

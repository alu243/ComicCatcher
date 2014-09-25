using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using Utils;
namespace ComicModels
{
    public class Xindm : IComicCatcher
    {
        public void Dispose()
        {
            this._cRoot = null;
        }

        #region Root
        private ComicRoot _cRoot = new ComicRoot()
        {
            WebSiteTitle = "新動漫(xindm)",
            WebSiteName = "xindm",
            WebHost = @"http://www.xindm.cn/mh",
            PicHost = @"http://mh.xindm.cn/",
            PicHostAlternative = @"http://mh2.xindm.cn/"
        };

        public ComicRoot GetComicRoot()
        {
            return this._cRoot;
        }
        #endregion

        #region Groups
        public List<ComicGroup> GetComicGroups()
        {
            List<ComicGroup> groupList = new List<ComicGroup>();
            for (int i = 1; i <= 300; ++i)
            {
                ComicGroup cg = new ComicGroup();
                cg.GroupNumber = i + 1;
                cg.Caption = "第" + (i + 1).ToString().PadLeft(3, '0') + "頁";
                cg.Url = this._cRoot.WebHost + "/index_" + (i + 1).ToString() + ".html").Replace("index_1.html", "index.html");
                groupList.Add(cg);
            }
            return groupList;
        }
        #endregion

        #region Names
        public List<ComicName> GetComicNames(ComicGroup cGroup)
        {
            Regex rLink = new Regex(@"<a (.|\n)*?>", RegexOptions.Compiled);
            Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
            Regex rCaption = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);

            string htmlContent = ComicUtil.GetContent(cGroup.Url);

            List<string> comicList = SplitForComicName(htmlContent);
            List<ComicName> result = comicList.Select<string, ComicName>(comic =>
            {
                string sLink = rLink.Match(comic).Value;
                ComicName cn = new ComicName()
                {
                    IconUrl = RetriveIconUri(comic), // 取得漫畫首頁圖像連結
                    LastUpdateDate = RetriveLastUpdateDate(comic), // 取得最近更新日期
                    LastUpdateChapter = RetriveLastUpdateInfo(comic), // 取得最近更新回數
                    Url = rUrl.Match(sLink).Value.Replace("href=", "").Replace(@"""", "").Trim(),
                    Caption = CharsetConvertUtil.ToTraditional(rCaption.Match(sLink).Value.Replace("title=", "").Replace(@"""", "").Trim())
                    //foreach (char c in Path.GetInvalidFileNameChars()) cb.description = cb.description.Replace(c.ToString(), "");
                };
                if (Uri.IsWellFormedUriString(cn.Url, UriKind.Absolute) == false) cn.Url = (new Uri(new Uri(XindmWebSite.WebUrl), cn.Url)).ToString();
                return cn;
            }).ToList();

            return result;
        }

        private List<string> SplitForComicName(string htmlContent)
        {
            Regex rTableTag = new Regex(@"<table width=""\d+"" border=""\d+"" cellpadding=""\d+"" cellspacing=""\d+"" bgcolor=""#CDCDCD"">(.|\n)*?</table>", RegexOptions.Compiled);
            //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Regex rComicList = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);
            string sTemp = rTableTag.Match(htmlContent).ToString();
            return rComicList.Matches(sTemp).Cast<Match>().Select(p => p.Value).ToList();
        }

        private string RetriveIconUri(string matchedData)
        {
            Regex rIconUri = new Regex(@"<img src=""(.|\n)*?""", RegexOptions.Compiled);
            Uri iconUri = new Uri(new Uri(XindmWebSite.WebUrl),
                rIconUri.Match(matchedData).Value.Replace("<img src=", "").Replace(@"""", "").Trim());
            return iconUri.ToString();
        }

        private string RetriveLastUpdateDate(string matchedData)
        {
            Regex rLastUpdateDate = new Regex(@"<span class=""gray font11"">(.|\n)*?</span>", RegexOptions.Compiled);
            return rLastUpdateDate.Match(matchedData).Value.Replace(@"<span class=""gray font11"">", "").Replace("</span>", "");
        }

        private string RetriveLastUpdateInfo(string matchedData)
        {
            Regex rLastUpdateInfoOuter = new Regex(@"\[<a href=""(.|\n)*?</a>\]", RegexOptions.Compiled);
            Regex rLastUpdateInfoInner = new Regex(@">(.|\n)+?<", RegexOptions.Compiled);
            return rLastUpdateInfoInner.Match(rLastUpdateInfoOuter.Match(matchedData).Value).Value.Replace("<", "").Replace(">", "");
        }
        #endregion

        #region Chapters
        public List<ComicChapter> GetComicChapters(ComicName cName)
        {
            //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Regex rVolumnList = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);
            //private readonly static Regex rLink = new Regex(@"<a (.|\n)*?>", RegexOptions.Compiled);
            Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
            Regex rCleanTag = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
            Regex rDesc = new Regex(@"<span class=""black"">.+?</span>", RegexOptions.Compiled);

            string htmlContent = HttpUtil.getResponse(cName.Url);
            List<ComicChapter> result = new List<ComicChapter>();
            string sTemp = ComicUtil.GetContent(htmlContent);
            foreach (Match data in rVolumnList.Matches(sTemp))
            {
                //string sLink = rLink.Match(data.Value).Value;
                string sLink = data.Value;
                ComicChapter cb = new ComicChapter()
                {
                    Url = rUrl.Match(sLink).Value.Replace("href=", "").Replace(@"""", "").Replace(@" /", @"/").Trim(),
                    //description = CharsetConverter.ToTraditional(rDesc.Match(sLink).Value.Replace(@"<span class=""black"">", "").Replace(@"</span>", "")
                    //.Replace(@"<fontcolor=red>", "").Replace(@"</font>", "").Replace(@"<b>","").Replace(@"</b>","").Trim())
                    Caption = CharsetConvertUtil.ToTraditional(rCleanTag.Replace(rDesc.Match(sLink).Value.Trim(), ""))
                };
                if (Uri.IsWellFormedUriString(cb.Url, UriKind.Absolute) == false) cb.Url = (new Uri(new Uri(XindmWebSite.WebUrl), cb.Url)).ToString();


                if (false == String.IsNullOrEmpty(cb.Caption))
                    result.Add(cb);
            }

            return result;
        }

        private string RetriveHtmlTagContent(string htmlContent)
        {
            Regex rVolumnTag = new Regex(@"<ul>(.|\n)*?</ul>", RegexOptions.Compiled);
            StringBuilder sb = new StringBuilder();
            foreach (Match match in rVolumnTag.Matches(htmlContent))
            {
                sb.AppendLine(match.Value);
            }
            return sb.ToString();
        }
        #endregion

        #region Pages
        public List<ComicPage> GetComicPages(ComicChapter cChapter)
        {
            Uri webPrefix = new Uri(this._cRoot.PicHost);
            Regex rPages = new Regex(@"var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);

            string htmlContent = HttpUtil.getResponse(cChapter.Url);
            List<ComicPage> pages = new List<ComicPage>();
            foreach (Match match in rPages.Matches(htmlContent))
            {
                int i = 1;
                foreach (string tmp in match.Value.Split(','))
                {
                    ComicPage page = new ComicPage();
                    page.Url = tmp.Replace(@"var ArrayPhoto=new Array(", "").Replace(@"""", "").Replace(@");", "").Trim();
                    page.Url = new Uri(webPrefix, page.Url.TrimStart('/')).ToString();

                    page.Caption = "第" + i.ToString().PadLeft(3, '0') + "頁";
                    pages.Add(page);
                    i++;
                }
            }
            return pages;
        } 
        #endregion
    }
}

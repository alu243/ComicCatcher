using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using Utils;

namespace ComicModels
{
    public class Dm5 : IComicCatcher
    {
        public void Dispose()
        {
            this._cRoot = null;
        }

        #region Root
        private ComicWebRoot _cRoot = new ComicWebRoot()
        {
            WebSiteTitle = "動漫屋(dm5)",
            WebSiteName = "dm5",
            WebHost = @"http://www.dm5.com/",
            IconHost = @"",
            PicHost = @"",
            PicHost2 = @"",

            PicHostAlternative = @"",

            ThreadCount = 40
        };

        public ComicWebRoot GetComicWebRoot()
        {
            return this._cRoot;
        }
        #endregion

        #region Groups
        public List<ComicWebPage> GetComicWebPages()
        {
            List<ComicWebPage> webPages = new List<ComicWebPage>();
            for (int i = 0; i < 300; ++i)
            {
                // http://www.dm5.com/manhua-list-p1/
                ComicWebPage wp = new ComicWebPage();
                wp.GroupNumber = i + 1;
                wp.Caption = "第" + (i + 1).ToString().PadLeft(3, '0') + "頁";

                //wp.Url = new Uri(new Uri(this._cRoot.WebHost), ("manhua-list-p" + (i + 1).ToString() + "/")).ToString();
                wp.Url = new Uri(new Uri(this._cRoot.WebHost), ("manhua-list-s2-p" + (i + 1).ToString() + "/")).ToString();

                webPages.Add(wp);
            }
            return webPages;
        }
        #endregion

        #region Names
        public List<ComicNameInWebPage> GetComicNames(ComicWebPage cGroup)
        {
            //Regex rLink = new Regex(@"<a (.|\n)*?<strong>(.|\n)*?</a>", RegexOptions.Compiled);
            //Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
            //Regex rCaption = new Regex(@"<strong>(.|\n)*?<", RegexOptions.Compiled);
            string htmlContent = ComicUtil.GetUtf8Content(cGroup.Url);
            List<string> comicList = RetriveComicNames(htmlContent);
            List<ComicNameInWebPage> result = comicList.Select<string, ComicNameInWebPage>(comic =>
            {
                //string sLink = rLink.Match(comic).Value;
                ComicNameInWebPage cn = new ComicNameInWebPage()
                {
                    IconUrl = RetriveComicName_IconUrl(comic), // 取得漫畫首頁圖像連結
                    LastUpdateDate = RetriveComicName_LastUpdateDate(comic), // 取得最近更新日期
                    LastUpdateChapter = RetriveComicName_LastUpdateInfo(comic), // 取得最近更新回數
                    //Url = rUrl.Match(sLink).Value.Replace("href=", "").Trim('"').Trim(),
                    Url = RetriveComicName_Url(comic),
                    Caption = RetriveComicName_Caption(comic),
                    //Caption = CharsetConvertUtil.ToTraditional(rCaption.Match(sLink).Value.Replace("<strong>", "").Trim('<'))
                    //foreach (char c in Path.GetInvalidFileNameChars()) cb.description = cb.description.Replace(c.ToString(), "");
                };

                if (false == String.IsNullOrEmpty(cn.LastUpdateChapter))
                {
                    cn.LastUpdateChapter = cn.LastUpdateChapter.Replace(cn.Caption, String.Empty).Trim();
                }

                if (Uri.IsWellFormedUriString(cn.Url, UriKind.Absolute) == false) cn.Url = (new Uri(new Uri(this._cRoot.WebHost), cn.Url)).ToString();
                return cn;
            }).ToList();

            return result;
        }

        private List<string> RetriveComicNames(string htmlContent)
        {
            Regex comicListOuter = new Regex(@"<ul class=""mh-list col7"">(.|\n)*?</ul>", RegexOptions.Compiled);
            Regex comicListInner = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);

            var allInOne = comicListOuter.Matches(htmlContent).Cast<Match>().Select(p => p.Value).First();
            return comicListInner.Matches(allInOne).Cast<Match>().Select(p => p.Value).ToList();
        }

        private string RetriveComicName_IconUrl(string matchedData)
        {
            //  <span>最新</span>
            //  <a href="/m582240/" title="番外2" target="_blank">番外2 </a>
            Regex rIconUri = new Regex(@"<p class=""mh-cover"" +style=""background-image: url\((.|\n)*?\)", RegexOptions.Compiled);
            Uri iconUri = new Uri(rIconUri.Match(matchedData).Value.Replace(@"<p class=""mh-cover""", "").Trim().Replace(@"style=""background-image: url(", "").Replace(@")", "").Trim());
            return iconUri.ToString();
        }

        private string RetriveComicName_LastUpdateDate(string matchedData)
        {
            //<p class="zl"> 17分钟前更新 </p>
            //Regex rLastUpdateDate = new Regex(@"\d{4}年\d{1,2}月\d{1,2}日：", RegexOptions.Compiled);
            Regex rLastUpdateDate = new Regex(@"<p class=""zl"">(.|\n)*?</p>", RegexOptions.Compiled);
            return rLastUpdateDate.Match(matchedData).Value.Replace(@"<p class=""zl"">", "").Replace("</p>", "").Trim();
        }

        private string RetriveComicName_LastUpdateInfo(string matchedData)
        {
            // <span>最新</span>
            // <a href="/m582240/" title="番外2" target="_blank">番外2 </a>
            //Regex rLastUpdateInfoOuter = new Regex(@"\[(.|\n)*?\]", RegexOptions.Compiled);
            //Regex rLastUpdateInfoInner = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);
            Regex rLastUpdateInfoOuter = new Regex(@"<span>最新(.|\n)*?</a>", RegexOptions.Compiled);
            Regex rLastUpdateInfoInner = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);

            string simplified = rLastUpdateInfoInner.Match(rLastUpdateInfoOuter.Match(matchedData).Value).Value.Replace("title=", "").Trim('"');
            return CharsetConvertUtil.ToTraditional(simplified.Trim());
        }

        private string RetriveComicName_Url(string matchedData)
        {
            //<h2 class="title">
            //  <a href="/manhua-huanjue-zaiyici/" title="幻觉 再一次">幻觉 再一次</a>
            //  <span class="mh-star star-4"></span>
            //</h2>
            Regex rUrlOuter = new Regex(@"<h2 class=""title"">(.|\n)*?</h2>", RegexOptions.Compiled);
            Regex rUrlInner = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);

            string result = rUrlInner.Match(rUrlOuter.Match(matchedData).Value).Value.Replace("href=", "").Trim('"');
            return result;
        }

        private string RetriveComicName_Caption(string matchedData)
        {
            // <h2 class="title">
            //   <a href="/manhua-lurenshangbanzuhebuliangnvgaozhongsheng/" title="路人上班族和不良女高中生" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">路人上班族和不良女高中生</a>
            // </h2>
            Regex rCaptionOuter = new Regex(@"<h2 class=""title"">(.|\n)*?</h2>", RegexOptions.Compiled);
            Regex rCaptionInner = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);

            string simplified = rCaptionInner.Match(rCaptionOuter.Match(matchedData).Value).Value.Replace("title=", "").Trim('"');
            return CharsetConvertUtil.ToTraditional(simplified.Trim());
        }
        #endregion

        #region Chapters
        public List<ComicChapterInName> GetComicChapters(ComicNameInWebPage cName)
        {
            //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            //Regex rVolumnList = new Regex(@"<li {0,}(.|\n)*?</li>", RegexOptions.Compiled);
            //Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
            //Regex rCaption = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);

            string htmlContent = ComicUtil.GetUtf8Content(cName.Url);
            List<ComicChapterInName> result = new List<ComicChapterInName>();
            var chapters = RetriveChapters(htmlContent);
            chapters.ForEach(c =>
            {
                ComicChapterInName cb = new ComicChapterInName()
                {
                    Url = RetriveChapter_Url(c),
                    Caption = RetriveChapter_Caption(c)
                };
                if (Uri.IsWellFormedUriString(cb.Url, UriKind.Absolute) == false) cb.Url = (new Uri(new Uri(this._cRoot.WebHost), cb.Url)).ToString();

                if (false == String.IsNullOrEmpty(cb.Caption))
                {
                    cb.Caption = cb.Caption.Replace(cName.Caption, String.Empty).Trim();
                    result.Add(cb);
                }
            });

            //string tagContent = RetriveHtmlTagContent(htmlContent);
            //foreach (Match volumn in rVolumnList.Matches(tagContent))
            //{
            //    //string sLink = rLink.Match(data.Value).Value;
            //    string sLink = volumn.Value;
            //    ComicChapterInName cb = new ComicChapterInName()
            //    {
            //        Url = rUrl.Match(sLink).Value.Replace("href=", "").Trim('"'),
            //        //description = CharsetConverter.ToTraditional(rDesc.Match(sLink).Value.Replace(@"<span class=""black"">", "").Replace(@"</span>", "")
            //        //.Replace(@"<fontcolor=red>", "").Replace(@"</font>", "").Replace(@"<b>","").Replace(@"</b>","").Trim())
            //        Caption = CharsetConvertUtil.ToTraditional(rCaption.Match(sLink).Value.Replace("title=", "").Trim('"'))
            //    };
            //    if (Uri.IsWellFormedUriString(cb.Url, UriKind.Absolute) == false) cb.Url = (new Uri(new Uri(this._cRoot.WebHost), cb.Url)).ToString();

            //    if (false == String.IsNullOrEmpty(cb.Caption))
            //    {
            //        cb.Caption = cb.Caption.Replace(cName.Caption, String.Empty).Trim();
            //        result.Add(cb);
            //    }
            //}
            return result;
        }

        private List<string> RetriveChapters(string htmlContent)
        {
            Regex comicListOuter = new Regex(@"<div id=""chapterlistload"">(.|\n)*?</div>", RegexOptions.Compiled);
            Regex comicListInner = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);

            var allInOne = comicListOuter.Matches(htmlContent).Cast<Match>().Select(p => p.Value).First();
            return comicListInner.Matches(allInOne).Cast<Match>().Select(p => p.Value).ToList();
        }

        private string RetriveChapter_Url(string chapter)
        {
            //<li>
            //  <a href="/m582159/" title="" target="_blank">第103话
            //    <span>（18P）</span>
            //  </a>
            //  <span class="new"></span>
            //</li>
            Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);

            string result = rUrl.Match(chapter).Value.Replace("href=", "").Trim('"');
            return result;
        }

        private string RetriveChapter_Caption(string chapter)
        {
            //<li>
            //  <a href="/m582159/" title="" target="_blank">第103话
            //    <span>（18P）</span>
            //  </a>
            //  <span class="new"></span>
            //</li>
            Regex rCaption = new Regex(@"target=""_blank"" *?>(.|\n)*?<", RegexOptions.Compiled);
            string simplified = rCaption.Match(chapter).Value.Replace(@"target=""_blank""", "").Trim().Replace(">", "").Replace(@"<", "").Trim('"').Trim();
            return CharsetConvertUtil.ToTraditional(simplified.Trim());
        }

        //private string RetriveHtmlTagContent(string htmlContent)
        //{
        //    //// <ul class="nr6 lan2" id="cbc_1"> 
        //    //Regex rVolumnTag = new Regex(@"<ul class=""nr6 lan2"" id=""cbc(.|\n)*?</ul>", RegexOptions.Compiled);
        //    StringBuilder sb = new StringBuilder();
        //    foreach (Match match in rVolumnTag.Matches(htmlContent))
        //    {
        //        sb.AppendLine(match.Value);
        //    }
        //    return sb.ToString();
        //}
        #endregion

        #region Pages
        public List<ComicPageInChapter> GetComicPages(ComicChapterInName cChapter)
        {
            // fix by
            // http://css122.us.cdndm.com/v201708091849/default/js/chapternew_v22.js
            //Regex rPages = new Regex(@"var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);
            string htmlContent = ComicUtil.GetUtf8Content(cChapter.Url);
            string cid = cChapter.Url.Replace(@"http://www.dm5.com/m", "").Trim('/');
            string mid = RetrivePage_MID(htmlContent);
            string dt = RetrivePage_VIEWSIGNDT(htmlContent);
            string sign = RetrivePage_VIEWSIGN(htmlContent);
            int pageCount = RetrivePage_PageCount(htmlContent);


            //string jsCodeWrapper = "; var url = (typeof (hd_c) != \"undefined\" && hd_c.length > 0) ? hd_c[0] : d[0];";
            //string jsCodeWrapper = "; d[0];";
            string jsCodeWrapper = ";var url = (typeof (isrevtt) != \"undefined\" && isrevtt) ? hd_c[0] : d[0];";
            List<ComicPageInChapter> pages = new List<ComicPageInChapter>();
            if (pageCount > 0)
            {
                for (int i = 1; i <= pageCount; i++)
                {
                    // unpacker test url = 
                    // url: 'chapterfun.ashx',
                    // data: { cid: DM5_CID, page: DM5_PAGE, key: mkey, language: 1, gtk: 6, _cid: DM5_CID, _mid: DM5_MID, _dt: DM5_VIEWSIGN_DT, _sign: DM5_VIEWSIGN },
                    string reffer = this._cRoot.WebHost + "m" + cid + "/";
                    string pageFunUrl = this._cRoot.WebHost + "m" + cid + "/" + "chapterfun.ashx?cid=" + cid + "&page=" + i.ToString() + "&language=1&gtk=6&_cid=" + cid + "&_mid=" + mid + "&_dt=" + dt + "&_sign=" + sign;
                    string pageFunContent;
                    lock (this)
                    {
                        pageFunContent = ComicUtil.GetUtf8Content(pageFunUrl, reffer); // 這個得到的是一串 eval(...)字串
                        for (int j = 0; j <= 20; j++)
                        {
                            if (false == String.IsNullOrEmpty(pageFunContent) && false == pageFunContent.Contains("war|jpg"))
                            {
                                break;
                            }
                            else
                            {
                                pageFunContent = ComicUtil.GetUtf8Content(pageFunUrl); // 這個得到的是一串 eval(...)字串
                            }
                        }
                    }
                    pageFunContent = pageFunContent.Trim('"').Trim('\n');
                    pageFunContent = pageFunContent.Substring(5, pageFunContent.Length - 6);
                    string jsCode = ComicUtil.EvalJScript("var cs = " + pageFunContent).ToString();

                    string jsCodePass2 = ComicUtil.EvalJScript("var isrevtt; var hd_c;" + jsCode + jsCodeWrapper).ToString();
                    ComicPageInChapter page = new ComicPageInChapter();
                    page.PageNumber = i;
                    page.Reffer = reffer;
                    //page.Url = photoServer + pageFile + "?cid=" + cid + "&key=" + key + "&ak=" + ak;
                    page.Url = jsCodePass2;
                    page.Caption = "第" + i.ToString().PadLeft(3, '0') + "頁";
                    page.PageFileName = i.ToString().PadLeft(3, '0') + "." + System.IO.Path.GetExtension(page.Url.Substring(0, page.Url.IndexOf("?"))).TrimStart('.');
                    page.PageFileName = page.PageFileName.Replace("..", ".");

                    pages.Add(page);
                }
            }
            else
            {
                var pageUrls = RetrivePage_PageCount_Type2(htmlContent);
                for (int i = 1; i <= pageUrls.Count; i++)
                {
                    //string reffer = "";
                    //try
                    //{
                    //    reffer = new Uri(pageUrls[i - 1]).Authority;
                    //}
                    //catch { }
                    string reffer = this._cRoot.WebHost + "m" + cid + "/";
                    ComicPageInChapter page = new ComicPageInChapter();
                    page.PageNumber = i;
                    page.Reffer = reffer;
                    page.Url = pageUrls[i - 1];
                    page.Caption = "第" + i.ToString().PadLeft(3, '0') + "頁";
                    page.PageFileName = i.ToString().PadLeft(3, '0') + "." + System.IO.Path.GetExtension(page.Url.Substring(0, page.Url.IndexOf("?"))).TrimStart('.');
                    page.PageFileName = page.PageFileName.Replace("..", ".");

                    pages.Add(page);
                }
            }
            Regex rInvalidFileName = new Regex(string.Format("[{0}]", Regex.Escape(new String(System.IO.Path.GetInvalidFileNameChars()))));
            return pages;
        }

        private string GetJSVariableValue(string jsContent, string variableName)
        {
            Regex vn = new Regex(variableName + @"=.+?;", RegexOptions.Compiled);
            return vn.Match(jsContent).Value.Replace(variableName + "=", "").TrimEnd(';').Trim(new char[] { '"', '\'' });
        }

        private int RetrivePage_PageCount(string page)
        {
            //<div class="chapterpager" style="display: inline;" id="chapterpager">
            //  <span class="current">1</span>
            //  <a href="/m582159-p2/">2</a>
            //  <a href="/m582159-p3/">3</a>
            //  <a href="/m582159-p4/">4</a>
            //  <a href="/m582159-p5/">5</a>
            //  <a href="/m582159-p6/">6</a>
            //  <a href="/m582159-p7/">7</a>
            //  <a href="/m582159-p8/">8</a> ...
            //  <a href="/m582159-p18/">18</a>
            //</div>
            Regex rPageCountOuter = new Regex(@"<div class=""chapterpager""(.|\n)*?</div>", RegexOptions.Compiled);
            Regex rPageCountInner = new Regex(@"<a href=""(.|\n)*?</a>", RegexOptions.Compiled);
            Regex rPageCountText = new Regex(@">(.|\n)*?<", RegexOptions.Compiled);

            var lastPageLink = rPageCountInner.Matches(rPageCountOuter.Match(page).Value).Cast<Match>().LastOrDefault();
            if (lastPageLink == null) return 0;
            string result = rPageCountText.Match(lastPageLink.Value).Value.Replace(">", "").Replace("<", "").Trim();
            int count;
            int.TryParse(result, out count);
            return count;
        }

        private List<string> RetrivePage_PageCount_Type2(string page)
        {
            //<div id="barChapter" oncontextmenu="return false;" style="margin-top: 5px;">         
            //  <p id="imgloading" style="color: rgb(102, 102, 102);margin: 400px 0;text-align: center;"><img style="margin-right:10px;" oncontextmenu="return false;" src="http://css119.test.cdndm.com/v201709131619/dm5/images/loading.gif" />加载中<br /></p>           
            //  <img src="" class="load-src" data-src="http://manhua1032-107-181-243-170.cdndm5.com/22/21840/562747/1_2217.jpg?cid=562747&key=66c590d453e57ca375f7dd5196ca9f7d&type=1" onclick="ShowEnd();" style="cursor:pointer;display:none;margin:0 auto;max-width:100%;width:850px;" />                
            //  <img src="" class="load-src" data-src="http://manhua1032-107-181-243-170.cdndm5.com/22/21840/562747/2_3047.jpg?cid=562747&key=66c590d453e57ca375f7dd5196ca9f7d&type=1" onclick="ShowEnd();" style="cursor:pointer;display:none;margin:0 auto;max-width:100%;width:850px;" />               
            //  <img src="" class="load-src" data-src="http://manhua1032-107-181-243-170.cdndm5.com/22/21840/562747/3_5895.jpg?cid=562747&key=66c590d453e57ca375f7dd5196ca9f7d&type=1" onclick="ShowEnd();" style="cursor:pointer;display:none;margin:0 auto;max-width:100%;width:850px;" />
            //  <img src="" class="load-src" data-src="http://manhua1032-107-181-243-170.cdndm5.com/22/21840/562747/4_1869.jpg?cid=562747&key=66c590d453e57ca375f7dd5196ca9f7d&type=1" onclick="ShowEnd();" style="cursor:pointer;display:none;margin:0 auto;max-width:100%;width:850px;" />      
            //  ...
            //</div>
            Regex rPageCountOuter = new Regex(@"<div id=""barChapter""(.|\n)*?</div>", RegexOptions.Compiled);
            Regex rPageCountInner = new Regex(@"data-src=""(.|\n)*?""", RegexOptions.Compiled);

            var pageLinks = rPageCountOuter.Match(page).Value;
            var results = rPageCountInner.Matches(pageLinks).Cast<Match>().Select(p => p.Value).ToList();
            results = results.Select(p => p.Replace("data-src=", "").Trim().Trim('"').Trim()).ToList();
            return results;
        }

        private string RetrivePage_MID(string page)
        {
            //var DM5_MID=7724; 
            Regex rMID = new Regex(@"var DM5_MID=(.|\n)*?;", RegexOptions.Compiled);
            string result = rMID.Match(page).Value.Replace("var DM5_MID=", "").Replace(";", "").Trim();
            return result;
        }
        private string RetrivePage_VIEWSIGNDT(string page)
        {
            //var DM5_VIEWSIGN_DT=\"2018-02-16 00:29:55\";
            Regex rMID = new Regex(@"var DM5_VIEWSIGN_DT=(.|\n)*?;", RegexOptions.Compiled);
            string result = rMID.Match(page).Value.Replace("var DM5_VIEWSIGN_DT=", "").Replace(";", "").Replace(@"""", "").Trim();
            return result;
        }
        private string RetrivePage_VIEWSIGN(string page)
        {
            //var DM5_VIEWSIGN=\"79e225b5839219b02d6b0be3ff815e1c\";
            Regex rMID = new Regex(@"var DM5_VIEWSIGN=(.|\n)*?;", RegexOptions.Compiled);
            string result = rMID.Match(page).Value.Replace("var DM5_VIEWSIGN=", "").Replace(";", "").Replace(@"""", "").Trim();
            return result;
        }
        #endregion
    }
}

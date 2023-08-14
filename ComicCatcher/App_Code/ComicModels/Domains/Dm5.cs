using ComicCatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ComicCatcher.ComicModels.Domains;

public class Dm5 : IComicCatcher
{
    private SemaphoreSlim locker;

    public Dm5()
    {
        _cRoot = new ComicRoot()
        {
            Caption = "動漫屋(dm5)",
            WebSiteName = "dm5",
            Url = @"https://www.dm5.com/",
            IconHost = @"",
            PicHost = @"",
            PicHost2 = @"",

            PicHostAlternative = @"",
            ThreadCount = 20,
            Paginations = new List<ComicPagination>()
        };
        locker = new SemaphoreSlim(_cRoot.ThreadCount);
        HttpClientUtil.SetConnections(_cRoot.ThreadCount / 2);
    }

    public void Dispose()
    {
        this._cRoot = null;
    }
    #region Root

    private ComicRoot _cRoot;

    public ComicRoot GetRoot()
    {
        return this._cRoot;
    }
    #endregion

    #region Groups
    public List<ComicPagination> GetPaginations()
    {
        var paginations = new List<ComicPagination>();
        for (int i = 1; i <= 300; ++i)
        {
            var url = $"manhua-list-s2-p{i}/";
            var pagination = new ComicPagination()
            {
                TabNumber = i,
                Caption = "第" + i.ToString().PadLeft(3, '0') + "頁",
                Url = url.MakeUrlAbsolute(this.GetRoot().Url),
                Comics = new List<ComicEntity>()
            };
            paginations.Add(pagination);
        }
        this.GetRoot().Paginations.AddRange(paginations);
        return paginations;
    }
    #endregion

    #region Names
    public ComicEntity GetSingleComicName(string url)
    {
        var htmlContent = ComicUtil.GetUtf8Content(url).Result;

        //string sLink = rLink.Match(comic).Value;
        Regex rTitle = new Regex(@"<div class=""banner_detail_form"">(.|\n)*?</div>(.|\n)*?</div>(.|\n)*?</div>(.|\n)*?</div>(.|\n)*?</div>", RegexOptions.Compiled);
        Regex rTitle_Caption = new Regex(@"<p class=""title"">(.|\n)*?<", RegexOptions.Compiled);
        Regex rTitle_IconUrl = new Regex(@"<img src=""(.|\n)*?""", RegexOptions.Compiled);
        Regex rLastUpdate = new Regex(@"<span class=""s"">(.|\n)*?</span>", RegexOptions.Compiled);
        Regex rLastUpdate_Date = new Regex(@"</a>&nbsp;(.|\n)*?<", RegexOptions.Compiled);
        Regex rLastUpdate_Chapter = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);


        string title = rTitle.Match(htmlContent).Value;
        string lastUpdate = rLastUpdate.Match(htmlContent).Value;

        var caption = CharsetConvertUtil.ToTraditional(rTitle_Caption.Match(title).Value.Replace(@"<p class=""title"">", "").Trim('<').Trim());
        var cn = new ComicEntity()
        {
            IconUrl = rTitle_IconUrl.Match(title).Value.Replace(@"<img src=", "").Trim('"').Trim(),
            // 取得最近更新日期
            LastUpdateDate = CharsetConvertUtil.ToTraditional(rLastUpdate_Date.Match(lastUpdate).Value.Replace("</a>&nbsp;", "").Trim('<').Trim()),
            // 取得最近更新回數
            LastUpdateChapter = CharsetConvertUtil.ToTraditional(rLastUpdate_Chapter.Match(lastUpdate).Value.Replace("title=", "").Trim('"').Trim()),
            Caption = caption.TrimEscapeString(),
            Url = url.MakeUrlAbsolute(this.GetRoot().Url)
        };
        cn.LastUpdateChapter = cn.LastUpdateChapter.TrimComicName(caption);
        return cn;
    }

    public void LoadComics(ComicPagination pagination)
    {
        locker.Wait();
        try
        {
            var htmlContent = ComicUtil.GetUtf8Content(pagination.Url).Result;
            var comicList = RetriveComicNames(htmlContent);
            var results = comicList.Select(comic =>
            {
                var url = RetriveComicName_Url(comic);
                var caption = RetriveComicName_Caption(comic);
                var lastChapter = RetriveComicName_LastUpdateInfo(comic); // 取得最近更新回數
                var cn = new ComicEntity()
                {
                    IconUrl = RetriveComicName_IconUrl(comic), // 取得漫畫首頁圖像連結
                    LastUpdateDate = RetriveComicName_LastUpdateDate(comic), // 取得最近更新日期
                    LastUpdateChapter = lastChapter.TrimEscapeString(),
                    Url = url.MakeUrlAbsolute(this.GetRoot().Url),
                    Caption = caption.TrimEscapeString(),
                    Chapters = new List<ComicChapter>()
                };
                cn.LastUpdateChapter = cn.LastUpdateChapter.TrimComicName(cn.Caption);

                Task.Run(() => this.LoadIconImage(cn));
                Task.Run(() => this.LoadChapters(cn));
                return cn;
            }).ToList();
            pagination.Comics.AddRange(results);
        }
        finally
        {
            locker.Release();
        }
    }

    private void LoadIconImage(ComicEntity cn)
    {
        locker.Wait();
        try
        {
            using Stream iconData = ComicUtil.GetPicture(cn.IconUrl).Result;
            cn.IconImage = Image.FromStream(iconData);
        }
        finally
        {
            locker.Release();
        }
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
        Regex rCaptionOuter = new Regex(@"<h2 class=""title"">(.|\n)*?</h2>", RegexOptions.Compiled);
        Regex rCaptionInner = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);

        string simplified = rCaptionInner.Match(rCaptionOuter.Match(matchedData).Value).Value.Replace("title=", "").Trim('"');
        return CharsetConvertUtil.ToTraditional(simplified.Trim());
    }
    #endregion

    #region Chapters
    public void LoadChapters(ComicEntity comic)
    {
        locker.Wait();
        try
        {
            string htmlContent = ComicUtil.GetUtf8Content(comic.Url).Result;
            List<ComicChapter> results = new List<ComicChapter>();
            var chapters = RetriveChapters(htmlContent);
            chapters.ForEach(c =>
            {
                var url = RetriveChapter_Url(c);
                var caption = RetriveChapter_Caption(c);

                var cb = new ComicChapter()
                {
                    Url = url.MakeUrlAbsolute(this.GetRoot().Url),
                    Caption = caption.TrimEscapeString().TrimComicName(comic.Caption)
                };

                if (false == string.IsNullOrEmpty(cb.Caption))
                {
                    results.Add(cb);
                }
            });
            comic.Chapters.AddRange(results);
        }
        finally
        {
            locker.Release();
        }

    }

    private List<string> RetriveChapters(string htmlContent)
    {
        Regex comicListOuter = new Regex(@"<div id=""chapterlistload"">(.|\n)*?</ul>(.|\n)*?</div>", RegexOptions.Compiled);
        Regex comicListInner = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);
        //try
        //{
        //if (url == "https://www.dm5.com/manhua-biaosuzhainan/" ||
        //    url == "https://www.dm5.com/manhua-yidianxingyuan/")
        //{
        //    var a = "aaa";
        //}
        var allInOne = comicListOuter.Matches(htmlContent).Cast<Match>().Select(p => p.Value).First();
        var results = comicListInner.Matches(allInOne).Cast<Match>().Select(p => p.Value).ToList();
        return results;
        //}
        //catch (Exception e)
        //{
        //    var m = comicListOuter.Matches(htmlContent);
        //    Console.WriteLine(e.ToString());
        //    return null;
        //}
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
        Regex rCaption = new Regex(@"target=""_blank"".*?>(.|\n)*?<", RegexOptions.Compiled);
        var caption = rCaption.Match(chapter).Value.Trim();
        //var caption = rCaption.Match(chapter).Value.Replace(@"target=""_blank""", "").Trim();
        var simplified = ChopArrow(caption);

        if (string.IsNullOrEmpty(simplified))
        {
            Regex rCaptionPart2 = new Regex(@"p.{1,10}class=""title.{1,10}"" *?>(.|\n)*?<", RegexOptions.Compiled);
            //<p class="title ">第1回 要么突破，要么死！<span>（65P）</span></p>
            caption = rCaptionPart2.Match(chapter).Value.Trim();
            simplified = ChopArrow(caption);
        }
        return CharsetConvertUtil.ToTraditional(simplified);
    }

    private string ChopArrow(string caption)
    {
        var start = caption.IndexOf('>');
        var end = caption.IndexOf('<');
        var simplified = caption.Substring(start + 1, end - start - 1).Trim();
        return simplified;
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
    public void GetPages(ComicChapter chapter)
    {
        locker.Wait();
        try
        {
            // fix by
            // http://css122.us.cdndm.com/v201708091849/default/js/chapternew_v22.js
            //Regex rPages = new Regex(@"var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);
            string htmlContent = ComicUtil.GetUtf8Content(chapter.Url).Result;
            string cid = chapter.Url.Replace(@"https://www.dm5.com/m", "").Trim('/');
            string mid = RetrivePage_MID(htmlContent);
            string dt = RetrivePage_VIEWSIGNDT(htmlContent);
            string sign = RetrivePage_VIEWSIGN(htmlContent);
            int pageCount = RetrivePage_PageCount(htmlContent);

            //string jsCodeWrapper = "; var url = (typeof (hd_c) != \"undefined\" && hd_c.length > 0) ? hd_c[0] : d[0];";
            //string jsCodeWrapper = "; d[0];";
            List<ComicPage> pages = new List<ComicPage>();

            List<string> pageUrls;
            if (pageCount > 0)
            {
                var tasks = new List<Task<string>>();
                for (int i = 1; i <= pageCount; i++)
                {
                    string pageUrl = this._cRoot.Url + "m" + cid + "/" + "chapterfun.ashx?cid=" + cid + "&page=" + i.ToString() + "&language=1&gtk=6&_cid=" + cid + "&_mid=" + mid + "&_dt=" + dt + "&_sign=" + sign;
                    var refer = $"{this._cRoot.Url}m{cid}/";
                    Task<string> task = Task.Run(() => GetPageUrl(pageUrl, refer));
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());

                pageUrls = tasks.Select(p => p.Result).ToList();
                tasks.Clear();
            }
            else
            {
                pageUrls = RetrivePage_PageCount_Type2(htmlContent);
            }

            for (int i = 1; i <= pageUrls.Count; i++)
            {
                var refer = $"{this._cRoot.Url}m{cid}/";
                var page = GetComicPage(refer, pageUrls[i - 1], i);
                pages.Add(page);
            }
            //Regex rInvalidFileName = new Regex(string.Format("[{0}]", Regex.Escape(new String(System.IO.Path.GetInvalidFileNameChars()))));
            chapter.Pages = pages;
        }
        finally
        {
            locker.Release();
        }
    }

    private string GetPageUrl(string pageUrl, string refer)
    {
        locker.Wait();
        try
        {
            ComicUtil util = ComicUtil.CreateVsaEngine();

            // unpacker test url = 
            // url: 'chapterfun.ashx',
            // data: { cid: DM5_CID, page: DM5_PAGE, key: mkey, language: 1, gtk: 6, _cid: DM5_CID, _mid: DM5_MID, _dt: DM5_VIEWSIGN_DT, _sign: DM5_VIEWSIGN },
            string jsCodeWrapper = ";var url = (typeof (isrevtt) != \"undefined\" && isrevtt) ? hd_c[0] : d[0];";

            string pageFunContent = ComicUtil.GetUtf8Content(pageUrl, refer).Result; // 這個得到的是一串 eval(...)字串
            for (int j = 0; j <= 20; j++)
            {
                if (false == String.IsNullOrEmpty(pageFunContent) && false == pageFunContent.Contains("war|jpg"))
                {
                    break;
                }
                else
                {
                    pageFunContent = ComicUtil.GetUtf8Content(pageUrl).Result; // 這個得到的是一串 eval(...)字串
                }
            }

            pageFunContent = pageFunContent.Trim('"').Trim('\n');
            pageFunContent = pageFunContent.Substring(5, pageFunContent.Length - 6) + ";";
            string url;
            //string jsCode;
            //jsCode = util.EvalJScript(pageFunContent).ToString();
            //jsCodePass2 = util.EvalJScript("var isrevtt; var hd_c;" + jsCode + jsCodeWrapper).ToString();
            url = ((string[])util.EvalJScript(pageFunContent))[0];

            return url;
        }
        finally
        {
            locker.Release();
        }
    }

    private ComicPage GetComicPage(string refer, string pageUrl, int pageNumber)
    {
        ComicPage page = new ComicPage()
        {
            PageNumber = pageNumber,
            Refer = refer,
            Url = pageUrl,
            Caption = $"第{pageNumber.ToString().PadLeft(3, '0')}頁",
            PageFileName = $"{pageNumber.ToString().PadLeft(3, '0')}.{Path.GetExtension(pageUrl.Substring(0, pageUrl.IndexOf("?"))).TrimStart('.')}",
        };
        page.PageFileName = page.PageFileName.Replace("..", ".");
        return page;
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

    #region Download

    public void DownloadChapter(ComicChapter chapter, string downloadPath)
    {
        locker.Wait();
        try
        {

            if (false == Directory.Exists(downloadPath)) Directory.CreateDirectory(downloadPath);

            var tasks = new List<Task>();
            foreach (var page in chapter.Pages)
            {
                var task = Task.Run(() => DownloadPage(page, downloadPath));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
            tasks.Clear();
        }
        finally
        {
            locker.Release();
        }
    }

    private void DownloadPage(ComicPage page, string downloadPath)
    {
        locker.Wait();
        try
        {
            var fullPath = Path.Combine(downloadPath, page.PageFileName);
            using var stream = ComicUtil.GetPicture(page.Url, page.Refer).Result;
            using var fs = new FileStream(fullPath, FileMode.Create);
            stream.CopyTo(fs);

            fs.Close();
            stream.Close();
        }
        catch (Exception ex)
        {
            throw new Exception("下載漫畫圖片時發生錯誤，原因：" + ex);
        }
        finally
        {
            locker.Release();
        }
    }
    #endregion
}
using ComicCatcherLib.Utils;
using System.Text.RegularExpressions;

namespace ComicCatcherLib.ComicModels.Domains;

public class Dm5 : IComicCatcher
{
    public Dm5()
    {
        _cRoot = new ComicRoot()
        {
            Caption = "動漫屋(dm5)",
            Url = @"https://www.dm5.com/",
            WebSiteName = "dm5",
            IconHost = @"",
            PicHost = @"",
            PicHost2 = @"",
            PicHostAlternative = @"",

            ListState = ComicState.Created,

            ThreadCount = 80,
            ConnectionCount = 30,
            Paginations = new List<ComicPagination>()
        };
        HttpClientUtil.SetConnections(_cRoot.ConnectionCount);
        LoadPaginations();
    }

    public void Dispose()
    {
        _cRoot = null;
    }
    #region Root

    private ComicRoot _cRoot;

    public ComicRoot GetRoot()
    {
        return _cRoot;
    }
    #endregion

    #region Groups
    public void LoadPaginations()
    {
        if (GetRoot().ListState != ComicState.Created) return;

        GetRoot().ListState = ComicState.Processing;

        GetRoot().Paginations.Clear();
        var paginations = new List<ComicPagination>();
        for (int i = 1; i <= 300; ++i)
        {
            var url = $"manhua-list-s2-p{i}/";
            var pagination = new ComicPagination()
            {
                TabNumber = i,
                Caption = "第" + i.ToString().PadLeft(3, '0') + "頁",
                Url = url.MakeUrlAbsolute(GetRoot().Url),
                Comics = new List<ComicEntity>(),
                ListState = ComicState.Created
            };
            paginations.Add(pagination);
        }
        GetRoot().Paginations.AddRange(paginations);
        GetRoot().ListState = ComicState.ListLoaded;
    }
    #endregion

    #region Names

    public static ComicEntity CreateComic(string url) => new ComicEntity()
    {
        ListState = ComicState.Created,
        ImageState = ComicState.Created,
        Url = url,
        Chapters = new List<ComicChapter>(),
        Caption = "Comic",
        IconImage = null,
        IconUrl = "",
        LastUpdateChapter = "",
        LastUpdateDate = "",
    };

    public async Task<ComicEntity> GetSingleComicName(string url)
    {
        var htmlContent = await ComicUtil.GetUtf8Content(url);

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
            LastUpdateDate = Parse_LastUpdateDate(rLastUpdate_Date.Match(lastUpdate).Value.Replace("</a>&nbsp;", "").Trim('<').Trim()),
            // 取得最近更新回數
            LastUpdateChapter = CharsetConvertUtil.ToTraditional(rLastUpdate_Chapter.Match(lastUpdate).Value.Replace("title=", "").Trim('"').Trim()),
            Caption = caption.TrimEscapeString(),
            Url = url.MakeUrlAbsolute(GetRoot().Url)
        };
        cn.LastUpdateChapter = cn.LastUpdateChapter.TrimComicName(caption);
        return cn;
    }

    public async Task LoadComics(ComicPagination pagination, Dictionary<string, string> ignoreComics)
    {
        if (pagination.ListState != ComicState.Created) return;

        pagination.ListState = ComicState.Processing;
        pagination.Comics.Clear();

        var htmlContent = await ComicUtil.GetUtf8Content(pagination.Url);
        var comicList = RetriveComicNames(htmlContent);
        var results = comicList.Select(comicContent =>
        {
            var url = RetriveComicName_Url(comicContent);
            url = url.MakeUrlAbsolute(GetRoot().Url);
            if (ignoreComics.ContainsKey(url)) return null; // 忽略的回傳 null，等一下就過濾

            var caption = RetriveComicName_Caption(comicContent);
            var lastChapter = RetriveComicName_LastUpdateInfo(comicContent); // 取得最近更新回數
            var comic = new ComicEntity()
            {
                IconUrl = RetriveComicName_IconUrl(comicContent), // 取得漫畫首頁圖像連結
                LastUpdateDate = RetriveComicName_LastUpdateDate(comicContent), // 取得最近更新日期
                LastUpdateChapter = lastChapter.TrimEscapeString(),
                Url = url,
                Caption = caption.TrimEscapeString(),
                Chapters = new List<ComicChapter>(),
                ListState = ComicState.Created,
                ImageState = ComicState.Created
            };
            comic.LastUpdateChapter = comic.LastUpdateChapter.TrimComicName(comic.Caption);
            return comic;
        }).Where(c => c != null).ToList();

        pagination.Comics.AddRange(results);
        pagination.ListState = ComicState.Created;

        foreach (var comic in pagination.Comics)
        {
            Task.Run(async () => await LoadIconImage(comic));
            Task.Run(async () => await LoadChapters(comic));
        }
    }

    public async Task LoadComicsForWeb(ComicPagination pagination)
    {
        if (pagination.ListState != ComicState.Created) return;

        pagination.ListState = ComicState.Processing;
        pagination.Comics.Clear();

        var htmlContent = await ComicUtil.GetUtf8Content(pagination.Url);
        var comicList = RetriveComicNames(htmlContent);
        var results = comicList.Select(comicContent =>
        {
            var url = RetriveComicName_Url(comicContent);
            url = url.MakeUrlAbsolute(GetRoot().Url);

            var caption = RetriveComicName_Caption(comicContent);
            var lastChapter = RetriveComicName_LastUpdateInfo(comicContent); // 取得最近更新回數
            var comic = new ComicEntity()
            {
                IconUrl = RetriveComicName_IconUrl(comicContent), // 取得漫畫首頁圖像連結
                LastUpdateDate = RetriveComicName_LastUpdateDate(comicContent), // 取得最近更新日期
                LastUpdateChapter = lastChapter.TrimEscapeString(),
                Url = url,
                Caption = caption.TrimEscapeString(),
                Chapters = new List<ComicChapter>(),
                ListState = ComicState.Created,
                ImageState = ComicState.Created
            };
            comic.LastUpdateChapter = comic.LastUpdateChapter.TrimComicName(comic.Caption);
            return comic;
        }).Where(c => c != null).ToList();

        pagination.Comics.AddRange(results);
        pagination.ListState = ComicState.ListLoaded;
    }

    private async Task LoadIconImage(ComicEntity comic)
    {
        if (comic.ImageState != ComicState.Created) return;

        comic.ImageState = ComicState.Processing;
        try
        {
            comic.IconImage = null;
            using Stream iconData = await ComicUtil.GetPicture(comic.IconUrl);
            comic.IconImage = new MemoryStream();
            iconData.CopyTo(comic.IconImage);
            comic.ImageState = ComicState.ImageLoaded;
        }
        catch (Exception ex)
        {
            comic.ImageState = ComicState.ListError;
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
        var lastUpdateDate = rLastUpdateDate.Match(matchedData).Value.Replace(@"<p class=""zl"">", "").Replace("</p>", "").Trim();
        lastUpdateDate = Parse_LastUpdateDate(lastUpdateDate);
        return lastUpdateDate;
    }

    private string Parse_LastUpdateDate(string lastUpdateDate)
    {
        lastUpdateDate = lastUpdateDate.Replace("更新", "").Trim();
        DateTime lastTime;
        if (DateTime.TryParse(lastUpdateDate, out lastTime))
        {
            return lastTime.ToString("yyyy/MM/dd");
        }

        if (DateTime.TryParseExact(DateTime.Now.Year + "年" + lastUpdateDate, "yyyy年MM月dd号", null,
                System.Globalization.DateTimeStyles.None, out lastTime))
        {
            return lastTime.ToString("yyyy/MM/dd");
        }

        var now = DateTime.UtcNow.AddHours(8); // GMT+8
        if (lastUpdateDate.Contains("前天"))
        {
            return now.AddDays(-2).ToString("yyyy/MM/dd") + " " + lastUpdateDate.Replace("前天", "").Trim() + ":00";
        }
        if (lastUpdateDate.Contains("昨天"))
        {
            return now.AddDays(-1).ToString("yyyy/MM/dd") + " " + lastUpdateDate.Replace("昨天", "").Trim() + ":00";
        }
        if (lastUpdateDate.Contains("今天"))
        {
            return now.ToString("yyyy/MM/dd") + " " + lastUpdateDate.Replace("今天", "").Trim() + ":00";
        }

        if (lastUpdateDate.Contains("分钟前更新"))
        {
            if (int.TryParse(lastUpdateDate.Replace("分钟前更新", "").Trim(), out int minute))
            {
                now.AddMinutes(-minute).ToString("yyyy/MM/dd HH:mm:ss");
            }
        }
        return lastUpdateDate;
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
    public async Task LoadChapters(ComicEntity comic)
    {
        if (comic.ListState != ComicState.Created) return;

        comic.ListState = ComicState.Processing;
        try
        {
            comic.Chapters.Clear();

            string htmlContent = await ComicUtil.GetUtf8Content(comic.Url);
            List<ComicChapter> results = new List<ComicChapter>();
            var chapters = RetriveChapters(htmlContent);
            var reverse = htmlContent.Contains(">正序<");
            if (reverse) chapters.Reverse();
            chapters.ForEach(c =>
            {
                var url = RetriveChapter_Url(c);
                var caption = RetriveChapter_Caption(c);

                var cb = new ComicChapter()
                {
                    Url = url.MakeUrlAbsolute(GetRoot().Url),
                    Caption = caption.TrimEscapeString().TrimComicName(comic.Caption),
                    Pages = new List<ComicPage>()
                };

                if (false == string.IsNullOrEmpty(cb.Caption))
                {
                    results.Add(cb);
                }
            });
            comic.Chapters.AddRange(results);
        }
        catch (Exception ex)
        {
            comic.ImageState = ComicState.ListError;
        }
        finally
        {
            comic.ListState = ComicState.ListLoaded;
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
    public async Task LoadPages(ComicChapter chapter)
    {
        chapter.Pages.Clear();
        // fix by
        // http://css122.us.cdndm.com/v201708091849/default/js/chapternew_v22.js
        //Regex rPages = new Regex(@"var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);
        string htmlContent = await ComicUtil.GetUtf8Content(chapter.Url);
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
                string pageUrl = _cRoot.Url + "m" + cid + "/" + "chapterfun.ashx?cid=" + cid + "&page=" + i.ToString() + "&language=1&gtk=6&_cid=" + cid + "&_mid=" + mid + "&_dt=" + dt + "&_sign=" + sign;
                var refer = $"{_cRoot.Url}m{cid}/";
                Task<string> task = GetPageUrl(pageUrl, refer);
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
            var refer = $"{_cRoot.Url}m{cid}/";
            var page = GetComicPage(refer, pageUrls[i - 1], i);
            pages.Add(page);
        }
        //Regex rInvalidFileName = new Regex(string.Format("[{0}]", Regex.Escape(new String(System.IO.Path.GetInvalidFileNameChars()))));
        chapter.Pages = pages;
        if (chapter.Pages.Count > 0) chapter.ListState = ComicState.ListLoaded;

    }

    private async Task<string> GetPageUrl(string pageUrl, string refer)
    {
        ComicUtil util = ComicUtil.CreateVsaEngine();

        // unpacker test url = 
        // url: 'chapterfun.ashx',
        // data: { cid: DM5_CID, page: DM5_PAGE, key: mkey, language: 1, gtk: 6, _cid: DM5_CID, _mid: DM5_MID, _dt: DM5_VIEWSIGN_DT, _sign: DM5_VIEWSIGN },
        string jsCodeWrapper = ";var url = (typeof (isrevtt) != \"undefined\" && isrevtt) ? hd_c[0] : d[0];";

        string pageFunContent = await ComicUtil.GetUtf8Content(pageUrl, refer); // 這個得到的是一串 eval(...)字串
        for (int j = 0; j <= 20; j++)
        {
            if (false == string.IsNullOrEmpty(pageFunContent) && false == pageFunContent.Contains("war|jpg"))
            {
                break;
            }
            else
            {
                pageFunContent = await ComicUtil.GetUtf8Content(pageUrl); // 這個得到的是一串 eval(...)字串
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
    public async Task DownloadChapter(DownloadChapterRequest request)
    {
        await LoadPages(request.Chapter);

        if (request.Chapter != null)
        {
            request.ReportProgressAction(0, $"[{request.Name}]準備開始下載，共{request.Chapter.Pages.Count}頁");
        }


        if (false == Directory.Exists(request.Path)) Directory.CreateDirectory(request.Path);

        var tasks = new List<Task>();
        foreach (var page in request.Chapter.Pages)
        {
            var task = DownloadPage(request, page);
            tasks.Add(task);
        }
        Task.WaitAll(tasks.ToArray());
        tasks.Clear();
    }

    private async Task DownloadPage(DownloadChapterRequest request, ComicPage page)
    {
        try
        {
            var fullPath = Path.Combine(request.Path, page.PageFileName);
            using var stream = await ComicUtil.GetPicture(page.Url, page.Refer);
            using var fs = new FileStream(fullPath, FileMode.Create);
            await stream.CopyToAsync(fs);

            fs.Close();
            stream.Close();
            //tagname = "[" + ((DownloadPictureScheduler)scheduler).name + "]"
            //string ThreadID = " Thread ID=[" + Thread.CurrentThread.GetHashCode().ToString() + "]";
            //bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = tagname + pictureName + "下載中...", infoMsg = tagname + pictureName + "下載中..." + "[" + pictureUrl + "]" + ThreadID });
            if (request.ReportProgressAction != null)
            {
                request.ReportProgressAction(0, $"[{request.Name}] {page.PageFileName}下載中...");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("下載漫畫圖片時發生錯誤，原因：" + ex);
        }
    }
    #endregion
}
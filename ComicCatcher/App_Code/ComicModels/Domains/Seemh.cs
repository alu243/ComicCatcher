using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ComicCatcher.App_Code.ComicModels.Domains;
using Utils;

namespace ComicCatcher.ComicModels.Domains;

public class Seemh : IComicCatcher
{
    public void Dispose()
    {
        this._cRoot = null;
    }

    #region Root
    private ComicRoot _cRoot = new ComicRoot()
    {
        Caption = "看漫畫(Seemh)",
        WebSiteName = "seemh",
        Url = @"http://www.seemh.com/",
        IconHost = @"",
        PicHost = @"",
        PicHost2 = @"",

        PicHostAlternative = @"",

        ThreadCount = 40
    };

    public ComicRoot GetRoot()
    {
        return this._cRoot;
    }
    #endregion

    #region Groups
    public List<ComicPagination> GetPaginations()
    {
        List<ComicPagination> webPages = new List<ComicPagination>();
        for (int i = 0; i < 300; ++i)
        {
            // http://www.seemh.com/
            ComicPagination wp = new ComicPagination();
            wp.TabNumber = i + 1;
            wp.Caption = "第" + (i + 1).ToString().PadLeft(3, '0') + "頁";
            if (i <= 6)
                wp.Url = new Uri(new Uri(this._cRoot.Url), ("update/d7.html")).ToString();
            else
                wp.Url = new Uri(new Uri(this._cRoot.Url), ("update/d" + (i + 1).ToString() + ".html")).ToString();
            webPages.Add(wp);
        }
        return webPages;
    }
    #endregion

    #region Names
    public List<ComicEntity> GetComics(ComicPagination cGroup, bool isLoadPicture)
    {
        Regex rLink = new Regex(@"<a (.|\n)*?<strong>(.|\n)*?</a>", RegexOptions.Compiled);
        Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
        Regex rCaption = new Regex(@"<strong>(.|\n)*?<", RegexOptions.Compiled);
        string htmlContent = ComicUtil.GetUtf8Content(cGroup.Url);
        htmlContent = RetriveCurrentDateString(htmlContent, cGroup.TabNumber);
        List<string> comicList = SplitForComicName(htmlContent);
        List<ComicEntity> result = comicList.Select<string, ComicEntity>(comic =>
        {
            string sLink = rLink.Match(comic).Value;
            ComicEntity cn = new ComicEntity()
            {
                IconUrl = RetriveIconUri(comic), // 取得漫畫首頁圖像連結
                LastUpdateDate = RetriveLastUpdateDate(comic), // 取得最近更新日期
                LastUpdateChapter = RetriveLastUpdateInfo(comic), // 取得最近更新回數
                Url = rUrl.Match(sLink).Value.Replace("href=", "").Trim('"').Trim(),
                Caption = CharsetConvertUtil.ToTraditional(rCaption.Match(sLink).Value.Replace("<strong>", "").Trim('<'))
                //foreach (char c in Path.GetInvalidFileNameChars()) cb.description = cb.description.Replace(c.ToString(), "");
            };

            if (false == String.IsNullOrEmpty(cn.LastUpdateChapter))
            {
                cn.LastUpdateChapter = cn.LastUpdateChapter.Replace(cn.Caption, String.Empty).Trim();
            }

            if (Uri.IsWellFormedUriString(cn.Url, UriKind.Absolute) == false) cn.Url = (new Uri(new Uri(this._cRoot.Url), cn.Url)).ToString();
            return cn;
        }).ToList();

        return result;
    }

    private string RetriveCurrentDateString(string htmlContent, int groupNumber)
    {
        Regex rCurrDate = new Regex(@"<div class=""latest\-list"">(.|\n)*?</div>", RegexOptions.Compiled);
        return rCurrDate.Matches(htmlContent)[groupNumber - 1].Value;
    }

    private List<string> SplitForComicName(string htmlContent)
    {
        Regex rComicList = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);
        return rComicList.Matches(htmlContent).Cast<Match>().Select(p => p.Value).ToList();
    }

    private string RetriveIconUri(string matchedData)
    {
        Regex rIconUri = new Regex(@"<img src=""(.|\n)*?""", RegexOptions.Compiled);
        Uri iconUri = new Uri(rIconUri.Match(matchedData).Value.Replace("<img src=", "").Replace(@"""", "").Trim());
        return iconUri.ToString();
    }

    private string RetriveLastUpdateDate(string matchedData)
    {
        Regex rLastUpdateDate = new Regex(@"\d{4}年\d{1,2}月\d{1,2}日：", RegexOptions.Compiled);
        return rLastUpdateDate.Match(matchedData).Value;
    }

    private string RetriveLastUpdateInfo(string matchedData)
    {
        Regex rLastUpdateInfoOuter = new Regex(@"\[(.|\n)*?\]", RegexOptions.Compiled);
        Regex rLastUpdateInfoInner = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);
        string simplified = rLastUpdateInfoInner.Match(rLastUpdateInfoOuter.Match(matchedData).Value).Value.Replace("title=", "").Trim('"');
        return CharsetConvertUtil.ToTraditional(simplified);
    }

    #endregion

    #region Chapters
    public List<ComicChapter> GetChapters(ComicEntity cEntity)
    {
        //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        Regex rVolumnList = new Regex(@"<li {0,}(.|\n)*?</li>", RegexOptions.Compiled);
        Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
        Regex rCaption = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);

        string htmlContent = ComicUtil.GetUtf8Content(cEntity.Url);
        List<ComicChapter> result = new List<ComicChapter>();
        string tagContent = RetriveHtmlTagContent(htmlContent);
        foreach (Match volumn in rVolumnList.Matches(tagContent))
        {
            //string sLink = rLink.Match(data.Value).Value;
            string sLink = volumn.Value;
            ComicChapter cb = new ComicChapter()
            {
                Url = rUrl.Match(sLink).Value.Replace("href=", "").Trim('"'),
                //description = CharsetConverter.ToTraditional(rDesc.Match(sLink).Value.Replace(@"<span class=""black"">", "").Replace(@"</span>", "")
                //.Replace(@"<fontcolor=red>", "").Replace(@"</font>", "").Replace(@"<b>","").Replace(@"</b>","").Trim())
                Caption = CharsetConvertUtil.ToTraditional(rCaption.Match(sLink).Value.Replace("title=", "").Trim('"'))
            };
            if (Uri.IsWellFormedUriString(cb.Url, UriKind.Absolute) == false) cb.Url = (new Uri(new Uri(this._cRoot.Url), cb.Url)).ToString();

            if (false == String.IsNullOrEmpty(cb.Caption))
            {
                cb.Caption = cb.Caption.Replace(cEntity.Caption, String.Empty).Trim();
                result.Add(cb);
            }
        }

        return result;
    }

    private string RetriveHtmlTagContent(string htmlContent)
    {
        // <ul class="nr6 lan2" id="cbc_1"> 
        Regex rVolumnTag = new Regex(@"<ul class=""nr6 lan2"" id=""cbc(.|\n)*?</ul>", RegexOptions.Compiled);
        StringBuilder sb = new StringBuilder();
        foreach (Match match in rVolumnTag.Matches(htmlContent))
        {
            sb.AppendLine(match.Value);
        }
        return sb.ToString();
    }
    #endregion

    #region Pages
    //////public List<ComicPage> GetPages(ComicChapter cChapter)
    //////{
    //////    //Regex rPages = new Regex(@"var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);
    //////    string cid = cChapter.Url.Replace(@"http://www.dm5.com/m", "").Trim('/');
    //////    string htmlContent = ComicUtil.GetUtf8Content(cChapter.Url);

    //////    Regex rPageLinkOuter = new Regex(@"<select (.|\n)*?id=""pagelist""(.|\n)*?</select>", RegexOptions.Compiled);
    //////    Regex rPageLinkInner = new Regex(@"<option {1,}(.|\n)*?</option>", RegexOptions.Compiled);
    //////    string pageOuter = rPageLinkOuter.Matches(htmlContent)[0].Value;
    //////    int pageCount = rPageLinkInner.Matches(pageOuter).Count;


    //////    //Regex rKey = new Regex(@"key=.+?;", RegexOptions.Compiled);
    //////    //Regex rPhotoServer = new Regex(@"pix="".+?""", RegexOptions.Compiled);
    //////    Regex rPageFile = new Regex(@"pvalue=\["".+?""", RegexOptions.Compiled);
    //////    List<ComicPage> pages = new List<ComicPage>();
    //////    for (int i = 1; i <= pageCount; i++)
    //////    {
    //////        //string pageFunUrl = this._cRoot.WebHost + "imagefun.ashx?cid=" + cid + "&page=" + i.ToString();
    //////        string pageFunUrl = this._cRoot.WebHost + "chapterfun.ashx?cid=" + cid + "&page=" + i.ToString();

    //////        string pageFunContent = ComicUtil.GetUtf8Content(pageFunUrl); // 這個得到的是一串 eval(...)字串
    //////        pageFunContent = pageFunContent.Trim('"').Trim('\n');
    //////        pageFunContent = pageFunContent.Substring(5, pageFunContent.Length - 6);
    //////        string jsCode = ComicUtil.EvalJScript("var cs = " + pageFunContent).ToString();

    //////        //string key = rKey.Match(jsCode).Value.Replace("key=", "").Trim(new char[] { '"', '\'' });
    //////        //string photoServer = rPhotoServer.Match(jsCode).Value.Replace("pix=", "").Trim(new char[] { '"', '\'' });
    //////        string pageFile = rPageFile.Match(jsCode).Value.Replace("pvalue=[", "").Trim(new char[] { '"', '\'' });

    //////        string key = GetJSVariableValue(jsCode, "key");
    //////        string photoServer = GetJSVariableValue(jsCode, "pix");
    //////        string ak = GetJSVariableValue(jsCode, "ak");

    //////        ComicPage page = new ComicPage();
    //////        page.PageNumber = i;
    //////        page.Url = photoServer + pageFile + "?cid=" + cid + "&key=" + key + "&ak=" + ak;
    //////        page.Caption = "第" + i.ToString().PadLeft(3, '0') + "頁";
    //////        page.PageFileName = i.ToString().PadLeft(3, '0') + "." + System.IO.Path.GetExtension(pageFile);
    //////        page.PageFileName = page.PageFileName.Replace("..", ".");

    //////        pages.Add(page);
    //////    }
    //////    Regex rInvalidFileName = new Regex(string.Format("[{0}]", Regex.Escape(new String(System.IO.Path.GetInvalidFileNameChars()))));
    //////    return pages;
    //////}

    //////private string GetJSVariableValue(string jsContent, string variableName)
    //////{
    //////    Regex vn = new Regex(variableName + @"=.+?;", RegexOptions.Compiled);
    //////    return vn.Match(jsContent).Value.Replace(variableName + "=", "").TrimEnd(';').Trim(new char[] { '"', '\'' });
    //////}


    public List<ComicPage> GetPages(ComicChapter cChapter)
    {
        //Regex rPages = new Regex(@"var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);
        string cid = cChapter.Url.Replace(@"http://www.dm5.com/m", "").Trim('/');
        string htmlContent = ComicUtil.GetUtf8Content(cChapter.Url);

        Regex rPageLinkOuter = new Regex(@"<select (.|\n)*?id=""pagelist""(.|\n)*?</select>", RegexOptions.Compiled);
        Regex rPageLinkInner = new Regex(@"<option {1,}(.|\n)*?</option>", RegexOptions.Compiled);
        string pageOuter = rPageLinkOuter.Matches(htmlContent)[0].Value;
        int pageCount = rPageLinkInner.Matches(pageOuter).Count;

        ComicUtil util = ComicUtil.CreateVsaEngine();
        //string jsCodeWrapper = "; var url = (typeof (hd_c) != \"undefined\" && hd_c.length > 0) ? hd_c[0] : d[0];";
        //string jsCodeWrapper = "; d[0];";
        string jsCodeWrapper = ";var url = (typeof (isrevtt) != \"undefined\" && isrevtt) ? hd_c[0] : d[0];";
        List<ComicPage> pages = new List<ComicPage>();
        for (int i = 1; i <= pageCount; i++)
        {
            //string pageFunUrl = this._cRoot.WebHost + "imagefun.ashx?cid=" + cid + "&page=" + i.ToString();
            // unpacker test url = 
            string pageFunUrl = this._cRoot.Url + "chapterfun.ashx?cid=" + cid + "&page=" + i.ToString();
            string pageFunContent = ComicUtil.GetUtf8Content(pageFunUrl); // 這個得到的是一串 eval(...)字串
            pageFunContent = pageFunContent.Trim('"').Trim('\n');
            pageFunContent = pageFunContent.Substring(5, pageFunContent.Length - 6);
            string jsCode = util.EvalJScript("var cs = " + pageFunContent).ToString();

            string jsCodePass2 = util.EvalJScript("var isrevtt; var hd_c;" + jsCode + jsCodeWrapper).ToString();
            ComicPage page = new ComicPage();
            page.PageNumber = i;
            //page.Url = photoServer + pageFile + "?cid=" + cid + "&key=" + key + "&ak=" + ak;
            page.Url = jsCodePass2;
            page.Caption = "第" + i.ToString().PadLeft(3, '0') + "頁";
            page.PageFileName = i.ToString().PadLeft(3, '0') + "." + System.IO.Path.GetExtension(page.Url.Substring(0, page.Url.IndexOf("?"))).TrimStart('.');
            page.PageFileName = page.PageFileName.Replace("..", ".");

            pages.Add(page);
        }
        Regex rInvalidFileName = new Regex(string.Format("[{0}]", Regex.Escape(new String(System.IO.Path.GetInvalidFileNameChars()))));
        return pages;
    }

    private string GetJSVariableValue(string jsContent, string variableName)
    {
        Regex vn = new Regex(variableName + @"=.+?;", RegexOptions.Compiled);
        return vn.Match(jsContent).Value.Replace(variableName + "=", "").TrimEnd(';').Trim(new char[] { '"', '\'' });
    }
    #endregion
}
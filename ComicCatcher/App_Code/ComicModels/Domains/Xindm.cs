using ComicCatcher.ComicModels.Domains;
using ComicCatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ComicCatcher.ComicModels.Domains;

public class Xindm : IComicCatcher
{
    public void Dispose()
    {
        this._cRoot = null;
    }

    string webimgServerURL = String.Empty;
    string nativePicServer = String.Empty;
    string sonPicServer = String.Empty;

    #region Root
    private ComicRoot _cRoot = new ComicRoot()
    {
        Caption = "新動漫(xindm)",
        WebSiteName = "xindm",
        Url = @"http://www.xindm.cn/mh",
        //PicHost = @"http://mh.xindm.cn/", // Old
        IconHost = @"http://www.xindm.cn/",
        PicHost = @"http://beiyong.bukamh.com/",
        //PicHost = @"http://imgsxsq.bukamh.com/",
        PicHost2 = @"http://mh.xindm.cn/",
        PicHostAlternative = @"http://imgsxsq.bukamh.com/",

        ThreadCount = 10
    };

    private string GetWebimgServerURL()
    {
        if (false == string.IsNullOrEmpty(this.webimgServerURL)) return this.webimgServerURL;

        string content = ComicUtil.GetGbContent("http://www.xindm.cn/skin/v2/2014/js/server.js");
        Regex rhost = new Regex(@"WebimgServerURL\[0\].*?;", RegexOptions.Compiled);
        string hostName = rhost.Match(content).Value.Replace("WebimgServerURL[0]", "").Replace(";", "").Replace("=", "")
            .Trim().Trim('\"').Trim().TrimEnd('/') + "/";
        this.webimgServerURL = hostName;
        return this.webimgServerURL;
    }

    private string GetNativePicServer()
    {
        if (false == string.IsNullOrEmpty(this.nativePicServer)) return this.nativePicServer;

        string content = ComicUtil.GetGbContent("http://www.xindm.cn/skin/v2/2014/js/server.js");
        Regex rhost = new Regex(@"NativePicServer.*?;", RegexOptions.Compiled);
        string hostName = rhost.Match(content).Value.Replace("NativePicServer", "").Replace(";", "").Replace("=", "")
            .Trim().Trim('\"').Trim().TrimEnd('/') + "/";
        this.nativePicServer = hostName;
        return this.nativePicServer;
    }

    private string GetSonPicServer()
    {
        if (false == string.IsNullOrEmpty(this.sonPicServer)) return this.sonPicServer;

        string content = ComicUtil.GetGbContent("http://www.xindm.cn/skin/v2/2014/js/server.js");
        Regex rhost = new Regex(@"SonPicServer.*?;", RegexOptions.Compiled);
        string hostName = rhost.Match(content).Value.Replace("SonPicServer", "").Replace(";", "").Replace("=", "")
            .Trim().Trim('\"').Trim().TrimEnd('/') + "/";
        this.sonPicServer = hostName;
        return this.sonPicServer;
    }


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
            ComicPagination wp = new ComicPagination();
            wp.TabNumber = i + 1;
            wp.Caption = "第" + (i + 1).ToString().PadLeft(3, '0') + "頁";
            wp.Url = this._cRoot.Url + ("/index_" + (i + 1).ToString() + ".html").Replace("index_1.html", "index.html");
            webPages.Add(wp);
        }
        return webPages;
    }
    #endregion

    #region Names
    public List<ComicEntity> GetComics(ComicPagination cGroup, bool isLoadPicture)
    {
        Regex rLink = new Regex(@"<a (.|\n)*?>", RegexOptions.Compiled);
        Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
        Regex rCaption = new Regex(@"title=""(.|\n)*?""", RegexOptions.Compiled);

        string htmlContent = ComicUtil.GetGbContent(cGroup.Url);

        List<string> comicList = SplitForComicName(htmlContent);
        List<ComicEntity> result = comicList.Select<string, ComicEntity>(comic =>
        {
            string sLink = rLink.Match(comic).Value;
            ComicEntity cn = new ComicEntity()
            {
                IconUrl = RetriveIconUri(comic), // 取得漫畫首頁圖像連結
                LastUpdateDate = RetriveLastUpdateDate(comic), // 取得最近更新日期
                LastUpdateChapter = RetriveLastUpdateInfo(comic), // 取得最近更新回數
                Url = rUrl.Match(sLink).Value.Replace("href=", "").Replace(@"""", "").Trim(),
                Caption = CharsetConvertUtil.ToTraditional(rCaption.Match(sLink).Value.Replace("title=", "").Replace(@"""", "").Trim())
                //foreach (char c in Path.GetInvalidFileNameChars()) cb.description = cb.description.Replace(c.ToString(), "");
            };
            if (Uri.IsWellFormedUriString(cn.Url, UriKind.Absolute) == false) cn.Url = (new Uri(new Uri(this._cRoot.Url), cn.Url)).ToString();
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
        Uri iconUri = new Uri(new Uri(this._cRoot.IconHost),
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
    public List<ComicChapter> GetChapters(ComicEntity cEntity)
    {
        //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        Regex rVolumnList = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);
        //private readonly static Regex rLink = new Regex(@"<a (.|\n)*?>", RegexOptions.Compiled);
        Regex rUrl = new Regex(@"onclick=""window.open\('(.|\n)*?'", RegexOptions.Compiled);
        Regex rCleanTag = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
        Regex rCaption = new Regex(@"<span class=""(black|red)"">.+?</span>", RegexOptions.Compiled);

        string htmlContent = ComicUtil.GetGbContent(cEntity.Url);
        List<ComicChapter> result = new List<ComicChapter>();
        string tagContent = RetriveHtmlTagContent(htmlContent);
        foreach (Match data in rVolumnList.Matches(tagContent))
        {
            //string sLink = rLink.Match(data.Value).Value;
            string sLink = data.Value;
            ComicChapter cb = new ComicChapter()
            {
                Url = rUrl.Match(sLink).Value.Replace("onclick=\"window.open(", "").Trim('\''),
                //description = CharsetConverter.ToTraditional(rDesc.Match(sLink).Value.Replace(@"<span class=""black"">", "").Replace(@"</span>", "")
                //.Replace(@"<fontcolor=red>", "").Replace(@"</font>", "").Replace(@"<b>","").Replace(@"</b>","").Trim())
                Caption = CharsetConvertUtil.ToTraditional(rCleanTag.Replace(rCaption.Match(sLink).Value.Trim(), ""))
            };
            if (Uri.IsWellFormedUriString(cb.Url, UriKind.Absolute) == false) cb.Url = (new Uri(new Uri(this._cRoot.Url), cb.Url)).ToString();

            if (false == String.IsNullOrEmpty(cb.Caption))
                result.Add(cb);
        }

        return result;
    }

    private string RetriveHtmlTagContent(string htmlContent)
    {
        Regex rMhList = new Regex(@"id=""mhlist"">.+?</div>", RegexOptions.Compiled);
        Regex rVolumnTag = new Regex(@"<ul>(.|\n)*?</ul>", RegexOptions.Compiled);
        StringBuilder sb = new StringBuilder();
        //string sMhListStr = rMhList.Match(htmlContent).ToString();
        //foreach (Match match in rVolumnTag.Matches(sMhListStr))
        foreach (Match match in rVolumnTag.Matches(htmlContent))
        {
            sb.AppendLine(match.Value);
        }
        return sb.ToString();
    }
    #endregion

    #region Pages
    public List<ComicPage> GetPages(ComicChapter cChapter)
    {
        //Regex rPages = new Regex(@"var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);
        Regex rJS = new Regex(@"<script(.|\n)*?</script>", RegexOptions.Compiled);
        Regex rEval = new Regex(@"eval(.+)", RegexOptions.Compiled);
        //  var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);
        Regex rPages = new Regex(@"\'.+?\'", RegexOptions.Compiled);

        string htmlContent = ComicUtil.GetGbContent(cChapter.Url);
        //string htmlContent = ComicUtil.GetContent(@"http://www.xindm.cn/mh/Soul%20Catcher/109903.html");
        string jsCode = String.Join(Environment.NewLine, rJS.Matches(htmlContent).Cast<Match>().ToList().Select(m => m.ToString()).ToArray());
        string evalCode = rEval.Matches(jsCode)[0].ToString();
        evalCode = evalCode.Substring(5, evalCode.Length - 6);
        ComicUtil util = ComicUtil.CreateVsaEngine();
        string photoStr = util.EvalJScript("var cs = " + evalCode).ToString();

        List<ComicPage> pages = new List<ComicPage>();
        int i = 1;
        foreach (Match match in rPages.Matches(photoStr))
        {
            ComicPage page = new ComicPage();
            //string tmp = match.ToString().Trim('\'').Replace("/pic.php?url=http%3A%2F%2Fimages.dmzj.com%2F", "").Replace("+", " ");
            string tmp = match.ToString().Trim('\'');
            //tmp = System.Web.HttpUtility.UrlDecode(tmp, System.Text.Encoding.GetEncoding("gb2312"));
            //tmp = System.Web.HttpUtility.UrlEncode(tmp, System.Text.Encoding.GetEncoding("gb2312"));

            page.Url = GetPageUrl(match.ToString());
            //page.Url = this._cRoot.PicHost.TrimEnd('/') + "/" + tmp.TrimStart('/');

            page.Caption = "第" + i.ToString().PadLeft(3, '0') + "頁";

            //page.PageFileName = System.IO.Path.GetFileName(System.Web.HttpUtility.UrlDecode(page.Url, System.Text.Encoding.GetEncoding("gb2312")));
            string pageFile = System.IO.Path.GetFileName(System.Web.HttpUtility.UrlDecode(page.Url, System.Text.Encoding.GetEncoding("gb2312")));
            page.PageFileName = i.ToString().PadLeft(3, '0') + "." + System.IO.Path.GetExtension(pageFile);
            page.PageFileName = page.PageFileName.Replace("..", ".");

            pages.Add(page);
            i++;
        }

        Regex rInvalidFileName = new Regex(string.Format("[{0}]", Regex.Escape(new String(System.IO.Path.GetInvalidFileNameChars()))));
        pages.ForEach(p =>
        {
            p.PageFileName = rInvalidFileName.Replace(p.PageFileName, "");
        });

        return pages;

        //string js = @"var arr=[]; eval(function(p,a,c,k,e,d){e=function(c){return(c<a?'':e(parseInt(c/a)))+((c=c%a)>35?String.fromCharCode(c+29):c.toString(36))};if(!''.replace(/^/,String)){while(c--){d[e(c)]=k[c]||e(c)}k=[function(e){return d[e]}];e=function(){return'\\w+'};c=1};while(c--){if(k[c]){p=p.replace(new RegExp('\\b'+e(c)+'\\b','g'),k[c]); arr.push(p);}}return p}('L h=S T();h[0]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%U.p\';h[1]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%V.p\';h[2]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%X.p\';h[3]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%Y.p\';h[4]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1b.p\';h[5]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1a.p\';h[6]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%Z.p\';h[7]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1c.p\';h[8]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%N.p\';h[9]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%J.p\';h[10]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%K.p\';h[11]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%I.p\';h[12]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%H.p\';h[13]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%R.p\';h[14]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%Q.p\';h[15]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%M.p\';h[16]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%O.p\';h[17]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%P.p\';h[18]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%W.p\';h[19]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1w.p\';h[1v]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1t.p\';h[1s]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1u.p\';h[1x]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1d.p\';h[1q]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1i.p\';h[1r]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1h.p\';h[1g]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1e.p\';h[1f]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1j.p\';h[1k]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%b%G%c%G%1p%1o%1n%1l%1m.p\';',62,96,'||||||||||B5|2F|C2|D7|C3|D3|A5|ArrayPhoto|2Fs|AB|C7|EB|com|2Fimages|pic|jpg|php|url|3A|http|dmzj|CB|D4|DA1|BB|E8|B9|B0_1411651600|C4|B0|C1|AE|D0|2FP017|2FP016|2FP014|2FP015|var|2FP020|2FP013|2FP021|2FP022|2FP019|2FP018|new|Array|2FP003|2FP004|2FP023|2FP007|2FP008|2FP011|||||||||||2FP010|2FP009|2FP012|2FP027|2FP030|26|25|2FP029|2FP028|2FP032|27|ED|93|F7|82|FB|23|24|21|2FP025|2FP026|20|2FP024|22'.split('|'),0,{})); var b = arr;";
        //string js = @"var cs = function(p,a,c,k,e,d){e=function(c){return(c<a?'':e(parseInt(c/a)))+((c=c%a)>35?String.fromCharCode(c+29):c.toString(36))};if(!''.replace(/^/,String)){while(c--){d[e(c)]=k[c]||e(c)}k=[function(e){return d[e]}];e=function(){return'\\w+'};c=1};while(c--){if(k[c]){p=p.replace(new RegExp('\\b'+e(c)+'\\b','g'),k[c])}}return p}('L h=S T();h[0]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%U.p\';h[1]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%V.p\';h[2]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%X.p\';h[3]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%Y.p\';h[4]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1b.p\';h[5]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1a.p\';h[6]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%Z.p\';h[7]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1c.p\';h[8]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%N.p\';h[9]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%J.p\';h[10]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%K.p\';h[11]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%I.p\';h[12]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%H.p\';h[13]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%R.p\';h[14]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%Q.p\';h[15]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%M.p\';h[16]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%O.p\';h[17]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%P.p\';h[18]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%W.p\';h[19]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1w.p\';h[1v]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1t.p\';h[1s]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1u.p\';h[1x]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1d.p\';h[1q]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1i.p\';h[1r]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1h.p\';h[1g]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1e.p\';h[1f]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1j.p\';h[1k]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%b%G%c%G%1p%1o%1n%1l%1m.p\';',62,96,'||||||||||B5|2F|C2|D7|C3|D3|A5|ArrayPhoto|2Fs|AB|C7|EB|com|2Fimages|pic|jpg|php|url|3A|http|dmzj|CB|D4|DA1|BB|E8|B9|B0_1411651600|C4|B0|C1|AE|D0|2FP017|2FP016|2FP014|2FP015|var|2FP020|2FP013|2FP021|2FP022|2FP019|2FP018|new|Array|2FP003|2FP004|2FP023|2FP007|2FP008|2FP011|||||||||||2FP010|2FP009|2FP012|2FP027|2FP030|26|25|2FP029|2FP028|2FP032|27|ED|93|F7|82|FB|23|24|21|2FP025|2FP026|20|2FP024|22'.split('|'),0,{})";
        //string js = @"'L h=S T();h[0]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%U.p\';h[1]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%V.p\';h[2]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%X.p\';h[3]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%Y.p\';h[4]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1b.p\';h[5]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1a.p\';h[6]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%Z.p\';h[7]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1c.p\';h[8]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%N.p\';h[9]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%J.p\';h[10]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%K.p\';h[11]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%I.p\';h[12]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%H.p\';h[13]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%R.p\';h[14]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%Q.p\';h[15]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%M.p\';h[16]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%O.p\';h[17]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%P.p\';h[18]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%W.p\';h[19]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1w.p\';h[1v]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1t.p\';h[1s]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1u.p\';h[1x]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1d.p\';h[1q]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1i.p\';h[1r]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1h.p\';h[1g]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1e.p\';h[1f]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%1j.p\';h[1k]=\'/o.q?r=t%s%b%n.u.m%i%b%v%j%d%f%e%e%f%l%a%g%k%d%c%z%a%C%E%a%D%F%A%g%c%w%b%a%x%y%B%b%G%c%G%1p%1o%1n%1l%1m.p\';',62,96,'||||||||||B5|2F|C2|D7|C3|D3|A5|ArrayPhoto|2Fs|AB|C7|EB|com|2Fimages|pic|jpg|php|url|3A|http|dmzj|CB|D4|DA1|BB|E8|B9|B0_1411651600|C4|B0|C1|AE|D0|2FP017|2FP016|2FP014|2FP015|var|2FP020|2FP013|2FP021|2FP022|2FP019|2FP018|new|Array|2FP003|2FP004|2FP023|2FP007|2FP008|2FP011|||||||||||2FP010|2FP009|2FP012|2FP027|2FP030|26|25|2FP029|2FP028|2FP032|27|ED|93|F7|82|FB|23|24|21|2FP025|2FP026|20|2FP024|22'.split('|')";
        //string js = @"( function Test(inputParm) {  return 'hello world ' + inputParm; } )";
        //object jsResult = ComicUtil.EvalJScript(js);

        //Microsoft.JScript.JSObject jsobj = jsResult as Microsoft.JScript.JSObject;


        ////Helpers.NLogger.Trace(jsResult.ToString());
        //foreach (string FieldName in jsobj)
        //{
        //    object FieldVal = jsobj[FieldName];
        //    Helpers.NLogger.Trace(FieldVal.ToString());
        //}

        //return new List<ComicPage>();
    }

    private string GetPageUrl(string pageUrl)
    {
        string url = String.Empty;

        if (false == pageUrl.Contains(".php"))
        {
            url = GetNativePicServer().TrimEnd('/') + "/" + pageUrl.Trim('\'').TrimStart('/');
        }
        else
        {
            string urlString = String.Empty;
            string queryString = String.Empty;

            pageUrl = pageUrl.Trim('\'').TrimStart('/');
            pageUrl = System.Web.HttpUtility.UrlDecode(pageUrl, System.Text.Encoding.GetEncoding("gb2312"));

            if (pageUrl.Contains("?url="))
            {
                urlString = pageUrl.Substring(0, pageUrl.IndexOf("?url=")) + "?url=";
                queryString = pageUrl.Substring(pageUrl.IndexOf("?url=") + 5);
            }
            else
            {
                urlString = String.Empty;
                queryString = pageUrl;
            }

            if (pageUrl.Contains("zhui.php"))
            {
                url = GetSonPicServer().TrimEnd('/') + "/" + urlString + System.Web.HttpUtility.UrlEncode(queryString, Encoding.GetEncoding("gb2312"));
            }
            else
            {
                url = GetWebimgServerURL().TrimEnd('/') + "/" + urlString + System.Web.HttpUtility.UrlEncode(queryString, Encoding.GetEncoding("gb2312"));
            }
        }
        return url;

        ////////            var server=getserver();
        ////////var serverurl=WebimgServerURL[server];
        ////////if(imageslist[page].indexOf('.php')==-1){
        ////////    var picurl =NativePicServer+imageslist[page+nextImgsNum];
        ////////}else{
        ////////    if(imageslist[page].indexOf('zhui.php')!=-1){
        ////////    var picurl =SonPicServer+imageslist[page+nextImgsNum];
        ////////    }else var picurl =serverurl+imageslist[page+nextImgsNum];
        ////////}
        ////////document.getElementById("nextimg"+nextImgsNum).src=picurl;
        ////////nextImgsNum++;
        //////////晊喧樓婥枑遣湔芞





        ////////if (match.ToString().Contains(".php"))
        ////////{
        ////////    page.Url = this._cRoot.PicHost.TrimEnd('/') + "/" + tmp.TrimStart('/');

        ////////    //Regex r = new Regex("")

        ////////    //string decodeUrl = System.Web.HttpUtility.UrlDecode(page.Url, System.Text.Encoding.GetEncoding("gb2312"));
        ////////    //page.Url = tmp.Replace(this._cRoot.PicHost + "pic.php?url=http%3A%2F%2Fimages.dmzj.com%2F", this._cRoot.PicHostAlternative);
        ////////    //page.Url = decodeUrl.Replace(this._cRoot.PicHost + "pic.php?url=http://images.dmzj.com/", this._cRoot.PicHostAlternative);


        ////////    //string tmp = match.ToString().Trim('\'').Replace("/pic.php?url=", "");
        ////////    //tmp = System.Web.HttpUtility.UrlDecode(tmp, System.Text.Encoding.GetEncoding("gb2312"));
        ////////    //tmp = System.Web.HttpUtility.UrlEncode(tmp, System.Text.Encoding.GetEncoding("gb2312"));
        ////////    //tmp = "pic.php?url=" + tmp;
        ////////    //page.Url = this._cRoot.PicHost + tmp;
        ////////}
        ////////else
        ////////{
        ////////    string tmpUrl = match.ToString().Trim('\'').Replace("/pic.php?url=http%3A%2F%2Fimages.dmzj.com%2F", "").Replace("+", " ");
        ////////    page.Url = new Uri(new Uri(this._cRoot.PicHost2), tmp).ToString();
        ////////}
    }
    #endregion
}
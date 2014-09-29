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
            //PicHost = @"http://mh.xindm.cn/", // Old
            IconHost = @"http://www.xindm.cn/",
            //PicHost = @"http://beiyong.bukamh.com/",
            PicHost = @"http://imgsxsq.bukamh.com/",
            PicHost2 = @"http://mh.xindm.cn/",


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
            for (int i = 0; i < 300; ++i)
            {
                ComicGroup cg = new ComicGroup();
                cg.GroupNumber = i + 1;
                cg.Caption = "第" + (i + 1).ToString().PadLeft(3, '0') + "頁";
                cg.Url = this._cRoot.WebHost + ("/index_" + (i + 1).ToString() + ".html").Replace("index_1.html", "index.html");
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
                if (Uri.IsWellFormedUriString(cn.Url, UriKind.Absolute) == false) cn.Url = (new Uri(new Uri(this._cRoot.WebHost), cn.Url)).ToString();
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
        public List<ComicChapter> GetComicChapters(ComicName cName)
        {
            //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Regex rVolumnList = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);
            //private readonly static Regex rLink = new Regex(@"<a (.|\n)*?>", RegexOptions.Compiled);
            Regex rUrl = new Regex(@"onclick=""window.open\('(.|\n)*?'", RegexOptions.Compiled);
            Regex rCleanTag = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
            Regex rCaption = new Regex(@"<span class=""(black|red)"">.+?</span>", RegexOptions.Compiled);

            string htmlContent = ComicUtil.GetContent(cName.Url);
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
                if (Uri.IsWellFormedUriString(cb.Url, UriKind.Absolute) == false) cb.Url = (new Uri(new Uri(this._cRoot.WebHost), cb.Url)).ToString();

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
        public List<ComicPage> GetComicPages(ComicChapter cChapter)
        {
            //Regex rPages = new Regex(@"var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);
            Regex rJS = new Regex(@"<script language=\""javascript\"">(.|\n)*?</script>", RegexOptions.Compiled);
            Regex rEval = new Regex(@"eval(.+)", RegexOptions.Compiled);
            //  var ArrayPhoto=new Array\(""(.|\n)+?;", RegexOptions.Compiled);
            Regex rPages = new Regex(@"\'.+?\'", RegexOptions.Compiled);


            string htmlContent = ComicUtil.GetContent(cChapter.Url);
            //string htmlContent = ComicUtil.GetContent(@"http://www.xindm.cn/mh/Soul%20Catcher/109903.html");
            string jsCode = rJS.Matches(htmlContent)[0].Groups[0].ToString();
            string evalCode = rEval.Matches(jsCode)[0].ToString();
            evalCode = evalCode.Substring(5, evalCode.Length - 6);
            string photoStr = ComicUtil.EvalJScript("var cs = " + evalCode).ToString();

            List<ComicPage> pages = new List<ComicPage>();
            foreach (Match match in rPages.Matches(photoStr))
            {
                int i = 1;
                ComicPage page = new ComicPage();
                string tmp = match.ToString().Trim('\'').Replace("/pic.php?url=http%3A%2F%2Fimages.dmzj.com%2F", "").Replace("+", " ");
                //tmp = System.Web.HttpUtility.UrlDecode(tmp, System.Text.Encoding.GetEncoding("gb2312"));
                //tmp = System.Web.HttpUtility.UrlEncode(tmp, System.Text.Encoding.GetEncoding("gb2312"));
                if (match.ToString().Contains("pic.php"))
                {
                    page.Url = new Uri(new Uri(this._cRoot.PicHost), tmp).ToString();
                }
                else
                {
                    page.Url = new Uri(new Uri(this._cRoot.PicHost2), tmp).ToString();
                }
                page.Caption = "第" + i.ToString().PadLeft(3, '0') + "頁";
                page.PageFileName = System.IO.Path.GetFileName(System.Web.HttpUtility.UrlDecode(page.Url, System.Text.Encoding.GetEncoding("gb2312")));
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
        #endregion
    }
}

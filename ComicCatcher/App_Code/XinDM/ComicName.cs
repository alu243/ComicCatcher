using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using System.IO;
using Utils;
namespace ComicModels
{
    public class ComicName : ComicBase, IComicName
    {

        private readonly static Regex rVolumnTag = new Regex(@"<ul>(.|\n)*?</ul>", RegexOptions.Compiled);
        //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private readonly static Regex rVolumnList = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);
        //private readonly static Regex rLink = new Regex(@"<a (.|\n)*?>", RegexOptions.Compiled);
        private readonly static Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
        private readonly static Regex rCleanTag = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
        private readonly static Regex rDesc = new Regex(@"<span class=""black"">.+?</span>", RegexOptions.Compiled);
        //Regex rDesc = new Regex

        public ComicName()
            : base()
        { }
        public ComicName(string url)
        {
            this.url = url;
        }

        public List<ComicChapter> getComicChapterList()
        {
            List<ComicChapter> result = new List<ComicChapter>();
            string sTemp = getHtmlTagContent(this.htmlContent);
            foreach (Match data in rVolumnList.Matches(sTemp))
            {
                //string sLink = rLink.Match(data.Value).Value;
                string sLink = data.Value;
                ComicChapter cb = new ComicChapter()
                {
                    url = rUrl.Match(sLink).Value.Replace("href=", "").Replace(@"""", "").Replace(@" /", @"/").Trim(),
                    //description = CharsetConverter.ToTraditional(rDesc.Match(sLink).Value.Replace(@"<span class=""black"">", "").Replace(@"</span>", "")
                    //.Replace(@"<fontcolor=red>", "").Replace(@"</font>", "").Replace(@"<b>","").Replace(@"</b>","").Trim())
                    Caption = CharsetConvertUtil.ToTraditional(rCleanTag.Replace(rDesc.Match(sLink).Value.Trim(), ""))
                };
                if (false == String.IsNullOrEmpty(cb.Caption))
                    result.Add(cb);
            }

            return result;
        }

        private string getHtmlTagContent(string htmlContent)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Match match in rVolumnTag.Matches(htmlContent))
            {
                sb.AppendLine(match.Value);
            }
            return sb.ToString();
        }
    }
}

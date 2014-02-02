using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using System.IO;
using Utils;
namespace ComicModels
{
    public class ComicName : ComicBase
    {

        private readonly static Regex rVolumnTag = new Regex(@"<ul>(.|\n)*?</ul>", RegexOptions.Compiled);
        //Regex rr = new Regex(@"^<table(.+|\n*)</table>$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private readonly static Regex rVolumnList = new Regex(@"<li>(.|\n)*?</li>", RegexOptions.Compiled);
        //private readonly static Regex rLink = new Regex(@"<a (.|\n)*?>", RegexOptions.Compiled);
        private readonly static Regex rUrl = new Regex(@"href=""(.|\n)*?""", RegexOptions.Compiled);
        private readonly static Regex rCleanTag = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
        private readonly static Regex rDesc = new Regex(@"<span class=""black"">.+?</span>", RegexOptions.Compiled);
        //Regex rDesc = new Regex

        public ComicName(string url)
        {
            this.url = url;

            this.htmlContent = HttpUtil.getResponse(url);
        }

        public List<ComicBase> getComicBaseList()
        {
            List<ComicBase> result = new List<ComicBase>();
            string sTemp = getURLTag();
            foreach (Match data in rVolumnList.Matches(sTemp))
            {
                //string sLink = rLink.Match(data.Value).Value;
                string sLink = data.Value;
                ComicBase cb = new ComicBase()
                {
                    url = rUrl.Match(sLink).Value.Replace("href=", "").Replace(@"""", "").Replace(@" /", @"/").Trim(),
                    //description = CharsetConverter.ToTraditional(rDesc.Match(sLink).Value.Replace(@"<span class=""black"">", "").Replace(@"</span>", "")
                    //.Replace(@"<fontcolor=red>", "").Replace(@"</font>", "").Replace(@"<b>","").Replace(@"</b>","").Trim())
                    description = CharsetConvertUtil.ToTraditional(rCleanTag.Replace(rDesc.Match(sLink).Value.Trim(), ""))

                };
                //foreach (char c in Path.GetInvalidFileNameChars()) cb.description = cb.description.Replace(c.ToString(), "");
                cb.description = cb.description.trimEscapeString();
                if (false == String.IsNullOrEmpty(cb.description))
                    result.Add(cb);
            }

            return result;
        }

        private string getURLTag()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Match match in rVolumnTag.Matches(this.htmlContent))
            {
                sb.AppendLine(match.Value);
            }
            return sb.ToString();
        }
    }
}

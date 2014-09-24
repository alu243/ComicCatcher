using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicModels
{
    public class XindmWebSite : IComicSiteSettings, IComicRoot, IComicPage, IComicName, IComicChapter
    {
        public static string Title { get { return @"新動漫(xindm)"; } }
        public static string WebSiteName { get { return @"xindm"; } }

        //public static string ListUrl { get { return @"http://www.xindm.cn/e/action/ListInfo.php"; } }
        public static string ListUrl { get { return @"http://www.xindm.cn/mh/index.html"; } }

        public static string WebUrl { get { return @"http://www.xindm.cn/mh"; } }

        public static string PicHost { get { return @"http://mh.xindm.cn/"; } }

        public static string PicHostAlternative { get { return @"http://mh2.xindm.cn/"; } }
    }
}

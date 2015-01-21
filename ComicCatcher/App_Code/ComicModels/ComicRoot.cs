using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using System.IO;
using Utils;

namespace ComicModels
{
    public class ComicRoot
    {
        public string WebSiteTitle { get; set; }
        public string WebSiteName { get; set; }
        public string WebHost { get; set; }
        public string IconHost { get; set; }
        public string PicHost { get; set; }
        public string PicHost2 { get; set; }
        public string PicHostAlternative { get; set; }
        //public bool BackgroundLoadIcon { get; set; }

        public int ThreadCount { get; set; }
    }
}

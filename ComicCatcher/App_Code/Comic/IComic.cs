using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicCatcher.App_Code.Comic
{
    interface IComic
    {
        /// <summary>
        /// URL
        /// </summary>
        string url { get; set; }

        string iconUrl { get; set; }

        /// <summary>
        /// 描述(第幾頁的內容或是第幾回)
        /// </summary>
        string description { get; set; }

        /// <summary>
        /// web網站的內容(就是url抓下來的內容)
        /// </summary>
        string content { get; set; }

   }
}

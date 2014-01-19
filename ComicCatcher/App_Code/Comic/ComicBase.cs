using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
namespace ComicCatcher.App_Code.Comic
{
    public class ComicBase : IDisposable
    {
        /// <summary>
        /// URL
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// web網站的內容(就是url抓下來的內容)
        /// </summary>
        public string htmlContent { get; set; }

        public string iconUrl { get; set; }

        /// <summary>
        /// 描述(第幾頁的內容或是第幾回)
        /// </summary>
        public string description { get; set; }


        /// <summary>
        /// 最近更新日期
        /// </summary>
        public string updateDate { get; set; }

        /// <summary>
        /// 最近更新回數
        /// </summary>
        public string updateChapter { get; set; }

        public MemoryStream imageData { get; set; }

        public void Dispose()
        {
            url = null;
            description = null;
            htmlContent = null;
        }
   }
}

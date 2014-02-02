using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Drawing;
using ComicCatcher.App_Code.Util;
using Utils;
using Utils;
namespace ComicModel
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
        private bool IsIconDataReaded = false;
        private MemoryStream iconData = null;
        public Image iconImage
        {
            get
            {
                try
                {
                    if (false == this.IsIconDataReaded)
                    {
                        this.iconData = HttpUtil.getPictureResponse(this.iconUrl);
                        this.IsIconDataReaded = true;
                    }
                    NLogger.Info("讀取icon圖檔中...," + this.description);
                    return Image.FromStream(this.iconData);
                }
                catch (Exception ex)
                {
                    NLogger.Error("無法讀取icon圖檔," + ex.ToString());
                    return null;
                }
            }
        }



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

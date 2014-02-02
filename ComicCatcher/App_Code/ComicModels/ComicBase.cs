using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Drawing;
using Helpers;
using Utils;
using Models;
using ComicModels;
namespace ComicModels
{
    public class ComicBase : IDisposable
    {
        /// <summary>
        /// URL
        /// </summary>
        public string url
        {
            get { return this._url; }
            set
            {
                this._url = value;
                try
                {
                    this.htmlContent = HttpUtil.getResponse(url);
                }
                catch (Exception ex)
                {
                    NLogger.Error("讀取Url內容時發生錯誤" + ex.ToString());
                }
            }
        }
        private string _url;

        /// <summary>
        /// web網站的內容(就是url抓下來的內容)
        /// </summary>
        public string htmlContent { get; set; }

        /// <summary>
        /// 描述(第幾頁的內容或是第幾回)
        /// </summary>
        public string Caption
        {
            get { return this._caption; }
            set { this._caption = value.trimEscapeString(); }
        }
        private string _caption;

        #region Icon
        public string iconUrl { get; set; }
        private bool IsIconDataReaded = false;
        private MemoryStream _iconData = null;
        public Image iconImage
        {
            get
            {
                try
                {
                    if (false == this.IsIconDataReaded || this._iconData == null)
                    {
                        this._iconData = HttpUtil.getPictureResponse(this.iconUrl);
                        this.IsIconDataReaded = true;
                    }
                    NLogger.Info("讀取icon圖檔中...," + this.Caption);
                    return Image.FromStream(this._iconData);
                }
                catch (Exception ex)
                {
                    NLogger.Error("無法讀取icon圖檔," + ex.ToString());
                    return null;
                }
            }
        }
        #endregion

        #region UpdateInfo
        /// <summary>
        /// 最近更新日期
        /// </summary>
        public string LastUpdateDate { get; set; }

        /// <summary>
        /// 最近更新回數
        /// </summary>
        public string LastUpdateChapter { get; set; }
        #endregion

        public void Dispose()
        {
            url = null;
            Caption = null;
            htmlContent = null;
        }
    }
}

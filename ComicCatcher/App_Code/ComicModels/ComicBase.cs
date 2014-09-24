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
using System.Threading;
namespace ComicModels
{
    public class ComicBase : IDisposable
    {

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

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
        public string IconUrl { get; set; }
        private bool IsIconDataReaded = false;
        private MemoryStream _iconData = null;
        public Image IconImage
        {
            get
            {
                if (false == this.IsIconDataReaded && this._iconData == null)
                {
                    //NLogger.Info("讀取icon圖檔中...," + this.Caption);
                    Thread t1 = new Thread(getPicture);
                    t1.IsBackground = true;
                    t1.Start();
                    return null;
                }
                else if (null == this._iconData)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        return Image.FromStream(this._iconData);
                    }
                    catch (Exception ex)
                    {
                        NLogger.Error("無法顯示icon圖檔," + ex.ToString());
                        return null;
                    }
                }
            }
        }
        #endregion

        private void getPicture()
        {
            try
            {
                this.IsIconDataReaded = true;
                this._iconData = HttpUtil.getPictureResponse(this.IconUrl);
            }
            catch (Exception ex)
            {
                NLogger.Error("無法讀取icon圖檔," + ex.ToString());
                NLogger.Error("漫畫名稱：" + this.Caption + "，圖檔路徑" + this.IconUrl);
            }

        }

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
            Url = null;
            Caption = null;
        }
    }
}

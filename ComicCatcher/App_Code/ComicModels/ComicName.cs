﻿using System;
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
    public class ComicNameInWebPage : ComicNodeBase
    {
        public string IconUrl { get; set; }

        private bool _isIconDataReaded = false;
        private Image _iconImage = null;
        public Image IconImage
        {
            get
            {
                if (false == this._isIconDataReaded && null == this._iconImage)
                {
                    this._isIconDataReaded = true;
                    Thread t1 = new Thread(GetPicture);
                    t1.IsBackground = true;
                    t1.Start();
                    return null;
                }
                else
                {
                    return this._iconImage;
                }
            }
        }

        private void GetPicture()
        {
            try
            {
                MemoryStream iconData = null;
                iconData = ComicUtil.GetPicture(this.IconUrl);
                this._iconImage = Image.FromStream(iconData);
            }
            catch (Exception ex)
            {
                NLogger.Info("讀取icon圖檔失敗," + this.Caption + "," + ex.ToString());
            }
        }

        public string LastUpdateDate { get; set; }
        public string LastUpdateChapter { get; set; }

    }
}

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
    public class ComicNodeBase
    {
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
    }
}

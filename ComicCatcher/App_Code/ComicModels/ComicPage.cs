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
    public class ComicPageInChapter : ComicNodeBase
    {
        public int PageNumber { get; set; }
        public string PageFileName { get; set; }

        public string Reffer { get; set; }
    }
}

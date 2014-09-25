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
    public class ComicPage : ComicNodeBase
    {
        public int PageNumber { get; set; }
    }
}

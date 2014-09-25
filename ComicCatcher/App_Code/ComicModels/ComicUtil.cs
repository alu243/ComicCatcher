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
    public class ComicUtil
    {
        public static MemoryStream GetPicture(string url)
        {
            return HttpUtil.getPictureResponse(url);
        }

        public static string GetContent(string url)
        {
            return HttpUtil.getResponse(url);
        }
    }
}

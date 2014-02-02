using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Text.RegularExpressions;
using Utils;
namespace ComicModels
{
    interface IComicChapter
    {
        List<string> genPictureUrl();
    }
}

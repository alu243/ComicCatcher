﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using System.IO;
using Utils;

namespace ComicModels
{
    interface IComicList
    {
        List<ComicName> getComicNameList();
    }
}

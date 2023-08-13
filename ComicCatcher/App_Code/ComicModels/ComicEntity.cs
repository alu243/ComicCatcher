using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using ComicCatcher.App_Code.ComicModels.Domains;
using Helpers;

namespace ComicCatcher.ComicModels;

public class ComicEntity : ComicBaseProperty
{
    public string IconUrl { get; set; }
    public Image IconImage { get; set; } = null;
    public string LastUpdateDate { get; set; }
    public string LastUpdateChapter { get; set; }
    public List<ComicChapter> Chapters { get; set; }
}
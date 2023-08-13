using System.Collections.Generic;
using System.Drawing;

namespace ComicCatcher.ComicModels;

public class ComicEntity : ComicBaseProperty
{
    public string IconUrl { get; set; }
    public Image IconImage { get; set; } = null;
    public string LastUpdateDate { get; set; }
    public string LastUpdateChapter { get; set; }
    public List<ComicChapter> Chapters { get; set; }
}
using System.Collections.Generic;
using System.Drawing;

namespace ComicCatcher.ComicModels;

public class ComicEntity : ComicBaseProperty
{
    public string IconUrl { get; set; }
    public Image IconImage { get; set; } = null;
    public string LastUpdateDate { get; set; }
    public string LastUpdateChapter { get; set; }
    public ComicState ListState { get; set; } = ComicState.Created;
    public ComicState ImageState { get; set; } = ComicState.Created;
    public List<ComicChapter> Chapters { get; set; } = new List<ComicChapter>();
}
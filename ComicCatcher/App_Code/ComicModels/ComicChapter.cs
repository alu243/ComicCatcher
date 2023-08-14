using System.Collections.Generic;

namespace ComicCatcher.ComicModels;

public class ComicChapter : ComicBaseProperty
{
    public List<ComicPage> Pages { get; set; } = new List<ComicPage>();
}
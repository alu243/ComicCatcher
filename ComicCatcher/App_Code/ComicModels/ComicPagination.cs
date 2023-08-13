using System.Collections.Generic;

namespace ComicCatcher.ComicModels;

public class ComicPagination : ComicBaseProperty
{
    public int TabNumber { get; set; }
    public List<ComicEntity> Comics { get; set; }
}
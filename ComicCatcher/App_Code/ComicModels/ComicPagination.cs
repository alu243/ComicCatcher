using System.Collections.Generic;

namespace ComicCatcher.ComicModels;

public class ComicPagination : ComicBaseProperty
{
    public int TabNumber { get; set; }
    public ComicState ListState { get; set; } = ComicState.Created;
    public List<ComicEntity> Comics { get; set; } = new List<ComicEntity>();
}
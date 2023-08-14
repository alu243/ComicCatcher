using System.Collections.Generic;
using System.Linq;

namespace ComicCatcher.ComicModels;

public class ComicRoot : ComicBaseProperty
{
    public string WebSiteName { get; set; }
    public string IconHost { get; set; }
    public string PicHost { get; set; }
    public string PicHost2 { get; set; }
    public string PicHostAlternative { get; set; }
    //public bool BackgroundLoadIcon { get; set; }
    public int ThreadCount { get; set; }
    public ComicState ListState { get; set; } = ComicState.Created;
    public List<ComicPagination> Paginations { get; set; }
    public List<ComicEntity> Comics => Paginations.SelectMany(p => p.Comics).ToList();
}
using System.Collections.Generic;

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
    public List<ComicPagination> Paginations { get; set; }
    public List<ComicEntity> Comics { get; set; }
}
using ComicCatcherLib.ComicModels;

namespace ComicApi.Model
{
    public class PageModel
    {
        public List<ComicPagination> Paginations { get; set; }
        public ComicPagination CurrPagination { get; set; }
    }

    public class ComicModel
    {
        public string Comic { get; set; }
        public ComicEntity CurrComic{ get; set; }
    }

    public class ChapterModel
    {
        public string Comic { get; set; }
        public string Chapter { get; set; }
        public string ComicName { get; set; }
        public ComicChapter CurrChapter{ get; set; }

    }
}

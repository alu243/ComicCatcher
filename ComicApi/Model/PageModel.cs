using ComicCatcherLib.ComicModels;

namespace ComicApi.Model
{
    public class PagesModel
    {
        public List<ComicPagination> Paginations { get; set; }
    }

    public class PageModel
    {
        public ComicPagination CurrPagination { get; set; }
        public List<ComicViewModel> Comics { get; set; }
    }

    public class ComicViewModel
    {
        public string Comic { get; set; }
        public string Url { get; set; }
        public string Caption { get; set; }
        public string IconUrl { get; set; }
        public string LastUpdateDate { get; set; }
        public string LastUpdateChapter { get; set; }
        public string ReadedChapter { get; set; }
        public string LastUpdateChapterLink { get; set; }
        public string ReadedChapterLink { get; set; }
        public bool IsIgnore { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class ComicModel
    {
        public int PageNumber { get; set; }
        public string Comic { get; set; }
        public ComicEntity CurrComic { get; set; }
        public string ReadedChapter { get; set; }
    }

    public class ChapterModel
    {

        public string Comic { get; set; }
        public string ComicName { get; set; }
        public string Chapter { get; set; }
        public string ChapterName { get; set; }
        public ComicChapter CurrChapter { get; set; }

    }
}

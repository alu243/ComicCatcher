using ComicCatcherLib.ComicModels;

namespace ComicApi.Model
{

    public class OldPageModel
    {
        public OldPageModel(
            List<ComicPagination> paginations,
            ComicPagination currPagination)
        {
            this.CurrPagination = currPagination;
            this.Paginations = paginations;
        }
        public List<ComicPagination> Paginations { get; set; }
        public ComicPagination CurrPagination { get; set; }
    }

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
        public string Url { get; set; }
        public string Caption { get; set; }
        public string IconUrl { get; set; }
        public string LastUpdateDate { get; set; }
        public string LastUpdateChapter { get; set; }
        public bool IsIgnore { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class ComicModel
    {
        public string Comic { get; set; }
        public ComicEntity CurrComic { get; set; }
    }

    public class ChapterModel
    {
        public string Comic { get; set; }
        public string Chapter { get; set; }
        public string ComicName { get; set; }
        public ComicChapter CurrChapter { get; set; }

    }
}

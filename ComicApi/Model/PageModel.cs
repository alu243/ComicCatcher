using ComicCatcherLib.ComicModels;

namespace ComicApi.Model
{
    public class PageModel
    {
        public PageModel(List<ComicPagination> paginations, ComicPagination currPagination, List<ComicEntity> filteredComics, Dictionary<string, string> ignoreComics)
        {
            this.CurrPagination = currPagination;
            this.Paginations = paginations;
            this.IgnoreComics = ignoreComics;
            this.FilteredComics = filteredComics;
        }
        public List<ComicPagination> Paginations { get; set; }
        public ComicPagination CurrPagination { get; set; }
        public List<ComicEntity> FilteredComics { get; set; }
        public Dictionary<string, string> IgnoreComics { get; set; }
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

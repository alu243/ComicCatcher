using ComicApi.Model.Requests;
using ComicCatcherLib.ComicModels;

namespace ComicApi.Model
{
    public class PageModel
    {
        public PageModel(
            List<ComicPagination> paginations,
            ComicPagination currPagination,
            List<ComicEntity> showComics,
            Dictionary<string, string> ignoreComics,
            Dictionary<string, string> favoriteComics)
        {
            this.CurrPagination = currPagination;
            this.Paginations = paginations;
            this.IgnoreComics = ignoreComics;
            this.ShowComics = showComics;
            this.FavoriteComics = favoriteComics;
        }
        public List<ComicPagination> Paginations { get; set; }
        public ComicPagination CurrPagination { get; set; }
        public List<ComicEntity> ShowComics { get; set; }
        public Dictionary<string, string> IgnoreComics { get; set; }
        public Dictionary<string, string> FavoriteComics { get; set; }
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

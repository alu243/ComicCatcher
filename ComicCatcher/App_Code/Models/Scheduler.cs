using ComicCatcher.App_Code.ComicModels;
using ComicCatcher.ComicModels;

namespace Models
{
    public class Tasker
    {
        public string name { get; set; }

        public ComicChapter downloadChapter { get; set; }
        //public List<string> downloadUrls { get; set; }
        public string downloadPath { get; set; }

        public IComicCatcher downloader { get; set; }
        //public List<string> downloadFileNames { get; set; }
    }
    public class DownloadPictureScheduler
    {
        public string name { get; set; }
        public string downloadUrl { get; set; }
        public string downloadPath { get; set; }
        public string downloadFileName { get; set; }
        public string reffer { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace Models
{
    public class Tasker
    {
        public string name { get; set; }

        public ComicModels.ComicChapterInName downloadChapter { get; set; }
        //public List<string> downloadUrls { get; set; }
        public string downloadPath { get; set; }

        public ComicModels.IComicCatcher downloader { get; set; }
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

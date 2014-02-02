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
        public string downloadUrl { get; set; }
        public bool usingAlternativeUrl { get; set; }
        public string downloadPath { get; set; }
    }
    public class DownloadPictureScheduler
    {
        public string name { get; set; }
        public string downloadUrl { get; set; }
        public string downloadPath { get; set; }
    }
}

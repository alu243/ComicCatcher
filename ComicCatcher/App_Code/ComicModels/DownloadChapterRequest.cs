using ComicCatcher.ComicModels.Domains;
using System;

namespace ComicCatcher.ComicModels;

public class DownloadChapterRequest
{
    public string Name { get; set; }
    public ComicChapter Chapter { get; set; }
    public string Path { get; set; }
    public Action<int, object> ReportProgressAction { get; set; }
}
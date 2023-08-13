using ComicCatcher.App_Code.ComicModels.Domains;
using ComicCatcher.ComicModels;
using System.IO;

namespace ComicCatcher.App_Code.ComicModels;

public class DownloadPageTask
{
    public DownloadPageTask(ComicPage page, string downloadPath)
    {
        Page = new ComicPage()
        {
            Url = page.Url,
            Caption = page.Caption,
            PageFileName = page.PageFileName,
            PageNumber = page.PageNumber,
            Refer = page.Refer,
        };
        DownloadPath = downloadPath;
    }

    public ComicPage Page { get; set; }
    public string DownloadPath { get; set; }
    public string GetFullPath() => Path.Combine(this.DownloadPath, this.Page.PageFileName);
}
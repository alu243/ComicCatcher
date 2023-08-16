using ComicApi.Model;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class ComicController : ControllerBase
    {
        private Dm5 dm5;
        private IHostingEnvironment env;
        public ComicController(Dm5 comic, IHostingEnvironment hostEnvironment)
        {
            dm5 = comic;
            env = hostEnvironment;
        }

        [HttpPost("{comic}/{chapter}")]
        public async Task<ResponseModel> AddComicChapter(string comic, string chapter)
        {
            //dm5.GetRoot().Paginations[0].Comics.Add(Dm5.CreateComic(comicUrl));

            var comicChapter = new ComicChapter()
            {
                Caption = "",
                Pages = new List<ComicPage>(),
                Url = new Uri(new Uri(new Uri(dm5.GetRoot().Url), comic), chapter).ToString()
            };
            await dm5.LoadPages(comicChapter);
            await dm5.DownloadChapter(new DownloadChapterRequest()
            {
                Path = Path.Combine(env.WebRootPath, Config.GetComicPath(), comic, chapter),
                Chapter = comicChapter,
                Name = "",
                ReportProgressAction = null,
            });

            //var chapterLocalPath = ApiChapterDao.SaveChapter(comic, chapter);
            return ResponseModel.Ok();
        }


        [HttpGet("{comic}")]
        public async Task<ContentResult> ShowComic(string comic)
        {
            var comicEntity = new ComicEntity()
            {
                Caption = "",
                ListState = ComicState.Created,
                ImageState = ComicState.Created,
                Url = new Uri(new Uri(dm5.GetRoot().Url), comic).ToString(),
                Chapters = new List<ComicChapter>(),
                IconImage = null,
                IconUrl = null,
                LastUpdateChapter = "",
                LastUpdateDate = "",
            };

            await dm5.LoadChapters(comicEntity);

            var sb = new StringBuilder();
            sb.AppendLine(@"<html><head><meta charset=""utf-8""></head><body>");
            sb.Append(@"<button onclick=""history.back()"">上一頁</button>");
            int i = 0;
            sb.Append("<table>");
            foreach (var chapter in comicEntity.Chapters)
            {
                var uri = new Uri(chapter.Url);
                if (i % 5 == 0) sb.Append("<tr>");
                sb.Append("<td>");
                sb.AppendLine($@"<p><a href=""/comic/{comic}/{uri.LocalPath.Trim('/')}"">{chapter.Caption}</a></p>");
                sb.Append("</td>");
                if (i % 5 == 4) sb.Append("</tr>");
                i++;
            }
            if (i % 5 != 0) sb.Append("</tr>");
            sb.AppendLine("</table>");
            sb.Append(@"<button onclick=""history.back()"">上一頁</button>");
            sb.AppendLine("</body></html>");
            //using (StreamWriter writer = new StreamWriter(viewPath))
            //{
            //    writer.WriteLine(sb.ToString());
            //}
            return new ContentResult
            {
                Content = sb.ToString(),
                ContentType = "text/html"
            };
        }

        [HttpGet("{comic}/{chapter}")]
        public async Task<ContentResult> ShowComicChapter(string comic, string chapter)
        {
            //var viewPath = Path.Combine(webHostEnvironment.WebRootPath, Config.GetComicPath(), comic, chapter, "view.html");
            string[] files;
            var comicChapter = new ComicChapter()
            {
                Caption = "",
                Pages = new List<ComicPage>(),
                Url = new Uri(new Uri(new Uri(dm5.GetRoot().Url), comic), chapter).ToString()
            };
            await dm5.LoadPages(comicChapter);


            var sb = new StringBuilder();
            sb.AppendLine(@"<html><head><meta charset=""utf-8""></head><body onload=""sc()"">");
            sb.Append(@"<button onclick=""history.back()"">上一頁</button>");
            foreach (var page in comicChapter.Pages)
            {
                //style=""min-wdith:70%;width:95%;""
                sb.AppendLine($@"<div><img src=""{page.Url}"" style=""width:98%;"" alt=""Snow""></div>");
            }

            sb.Append(@"<button onclick=""history.back()"">上一頁</button>");
            sb.AppendLine("<script>function sc(){window.scrollTo({ top: 0, left: 0, behavior: 'smooth' });}</script></body></html>");
            //using (StreamWriter writer = new StreamWriter(viewPath))
            //{
            //    writer.WriteLine(sb.ToString());
            //}
            return new ContentResult
            {
                Content = sb.ToString(),
                ContentType = "text/html"
            };

            //////////var viewPath = Path.Combine(webHostEnvironment.WebRootPath, Config.GetComicPath(), comic, chapter, "view.html");
            ////////string[] files;
            ////////var directory = Path.Combine(env.ContentRootPath, Config.GetComicPath(), comic, chapter);
            ////////if (Directory.Exists(directory))
            ////////{
            ////////    files = Directory.GetFiles(directory);
            ////////}
            ////////else
            ////////{
            ////////    await AddComicChapter(comic,  chapter);
            ////////    files = new string[] { };
            ////////}

            ////////var sb = new StringBuilder();
            ////////sb.AppendLine(@"<html><header></header><body onload=""sc()"">");
            ////////foreach (var file in files)
            ////////{
            ////////    //style=""min-wdith:70%;width:95%;""
            ////////    sb.AppendLine($@"<div><img src=""./{file}"" alt=""Snow""></div>");
            ////////}

            ////////sb.AppendLine("<script>function sc(){window.scrollTo({ top: 0, left: 0, behavior: 'smooth' });}</script></body></html>");
            //////////using (StreamWriter writer = new StreamWriter(viewPath))
            //////////{
            //////////    writer.WriteLine(sb.ToString());
            //////////}
            ////////return new ContentResult
            ////////{
            ////////    Content = sb.ToString(),
            ////////    ContentType = "text/html"
            ////////};
        }

        [HttpGet("{comic}/{chapter}/{pageFile}")]
        public FileResult GetComicPage(string comic, string chapter, string pageFile)
        {
            var path = Path.Combine(env.WebRootPath,
                Config.GetComicPath(),
                comic,
                chapter,
                pageFile);
            return new FileStreamResult(new FileStream(path, FileMode.Open), "image/jpeg");
        }
    }
}

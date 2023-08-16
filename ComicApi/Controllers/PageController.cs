using ComicApi.Model;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.ComicModels.Domains;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class PageController : ControllerBase
    {
        private Dm5 dm5;
        private IHostingEnvironment env;
        public PageController(Dm5 comic, IHostingEnvironment hostEnvironment)
        {
            dm5 = comic;
            env = hostEnvironment;
        }


        [HttpGet("{page}")]
        public async Task<ContentResult> ShowComics(int page = 1)
        {
            var pagination = dm5.GetRoot().Paginations[page - 1];

            await dm5.LoadComicsForWeb(pagination);

            var sb = new StringBuilder();
            sb.AppendLine(@"<html><head><meta charset=""utf-8""></head><body>");
            foreach (var pg in dm5.GetRoot().Paginations.Take(30))
            {
                var uri = new Uri(pagination.Url);
                sb.AppendLine($@"<a href=""/page/{page}"">{pg.Caption}</a>&nbsp;&nbsp;");
            }

            var pageCount = 5;
            int i = 0;
            sb.AppendLine("<table>");
            foreach (var comic in pagination.Comics)
            {
                if (i % pageCount == 0) sb.AppendLine("<tr>");
                var uri = new Uri(comic.Url);
                sb.Append("<td>");
                sb.AppendLine($@"<p><a href=""/comic/{uri.LocalPath.Trim('/')}""><img src=""{comic.IconUrl}""><br>{comic.Caption}</a></p>");
                sb.Append("</td>");
                if (i % pageCount == pageCount - 1) sb.AppendLine("</tr>");
                i++;
            }
            if (i % pageCount != 0) sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

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
        }
    }
}

using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class ComicController : Controller
    {
        private ComicApplication app;
        private readonly IHttpClientFactory _httpClientFactory;
        private static HttpClient client = null;

        public ComicController(ComicApplication comicApplication, IHttpClientFactory httpClientFactory)
        {
            app = comicApplication;
            _httpClientFactory = httpClientFactory;
            if (client == null)
            {
                var handler = new SocketsHttpHandler()
                {
                    UseCookies = true, 
                    Proxy = null, 
                    UseProxy = false, 
                    MaxConnectionsPerServer = 20, // 設置每個服務器的最大連接數
                    PooledConnectionLifetime = TimeSpan.FromMinutes(2), // 連接在池中的最大存活時間
                    PooledConnectionIdleTimeout = TimeSpan.FromSeconds(90) // 空閒連接的超時時間
                };
                //var handler = new HttpClientHandler() { UseCookies = true, UseProxy = false, Proxy = null };
                client = new HttpClient(handler);// { BaseAddress = baseAddress };
            }
        }

        [HttpGet("{comic}")]
        public async Task<IActionResult> ShowChaptersInComic(string comic)
        {
            return View("ShowChaptersInComic");
        }

        [HttpGet("{comic}/{chapter}")]
        public async Task<IActionResult> ShowPagesInChapter(string comic, string chapter)
        {
            var comicEnity = await app.GetComic(comic);
            return View("ShowPagesInChapter", comicEnity);
        }


        [HttpGet("{comic}/{chapter}/{page:int}")]
        public async Task<IActionResult> GetImage(string comic, string chapter, int page)
        {
            try
            {
                var comicChapter = await app.GetComicChapterWithPage(comic, chapter);
                var comicPage = comicChapter.Pages[page - 1];
                var url = comicPage.Url;
                var referer = $"https://www.dm5.cn/{chapter}/";
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                //requestMessage.Headers.Add("Referer", $"https://www.dm5.cn/{chapter}/");
                requestMessage.Headers.Referrer = new Uri(referer);
                //requestMessage.Headers.CacheControl = new CacheControlHeaderValue() { NoCache = true };

                //using var client = new HttpClient();
                var response = await client.SendAsync(requestMessage);
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine(response.RequestMessage.Headers.ToString());
                    return StatusCode(431);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode);
                }
                var content = await response.Content.ReadAsByteArrayAsync();
                return File(content, "image/jpeg");
            }
            catch (Exception e)
            {
                return StatusCode((int)404);
            }
        }
    }
}


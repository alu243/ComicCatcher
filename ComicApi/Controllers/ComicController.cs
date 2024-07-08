using Microsoft.AspNetCore.Mvc;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class ComicController : Controller
    {
        private ComicApplication app;
        private static HttpClient client = null;

        public ComicController(ComicApplication comicApplication)
        {
            app = comicApplication;
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


        [HttpGet("{comic}/{chapter}/{url}")] //[HttpGet("{comic}/{chapter}/{page:int}")]
        public async Task<IActionResult> GetImage(string comic, string chapter, string url)
        {
            try
            {
                //var comicChapter = await app.GetComicChapterWithPage(comic, chapter);
                //var comicPage = comicChapter.Pages[page - 1];
                //var url = comicPage.Url;
                url = Uri.UnescapeDataString(url);
                var referer = $"https://www.dm5.cn/" + chapter + "/";
                url = "https://manhua1038zjcdn123.cdndm5.com/82/81521/1542619/1_4319.jpg?cid=1542619&key=18e3d587dc68f7552e5945dae2a0f570";

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                //requestMessage.Headers.Add("Referer", $"https://www.dm5.cn/{chapter}/");
                requestMessage.Headers.Referrer = new Uri(referer);
                //requestMessage.Headers.CacheControl = new CacheControlHeaderValue() { NoCache = true };

                //using var client = new HttpClient();
                var response = await client.SendAsync(requestMessage);
                Console.WriteLine(url);
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    var rt = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(rt);
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


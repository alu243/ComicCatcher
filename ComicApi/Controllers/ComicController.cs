using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace ComicApi.Controllers
{
    [Route("[controller]")]
    public class ComicController : Controller
    {
        private ComicApplication app;
        private readonly IHttpClientFactory _httpClientFactory;

        public ComicController(ComicApplication comicApplication, IHttpClientFactory httpClientFactory)
        {
            app = comicApplication;
            _httpClientFactory = httpClientFactory;
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


        [HttpGet("{comic}/{chapter}/{url}")]
        public async Task<IActionResult> GetImage(string comic, string chapter, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL is required");
            }

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, Uri.UnescapeDataString(url));
                requestMessage.Headers.Add("Referrer-Policy", "unsafe-url");
                //requestMessage.Headers.Add("Referrer", $"https://www.dm5.cn/{chapter}");
                //requestMessage.Headers.Add("Referrer", $"https://www.dm5.cn/{chapter}/");
                //requestMessage.Headers.Add("Referer", $"https://www.dm5.cn/{chapter}/");
                requestMessage.Headers.Referrer = new Uri($"https://www.dm5.cn/{chapter}/");
                //requestMessage.Headers.Add("Referrer", $"https://www.dm5.cn/{chapter}/");
                var client = _httpClientFactory.CreateClient("proxy");
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
                return File(content, "image/jpeg"); // 你可以根据图片的 MIME 类型修改这里
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}


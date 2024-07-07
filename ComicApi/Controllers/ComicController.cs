using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;

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
                url = Uri.UnescapeDataString(url);
                var referer = $"https://www.dm5.cn/{chapter}/";
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                //requestMessage.Headers.Add("Referrer", $"https://www.dm5.cn/{chapter}");
                //requestMessage.Headers.Add("Referrer", $"https://www.dm5.cn/{chapter}/");
                //requestMessage.Headers.Add("Referer", $"https://www.dm5.cn/{chapter}/");
                requestMessage.Headers.Referrer = new Uri(referer);
                requestMessage.Headers.CacheControl = new CacheControlHeaderValue() { NoCache = true };

                //requestMessage.Headers.Add("Referrer", $"https://www.dm5.cn/{chapter}/");
                using var client = new HttpClient();
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


                //using (var client = new WebClient())
                //{
                //    client.Headers.Add("Referer", $"https://www.dm5.cn/{chapter}/");
                //    try
                //    {
                //        var content = await client.DownloadDataTaskAsync(Uri.UnescapeDataString(url));
                //        var contentType = client.ResponseHeaders["Content-Type"] ?? "application/octet-stream";
                //        return File(content, contentType); 
                //    }
                //    catch (WebException ex) when (ex.Response is HttpWebResponse response)
                //    {
                //        return StatusCode((int)response.StatusCode);
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}


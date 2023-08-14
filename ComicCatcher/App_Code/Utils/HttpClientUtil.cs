using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Jint.Parser.Ast;

namespace ComicCatcher.Utils
{
    public static class HttpClientUtil
    {
        private static HttpClient _client = null;

        private static HttpClient client
        {
            get
            {
                if (_client == null)
                {
                    var handler = new HttpClientHandler() { UseCookies = true, Proxy = null};
                    _client = new HttpClient(handler);// { BaseAddress = baseAddress };
                }
                return _client;
            }
        }

        public static void SetConnections(int conections)
        {
            var handler = new HttpClientHandler() { UseCookies = true, Proxy = null, MaxConnectionsPerServer = conections};
            _client = new HttpClient(handler);// { BaseAddress = baseAddress };
        }

        public static async Task<string> GetStringResponse(string url, string referer)
        {
            if (string.IsNullOrEmpty(referer))
            {
                referer = url?.GetRefererString();
            }

            using (var message = new HttpRequestMessage(HttpMethod.Get, url))
            {
                message.Headers.Add("Cookie", "7940D296A3BE781=19ED29CEE96C1DF90080A93377D9D1C2DD7C37D201C4DB2480F7563CA47DA90609093C32E4AA1DBF130BBB619C132215F421A972DF02C9B43B796E8BD8E7CFFB4D927F42CFA69E9D046960FB2579CC3A8FA07609E4D8BE06E95378BB93A68F4A9F17160B4BDA5818CA956743FFDC9E0FAF0E6B87787FA32EADEC7C44E437C19F8C24F9DDA59F820CE2B69D446192A567A6330D7C941E75015361F4B670A112A70F270973D718A740CBCF85A28D9E7307E8FAB8536B75DF374252756C957870D39B56C1E7B10BD8D427378BE058614E05C555F5CCDFB134DE37CC9321E56F4115140DDBA9CC171000FBBC61250429FD8C;isAdult=1");
                message.Headers.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
                message.Headers.Add("Referer", referer);
                //message.Headers.Add("Cookie", "SERVERID=node2");
                //message.Headers.Add("Cookie", "DM5_MACHINEKEY=ad2e5ffb-08ac-4746-b023-1b9a7960ac78");
                //message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36");
                var result = await client.SendAsync(message);
                result.EnsureSuccessStatusCode();
                var res = await result.Content.ReadAsStringAsync();
                //if (url == "https://www.dm5.com/manhua-yidianxingyuan/")
                //{
                //    var a = "";
                //}
                return res;
            }
        }

        public static async Task<Stream> GetStreamResponse(string url, string referer)
        {
            if (string.IsNullOrEmpty(referer))
            {
                referer = url.GetRefererString();
            }

            using (var message = GetRequest(HttpMethod.Get, url, referer))
            {
                var result = await client.SendAsync(message);
                result.EnsureSuccessStatusCode();
                var res = await result.Content.ReadAsStreamAsync();
                return res;
            }
        }

        private static HttpRequestMessage GetRequest(HttpMethod method, string url, string referer)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", "7940D296A3BE781=19ED29CEE96C1DF90080A93377D9D1C2DD7C37D201C4DB2480F7563CA47DA90609093C32E4AA1DBF130BBB619C132215F421A972DF02C9B43B796E8BD8E7CFFB4D927F42CFA69E9D046960FB2579CC3A8FA07609E4D8BE06E95378BB93A68F4A9F17160B4BDA5818CA956743FFDC9E0FAF0E6B87787FA32EADEC7C44E437C19F8C24F9DDA59F820CE2B69D446192A567A6330D7C941E75015361F4B670A112A70F270973D718A740CBCF85A28D9E7307E8FAB8536B75DF374252756C957870D39B56C1E7B10BD8D427378BE058614E05C555F5CCDFB134DE37CC9321E56F4115140DDBA9CC171000FBBC61250429FD8C;isAdult=1");
            request.Headers.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.Add("Referer", referer);
            //message.Headers.Add("Cookie", "SERVERID=node2");
            //message.Headers.Add("Cookie", "DM5_MACHINEKEY=ad2e5ffb-08ac-4746-b023-1b9a7960ac78");
            //message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36");
            return request;
        }
    }
}

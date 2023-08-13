using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utils
{
    public static class HttpUtil
    {
        private static HttpClient _client = null;

        private static HttpClient client
        {
            get
            {
                if (_client == null)
                {
                    var handler = new HttpClientHandler() { UseCookies = true, Proxy = null };
                    _client = new HttpClient(handler);// { BaseAddress = baseAddress };
                }
                return _client;
            }
        }

        private static async Task<string> GetStringResponse(string url, string referer)
        {
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



        private static CookieContainer _myCookie = null;
        private static CookieContainer myCookie
        {
            get
            {
                if (null == _myCookie)
                {
                    _myCookie = new CookieContainer();
                    _myCookie.Add(GetDm5Cookie("isAdult", "1", "www.dm5.com"));
                    _myCookie.Add(GetDm5Cookie("isAdult", "1", "dm5.com"));
                    _myCookie.Add(GetDm5Cookie("DM5_MACHINEKEY", "ad2e5ffb-08ac-4746-b023-1b9a7960ac78", "www.dm5.com"));
                    _myCookie.Add(GetDm5Cookie("DM5_MACHINEKEY", "ad2e5ffb-08ac-4746-b023-1b9a7960ac78", "dm5.com"));
                    _myCookie.Add(GetDm5Cookie("dm5cookieenabletest", "1", "www.dm5.com"));
                    _myCookie.Add(GetDm5Cookie("dm5cookieenabletest", "1", "dm5.com"));
                }
                return _myCookie;
            }
        }

        private static CookieContainer ChangeSession()
        {
            CookieContainer newCookie = new CookieContainer();
            newCookie.Add(GetDm5Cookie("isAdult", "1", "www.dm5.com"));
            newCookie.Add(GetDm5Cookie("isAdult", "1", "dm5.com"));
            newCookie.Add(GetDm5Cookie("DM5_MACHINEKEY", "ad2e5ffb-08ac-4746-b023-1b9a7960ac78", "www.dm5.com"));
            newCookie.Add(GetDm5Cookie("DM5_MACHINEKEY", "ad2e5ffb-08ac-4746-b023-1b9a7960ac78", "dm5.com"));
            newCookie.Add(GetDm5Cookie("dm5cookieenabletest", "1", "www.dm5.com"));
            newCookie.Add(GetDm5Cookie("dm5cookieenabletest", "1", "dm5.com"));
            return newCookie;
        }
        private static Cookie GetDm5Cookie(string name, string value, string domain)
        {
            Cookie dm5Cookie = new Cookie(name, value);
            //Cookie dm5Cookie = new Cookie("dm5imgcookie", "461585%7C5");

            dm5Cookie.Domain = domain;
            dm5Cookie.Path = "/";
            dm5Cookie.Expires = DateTime.Now.AddDays(365);
            return dm5Cookie;
        }

        public static string getResponse(string url)
        {
            Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
            return getResponseString(url, encode, "");
        }

        public static string getUtf8Response(string url, string referer)
        {
            if (string.IsNullOrEmpty(referer))
            {
                referer = url.GetRefererString();
            }
            var utf8 = GetStringResponse(url, referer).Result;
            return utf8;
            //Encoding encode = System.Text.Encoding.UTF8;
            //return getResponseString(url, encode, reffer);
        }

        private static string getResponseString(string url, Encoding encoding, string reffer)
        {
            HttpWebRequest request = CreateRequest(url, reffer);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            try
            {
                var stream = response.GetResponseStream();
                var readStream = new StreamReader(stream, encoding);
                try
                {
                    StringBuilder sb = new StringBuilder();
                    Char[] read = new Char[2048];
                    int count;
                    while (0 < (count = readStream.Read(read, 0, 2048)))
                    {
                        sb.Append(new String(read, 0, count));
                    }
                    return sb.ToString();
                }
                finally
                {
                    stream.Dispose();
                    readStream.Dispose();
                }
            }
            finally
            {
                if (null != response) response.Close();
                if (null != request) request.Abort();
                request = null;
                response.Dispose();
            }
        }


        public static MemoryStream GetFileResponse(string url, string reffer)
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            HttpWebRequest request = CreateRequest(url, reffer);
            request.AllowAutoRedirect = false;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399)
            {
                string redirectUrl = response.Headers["Location"];
                redirectUrl = Encoding.GetEncoding("GB2312").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(redirectUrl));
                //Encoding.GetEncoding("GB2312").GetString(Encoding.GetEncoding(1252).GetBytes(redirectUrl));
                //string urlString = redirectUrl.Substring(0, redirectUrl.IndexOf("?"));
                //string queryString = redirectUrl.Substring(redirectUrl.IndexOf("?") + 1);
                //redirectUrl = urlString + "?" + System.Web.HttpUtility.UrlEncode(queryString, Encoding.GetEncoding("gb2312"));
                response.Dispose();
                request = null;
                GC.Collect();
                request = CreateRequest(redirectUrl, reffer);
                response = (HttpWebResponse)request.GetResponse();
            }

            try
            {
                int size;
                int.TryParse(response.Headers["Content-Length"], out size);
                using (Stream receiveStream = response.GetResponseStream())
                {
                    MemoryStream ms = new MemoryStream();

                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    while (0 < (bytesRead = receiveStream.Read(buffer, 0, buffer.Length)))
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }

                    //sw.Stop();
                    //NLogger.Info(string.Format("{0}讀取完成(Thread ID=({1})({2}ms)", fileName, Thread.CurrentThread.GetHashCode(), sw.ElapsedMilliseconds));

                    ms.Position = 0;
                    if (size > 0 && ms.Length != size)
                    {
                        throw new WebException(String.Format("下載的檔案({0})大小不合，應為{1}，實際{2}", url, size, ms.Length));
                    }
                    else
                    {
                        return ms;
                    }
                }
            }
            finally
            {
                if (null != response) response.Close();
                response = null;
                request = null;
            }
        }

        private static HttpWebRequest CreateRequest(string url, string reffer = "")
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 200;
            //System.Net.ServicePointManager.DefaultNonPersistentConnectionLimit = 200;
            HttpWebRequest request = null;
            //if (url.Split('?').Length > 1)
            //{
            //    //url = System.Web.HttpUtility.UrlDecode(url, Encoding.GetEncoding("GB2312"));
            //    //url = urls[0] + "?" + System.Web.HttpUtility.UrlEncode(urls[1], Encoding.GetEncoding("GB2312")).Replace("url%3d", "url=");
            //    string[] urls = url.Split('?');
            //    url = urls[0] + "?" + Uri.EscapeUriString(urls[1]).Replace("url%3d", "url=");
            //}
            //Uri uri = HackedUri.Create(url);
            Uri uri = new Uri(url);

            request = (HttpWebRequest)WebRequest.Create(uri);
            //request.ServicePoint.ConnectionLimit = 512;
            //request.ServicePoint.Expect100Continue = false;
            //request.ServicePoint.UseNagleAlgorithm = false;
            //request.AllowWriteStreamBuffering = false;


            //request.AllowAutoRedirect = false; // 只有在 getPicture 時才會
            if (String.IsNullOrEmpty(reffer))
            {
                request.Referer = url.GetRefererString();
            }
            else
            {
                request.Referer = reffer;
            }
            request.CookieContainer = myCookie; // 拿到上次成功連線的 cookie 當作是同一個 session 
            //request.CookieContainer.Add(new Uri("http://www.dm5.com"), new Cookie("isAdult", "1"));

            request.Headers["Cookie"] = "isAdult=1";

            request.Timeout = 5000;
            request.ReadWriteTimeout = 30000;
            //request.ContentType = "image/jpeg";
            //request.Referer = url;
            request.Proxy = null;//ProxySetting.getProxy();
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-TW,zh;q=0.8,en-US;q=0.6,en;q=0.4,zh-CN;q=0.2");
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "GB2312");
            //request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded;charset=gb2312;";
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.KeepAlive = false;
            return request;
        }
    }

    ////public static class HackedUri
    ////{
    ////    private const GenericUriParserOptions c_Options =
    ////        GenericUriParserOptions.Default |
    ////        GenericUriParserOptions.DontUnescapePathDotsAndSlashes |
    ////        GenericUriParserOptions.Idn |
    ////        GenericUriParserOptions.IriParsing;
    ////    private static readonly GenericUriParser s_SyntaxHttp = new GenericUriParser(c_Options);
    ////    private static readonly GenericUriParser s_SyntaxHttps = new GenericUriParser(c_Options);

    ////    static HackedUri()
    ////    {
    ////        // Initialize the scheme 
    ////        FieldInfo fieldInfoSchemeName = typeof(UriParser).GetField("m_Scheme", BindingFlags.Instance | BindingFlags.NonPublic);
    ////        if (fieldInfoSchemeName == null)
    ////        {
    ////            throw new MissingFieldException("'m_Scheme' field not found");
    ////        }
    ////        fieldInfoSchemeName.SetValue(s_SyntaxHttp, "http");
    ////        fieldInfoSchemeName.SetValue(s_SyntaxHttps, "https");

    ////        FieldInfo fieldInfoPort = typeof(UriParser).GetField("m_Port", BindingFlags.Instance | BindingFlags.NonPublic);
    ////        if (fieldInfoPort == null)
    ////        {
    ////            throw new MissingFieldException("'m_Port' field not found");
    ////        }
    ////        fieldInfoPort.SetValue(s_SyntaxHttp, 80);
    ////        fieldInfoPort.SetValue(s_SyntaxHttps, 443);
    ////    }

    ////    public static Uri Create(string url)
    ////    {
    ////        Uri result = new Uri(url);

    ////        if (url.IndexOf("%2F", StringComparison.OrdinalIgnoreCase) != -1)
    ////        {
    ////            UriParser parser = null;
    ////            switch (result.Scheme.ToLowerInvariant())
    ////            {
    ////                case "http":
    ////                    parser = s_SyntaxHttp;
    ////                    break;
    ////                case "https":
    ////                    parser = s_SyntaxHttps;
    ////                    break;
    ////            }

    ////            if (parser != null)
    ////            {
    ////                // Associate the parser 
    ////                FieldInfo fieldInfo = typeof(Uri).GetField("m_Syntax", BindingFlags.Instance | BindingFlags.NonPublic);
    ////                if (fieldInfo == null)
    ////                {
    ////                    throw new MissingFieldException("'m_Syntax' field not found");
    ////                }
    ////                fieldInfo.SetValue(result, parser);
    ////            }
    ////        }
    ////        return result;
    ////    }
    ////}

}

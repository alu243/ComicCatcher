using System;
using System.IO;
using System.Net;
using System.Text;

namespace ComicCatcher.Utils
{
    public static class HttpUtil
    {

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

        public static string GetResponse(string url)
        {
            Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
            return GetResponseString(url, encode, "");
        }

        public static string GetUtf8Response(string url, string referer)
        {
            if (string.IsNullOrEmpty(referer))
            {
                referer = url.GetRefererString();
            }
            Encoding encode = Encoding.UTF8;
            return GetResponseString(url, encode, referer);
        }

        private static string GetResponseString(string url, Encoding encoding, string reffer)
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

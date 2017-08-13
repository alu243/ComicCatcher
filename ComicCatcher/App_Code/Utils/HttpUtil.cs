using System;
using System.Text;

using System.Net;
using System.IO;

using Helpers;
using System.Reflection;
using System.Threading;
using System.Web;
namespace Utils
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

        public static string getResponse(string url, int remainTries = 50)
        {
            HttpWebRequest request = CreateRequest(url);
            HttpWebResponse response = CreateResponse(request, url, remainTries);
            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                StreamReader readStream = new StreamReader(response.GetResponseStream(), encode);

                StringBuilder sb = new StringBuilder();
                Char[] read = new Char[2048];
                // Reads 256 characters at a time.    
                int count;
                while (0 < (count = readStream.Read(read, 0, 2048)))
                {
                    // Dumps the 256 characters on a string and displays the string to the console.
                    sb.Append(new String(read, 0, count));
                }
                return sb.ToString();
                //return readStream.ReadToEnd();
                //}
            }
            finally
            {
                if (null != response) response.Close();
                if (null != request) request.Abort();
                request = null;
            }
        }

        public static string getUtf8Response(string url, string reffer, int remainTries = 50)
        {
            HttpWebRequest request = CreateRequest(url, reffer);
            HttpWebResponse response = CreateResponse(request, url, remainTries);
            try
            {
                if (myCookie.GetCookies(new Uri("http://www.dm5.com")).Count <= 3)
                {
                    foreach (Cookie cookie in response.Cookies)
                    {
                        myCookie.Add(cookie);
                    }
                }


                Encoding encode = System.Text.Encoding.UTF8;
                StreamReader readStream = new StreamReader(response.GetResponseStream(), encode);

                StringBuilder sb = new StringBuilder();
                Char[] read = new Char[2048];
                // Reads 256 characters at a time.    
                int count;
                while (0 < (count = readStream.Read(read, 0, 2048)))
                {
                    // Dumps the 256 characters on a string and displays the string to the console.
                    sb.Append(new String(read, 0, count));
                }
                return sb.ToString();
                //return readStream.ReadToEnd();
                //}

                // Add Cookies

            }
            finally
            {
                if (null != response) response.Close();
                if (null != request) request.Abort();
                request = null;
            }
        }

        public static MemoryStream getPictureResponse(string url, string reffer = "", int remainTries = 50)
        {
            //url = System.Web.HttpUtility.UrlDecode(url, Encoding.GetEncoding("GB2312"));
            //url = url.Split('?')[0] + "?" + System.Web.HttpUtility.UrlEncode(url.Split('?')[1], Encoding.GetEncoding("GB2312"));
            //url = url.Split('?')[0] + "?" + System.Web.HttpUtility.UrlEncode(url.Split('?')[1], Encoding.UTF8);
            //url = url.Split('?')[0] + "?" + System.Web.HttpUtility.UrlEncode(url.Split('?')[1], Encoding.UTF7);
            //url = url.Split('?')[0] + "?" + System.Web.HttpUtility.UrlPathEncode(url.Split('?')[1]);

            //url = System.Web.HttpUtility.UrlDecode(url, Encoding.GetEncoding(950));
            //url = url.Split('?')[0] + "?" + System.Web.HttpUtility.UrlEncode(url.Split('?')[1], Encoding.UTF8);


            //url = url.Split('?')[0] + "?" + Uri.EscapeUriString(url.Split('?')[1]);
            //url = url.Replace("url%3d", "url=");
            //url = url.Replace("http://beiyong.bukamh.com/pic.php?url=http%3A%2F%2Fimages.dmzj.com%2F", "http://imgsxsq.bukamh.com/");
            HttpWebRequest request = CreateRequest(url, reffer);
            request.AllowAutoRedirect = false;
            HttpWebResponse response = CreateResponse(request, url, remainTries);
            if ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399)
            {
                string redirectUrl = response.Headers["Location"];
                redirectUrl = Encoding.GetEncoding("GB2312").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(redirectUrl));
                //Encoding.GetEncoding("GB2312").GetString(Encoding.GetEncoding(1252).GetBytes(redirectUrl));

                //string urlString = redirectUrl.Substring(0, redirectUrl.IndexOf("?"));
                //string queryString = redirectUrl.Substring(redirectUrl.IndexOf("?") + 1);

                //redirectUrl = urlString + "?" + System.Web.HttpUtility.UrlEncode(queryString, Encoding.GetEncoding("gb2312"));
                response.Close();
                request.Abort();
                request = null;
                GC.Collect();
                request = CreateRequest(redirectUrl);
                response = CreateResponse(request, redirectUrl, remainTries);
            }

            try
            {
                using (Stream receiveStream = response.GetResponseStream())
                {
                    MemoryStream ms = new MemoryStream();
                    //ms.Write(br.ReadBytes(Convert.ToInt32(response.ContentLength)), 0, Convert.ToInt32(response.ContentLength));
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while (0 < (bytesRead = receiveStream.Read(buffer, 0, buffer.Length)))
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }
                    return ms;
                }
            }
            finally
            {
                if (null != response) response.Close();
                if (null != request) request.Abort();
                request = null;
            }
        }

        private static HttpWebRequest CreateRequest(string url, string reffer = "")
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 200;
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

            //request.AllowAutoRedirect = false; // 只有在 getPicture 時才會
            if (String.IsNullOrEmpty(reffer))
            {
                request.Referer = url.getRefferString();
            }
            else
            {
                request.Referer = reffer;
            }
            request.CookieContainer = myCookie; // 拿到上次成功連線的 cookie 當作是同一個 session 
            //request.CookieContainer.Add(new Uri("http://www.dm5.com"), new Cookie("isAdult", "1"));

            request.Headers["Cookie"] = "isAdult=1";

            request.Timeout = 16000;
            //request.ContentType = "image/jpeg";
            //request.Referer = url;
            request.Proxy = ProxySetting.getProxy();
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-TW,zh;q=0.8,en-US;q=0.6,en;q=0.4,zh-CN;q=0.2");
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "GB2312");
            //request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded;charset=gb2312;";
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.KeepAlive = false;
            return request;
        }

        private static HttpWebResponse CreateResponse(HttpWebRequest request, string url, int remainTries = 50)
        {
            // 因為重新建立一個 request 時，url會被uri破壞，懶得重轉碼，就加入 url 參數使用
            int origTries = remainTries;

            HttpWebResponse response = null;
            int maxRemainTreis = remainTries;
            string errMsg = String.Empty;
            while (null == response && remainTries >= 0)
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (Exception e)
                {
                    //if (e.Message.Contains("(403)")) throw;
                    errMsg = e.ToString();

                    // 重試超過5次才開始記 log (因為半開連線數的限制，多執行緒很容易超過連線數的設定)
                    if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                    {
                        NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                    }
                    if (response != null) response.Close();
                    if (request != null) request.Abort();
                    System.Threading.Thread.Sleep(800);
                    request = null;
                    GC.Collect();
                    if (e.Message.Contains("作業逾時")) System.Threading.Thread.Sleep(1000);
                    request = CreateRequest(url);
                    request.CookieContainer = ChangeSession();
                }
                remainTries--;
            }
            if (null == response) throw new NullReferenceException("連線發生錯誤，且重新測試超過" + maxRemainTreis.ToString() + "次！！[" + errMsg + "]");


            return response;
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

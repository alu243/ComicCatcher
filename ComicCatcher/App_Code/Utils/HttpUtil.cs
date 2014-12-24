using System;
using System.Text;

using System.Net;
using System.IO;

using Helpers;
using System.Reflection;
namespace Utils
{
    public static class HttpUtil
    {
        static CookieContainer myCookie = new CookieContainer();

        public static string getResponse(string url, int remainTries = 10)
        {
            HttpWebRequest request = CreateRequest(url);
            HttpWebResponse response = CreateResponse(request, remainTries);
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

        public static string getUtf8Response(string url, int remainTries = 10)
        {
            HttpWebRequest request = CreateRequest(url);
            HttpWebResponse response = CreateResponse(request, remainTries);
            try
            {
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
            }
            finally
            {
                if (null != response) response.Close();
                if (null != request) request.Abort();
                request = null;
            }
        }

        public static MemoryStream getPictureResponse(string url, int remainTries = 10)
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
            HttpWebRequest request = CreateRequest(url);
            request.AllowAutoRedirect = false;
            HttpWebResponse response = CreateResponse(request, remainTries);

            if ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399)
            {
                string redirectUrl = response.Headers["Location"];
                redirectUrl = Encoding.GetEncoding("GB2312").GetString(Encoding.GetEncoding(1252).GetBytes(redirectUrl));
                //Encoding.GetEncoding("GB2312").GetString(Encoding.GetEncoding(1252).GetBytes(redirectUrl));

                //string urlString = redirectUrl.Substring(0, redirectUrl.IndexOf("?"));
                //string queryString = redirectUrl.Substring(redirectUrl.IndexOf("?") + 1);

                //redirectUrl = urlString + "?" + System.Web.HttpUtility.UrlEncode(queryString, Encoding.GetEncoding("gb2312"));

                request = CreateRequest(redirectUrl);
                response = CreateResponse(request, remainTries);
            }

            try
            {
                using (BinaryReader br = new BinaryReader(response.GetResponseStream()))
                {
                    MemoryStream ms = new MemoryStream();
                    ms.Write(br.ReadBytes(Convert.ToInt32(response.ContentLength)), 0, Convert.ToInt32(response.ContentLength));
                    //byte[] buffer = new byte[8192];
                    //int count = 0;
                    //while (0 < (count = br.Read(buffer, 0, 8192)))
                    //{
                    //    ms.Write(buffer, 0, count);
                    //}
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

        private static HttpWebRequest CreateRequest(string url)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 120;
            HttpWebRequest request = null;
            //if (url.Split('?').Length > 1)
            //{
            //    //url = System.Web.HttpUtility.UrlDecode(url, Encoding.GetEncoding("GB2312"));
            //    //url = urls[0] + "?" + System.Web.HttpUtility.UrlEncode(urls[1], Encoding.GetEncoding("GB2312")).Replace("url%3d", "url=");
            //    string[] urls = url.Split('?');
            //    url = urls[0] + "?" + Uri.EscapeUriString(urls[1]).Replace("url%3d", "url=");
            //}
            Uri uri = HackedUri.Create(url);

            request = (HttpWebRequest)WebRequest.Create(uri);
            //request = (HttpWebRequest)WebRequest.Create(url);

            //request = (HttpWebRequest)WebRequest.Create(HackedUri.Create(url));

            //request = (HttpWebRequest)WebRequest.Create(new Uri(Uri.EscapeUriString(url)));

            //request.AllowAutoRedirect = false;
            request.Referer = url.getRefferString();
            request.CookieContainer = myCookie; // 拿到上次成功連線的 cookie 當作是同一個 session 
            //request.CookieContainer.Add(new Uri("http://www.dm5.com"), new Cookie("isAdult", "1"));


            request.Timeout = 10000;
            //request.ContentType = "image/jpeg";
            //request.Referer = url;
            request.Proxy = ProxySetting.getProxy();
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "GB2312");
            //request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded;charset=gb2312;";
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.KeepAlive = false;
            return request;
        }

        private static HttpWebResponse CreateResponse(HttpWebRequest request, int remainTries = 10)
        {
            if (myCookie.GetCookies(new Uri("http://www.dm5.com")).Count <= 0)
            {
                Cookie dm5Cookie = new Cookie("isAdult", "1");
                dm5Cookie.Domain = "www.dm5.com";
                dm5Cookie.Path = "/";
                dm5Cookie.Expires = DateTime.Now.AddDays(1);
                myCookie.Add(dm5Cookie);
            }

            string url = request.RequestUri.ToString();
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
                    NLogger.Error("讀取url內容發生錯誤," + url + Environment.NewLine + e.ToString());
                    if (response != null) response.Close();
                    if (request != null) request.Abort();
                    request = CreateRequest(url);
                }
                remainTries--;
            }
            if (null == response) throw new NullReferenceException("連線發生錯誤，且重新測試超過" + maxRemainTreis.ToString() + "次！！[" + errMsg + "]");


            return response;
        }
    }

    public static class HackedUri
    {
        private const GenericUriParserOptions c_Options =
            GenericUriParserOptions.Default |
            GenericUriParserOptions.DontUnescapePathDotsAndSlashes |
            GenericUriParserOptions.Idn |
            GenericUriParserOptions.IriParsing;
        private static readonly GenericUriParser s_SyntaxHttp = new GenericUriParser(c_Options);
        private static readonly GenericUriParser s_SyntaxHttps = new GenericUriParser(c_Options);

        static HackedUri()
        {
            // Initialize the scheme 
            FieldInfo fieldInfoSchemeName = typeof(UriParser).GetField("m_Scheme", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfoSchemeName == null)
            {
                throw new MissingFieldException("'m_Scheme' field not found");
            }
            fieldInfoSchemeName.SetValue(s_SyntaxHttp, "http");
            fieldInfoSchemeName.SetValue(s_SyntaxHttps, "https");

            FieldInfo fieldInfoPort = typeof(UriParser).GetField("m_Port", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfoPort == null)
            {
                throw new MissingFieldException("'m_Port' field not found");
            }
            fieldInfoPort.SetValue(s_SyntaxHttp, 80);
            fieldInfoPort.SetValue(s_SyntaxHttps, 443);
        }

        public static Uri Create(string url)
        {
            Uri result = new Uri(url);

            if (url.IndexOf("%2F", StringComparison.OrdinalIgnoreCase) != -1)
            {
                UriParser parser = null;
                switch (result.Scheme.ToLowerInvariant())
                {
                    case "http":
                        parser = s_SyntaxHttp;
                        break;
                    case "https":
                        parser = s_SyntaxHttps;
                        break;
                }

                if (parser != null)
                {
                    // Associate the parser 
                    FieldInfo fieldInfo = typeof(Uri).GetField("m_Syntax", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fieldInfo == null)
                    {
                        throw new MissingFieldException("'m_Syntax' field not found");
                    }
                    fieldInfo.SetValue(result, parser);
                }
            }
            return result;
        }
    }

}

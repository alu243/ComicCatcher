using System;
using System.Text;

using System.Net;
using System.IO;

using Helpers;
namespace Utils
{
    public static class HttpUtil
    {
        static CookieContainer myCookie = new CookieContainer();

        public static string getResponse(string url, int remainTries = 10)
        {
            HttpWebRequest request = CreateRequest(url);
            HttpWebResponse response = null;
            while (null == response && remainTries >= 0)
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (Exception e)
                {
                    NLogger.Error("讀取url內容發生錯誤," + url + Environment.NewLine + e.ToString());
                    if (response != null) response.Close();
                    if (request != null) request.Abort();
                    request = CreateRequest(url);
                }
                remainTries--;
            }
            try
            {
                if (null == response) throw new NullReferenceException("連線發生錯誤，且重新測試超過10次！！");

                Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                StreamReader readStream = new StreamReader(response.GetResponseStream(), encode);

                StringBuilder sb = new StringBuilder();
                Char[] read = new Char[8192];
                // Reads 256 characters at a time.    
                int count = readStream.Read(read, 0, 8192);
                while (count > 0)
                {
                    // Dumps the 256 characters on a string and displays the string to the console.
                    sb.Append(new String(read, 0, count));
                    count = readStream.Read(read, 0, 8192);
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

        public static MemoryStream getPictureResponse(string url)
        {
            var request = CreateRequest(url);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (BinaryReader br = new BinaryReader(response.GetResponseStream()))
                    {
                        MemoryStream ms = new MemoryStream();
                        ms.Write(br.ReadBytes(Convert.ToInt32(response.ContentLength)), 0, Convert.ToInt32(response.ContentLength));
                        //byte[] buffer = new byte[4096];
                        //int count = 0;
                        //while (0 < (count = br.Read(buffer, 0, 4096)))
                        //{
                        //    ms.Write(buffer, 0, count);
                        //}
                        return ms;
                        //return readStream.ReadToEnd();
                    }
                }
            }
            finally
            {
                if (null != request) request.Abort();
            }
        }

        public static MemoryStream getPictureResponse(string url, int retryTimes)
        {
            Exception myEx = null;
            for (int i = 0; i < retryTimes; i++)
            {
                try
                {
                    return getPictureResponse(url);
                }
                catch (Exception ex)
                {
                    myEx = ex;
                }
            }
            throw myEx;
        }

        private static HttpWebRequest CreateRequest(string url)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 120;
            HttpWebRequest request = null;

            request = (HttpWebRequest)WebRequest.Create(url);
            request.Referer = url.getRefferString();
            request.CookieContainer = myCookie; // 拿到上次成功連線的 cookie 當作是同一個 session 
            //request.Timeout = 10000;
            //request.ContentType = "image/jpeg";
            //request.Referer = url;
            request.Proxy = ProxySetting.getProxy();
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.KeepAlive = false;
            return request;
        }

    }
}

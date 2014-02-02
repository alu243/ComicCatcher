using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Utils
{
    public class UsingProxy
    {
        private static bool? _isUseProxy;
        public static bool isUseProxy
        {
            get { return _isUseProxy ?? true; }
            set { _isUseProxy = value; }
        }

        public static string ProxyUrl { get; set; }

        public static int ProxyPort { get; set; }

        public static WebProxy getProxy()
        {
            if (isUseProxy) return new WebProxy(ProxyUrl, ProxyPort);

            return null;
        }
    }
}

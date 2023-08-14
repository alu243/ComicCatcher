using System.Net;

namespace ComicCatcherLib.Helpers;

public class ProxySetting
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
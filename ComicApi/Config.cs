using ComicApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ComicApi
{
    public static class Config
    {
        public static string ComicPath => "wwwroot/comic";
        public static string GetDbPath() => "volume/comic";

        public static int CacheMinute => 5;
    }
}

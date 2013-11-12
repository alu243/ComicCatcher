using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicCatcher.App_Code.Util
{
    public static class StringExtenssion
    {
        /// <summary>
        /// 清除有可能會造錯成錯誤目之字串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string trimEscapeString(this string s)
        {
            string ss = s;
            ss = ss.Replace("?", "");
            ss = ss.Replace("*", "＊");
            ss = ss.Replace(":", "：");
            ss = ss.Replace("\\", "＼");
            ss = ss.Replace("/", "／");
            ss = ss.Replace(">", "＞");
            ss = ss.Replace("<", "＜");
            ss = ss.Replace("|", "｜");
            ss = ss.Replace("\"", "");
            return ss;
        }

        public static string getRefferString(this string s)
        {
            return "http://" + new Uri(s).Host.ToString();
            //return new Uri(s).Host.ToString();
        }
    }
}

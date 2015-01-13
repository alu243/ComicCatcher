using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace ComicCacherUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestPictureDownload()
        {
            //////Helpers.ProxySetting.isUseProxy = false;
            //////string url = "http://beiyong.bukamh.com/pic.php?url=http%3A%2F%2Fimages.dmzj.com%2Fh%2F%BA%FA%CC%D2%B5%C4%CC%C7%B9%FB%2F%B5%DA25%BB%B0%2F002.jpg";
            ////////string url = "http://beiyong.bukamh.com/pic.php?url=http:%2f%2fimages.dmzj.com%2fh%2f胡桃的糖果%2f第25话%2f海报3-1.jpg";
            ////////var a = System.Web.HttpUtility.ParseQueryString(url);
            ////////url = System.Web.HttpUtility.UrlDecode(url, System.Text.Encoding.GetEncoding("GB2312"));
            //////Utils.HttpUtil.getPictureResponse(url);


            string a = "var WebimgServer = new Array();var WebimgServerURL = new Array();WebimgServer[0]=\"秷夔\";WebimgServerURL[0]=\"http://sx2.bukamh.com:81\";//扢离掛華芞督昢var NativePicServer=\"http://mh.xindm.cn\";//扢离煦霜芞督昢var SonPicServer=\"http://sx2.bukamh.com:81\";";
            Regex rhost = new Regex(@"WebimgServerURL\[0\].*?;", RegexOptions.Compiled);
            string b = rhost.Match(a).Value;
            string c = b;
        }
    }
}

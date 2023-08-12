using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Text;

namespace ComicCacherUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestPictureDownload()
        {
            Helpers.ProxySetting.isUseProxy = false;
            string url = "http://beiyong.bukamh.com/pic.php?url=http%3A%2F%2Fimages.dmzj.com%2Fh%2F%BA%FA%CC%D2%B5%C4%CC%C7%B9%FB%2F%B5%DA25%BB%B0%2F002.jpg";
            //string url = "http://beiyong.bukamh.com/pic.php?url=http:%2f%2fimages.dmzj.com%2fh%2f胡桃的糖果%2f第25话%2f海报3-1.jpg";
            //var a = System.Web.HttpUtility.ParseQueryString(url);
            //url = System.Web.HttpUtility.UrlDecode(url, System.Text.Encoding.GetEncoding("GB2312"));
            Utils.HttpUtil.getFileResponse(url, "", "test");


        }

        [TestMethod]
        public void TestPictureUrlParse()
        {
            Helpers.ProxySetting.isUseProxy = false;
            string url = "http://sx2.bukamh.com:81/pic.php?url=http:%2f%2fimages.dmzj.com%2fd%2f%B6%AB%BE%A9%86%D0%D6%D6re%2fChapter6%2f09.jpg";
            //var a = System.Web.HttpUtility.ParseQueryString(url);
            url = System.Web.HttpUtility.UrlDecode(url, System.Text.Encoding.GetEncoding("GB2312"));
            //Utils.HttpUtil.getPictureResponse(url);
            Console.Write(url);
        }

        [TestMethod]
        public void TestJavascriptMatchHostValue()
        {
            string a = "var WebimgServer = new Array();var WebimgServerURL = new Array();WebimgServer[0]=\"秷夔\";WebimgServerURL[0]=\"http://sx2.bukamh.com:81\";//扢离掛華芞督昢var NativePicServer=\"http://mh.xindm.cn\";//扢离煦霜芞督昢var SonPicServer=\"http://sx2.bukamh.com:81\";";
            Regex rhost = new Regex(@"WebimgServerURL\[0\].*?;", RegexOptions.Compiled);
            string b = rhost.Match(a).Value;
            string c = b;
        }

        [TestMethod]
        public void TestRedirectUrlParsing()
        {
            string redirectUrl = "http://sx2img.bukamh.com:82/d/¶«¾©ÐÖÖre/13/0016.jpg";
            redirectUrl = Encoding.GetEncoding("GB2312").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(redirectUrl));
            Console.Write(redirectUrl);
        }

        [TestMethod]
        public void TestEvalJsCode()
        {
            // javascript unpacker
            // http://matthewfl.com/unPacker.html
            string jsCode = "eval(function(p,a,c,k,e,d){e=function(c){return(c<a?\"\":e(parseInt(c/a)))+((c=c%a)>35?String.fromCharCode(c+29):c.toString(36))};if(!''.replace(/^/,String)){while(c--)d[e(c)]=k[c]||e(c);k=[function(e){return d[e]}];e=function(){return'\\\\w+'};c=1;};while(c--)if(k[c])p=p.replace(new RegExp('\\\\b'+e(c)+'\\\\b','g'),k[c]);return p;}('n 4(){1 6=3;1 5=\\'b\\';1 a=\\'9\\';1 8=\"e://j.k-m-l-h.c.g/f/r/3\";1 2=[\"/s.7\",\"/t.7\"];q(1 i=0;i<2.o;i++){2[i]=8+2[i]+\\'?6=3&5=b&a=9\\'}p 2}1 d;d=4();',30,30,'|var|pvalue|189541|dm5imagefun|key|cid|png|pix|5aa9feec776479c6|ak|a835d3c8dc0d5439c27007f30ea017b7|cdndm5||http|12|com|50||manhua1023|146|123|71|function|length|return|for|11056|1_4338|2_3608'.split('|'),0,{}))";
            ComicModels.ComicUtil util = new ComicModels.ComicUtil();
            string jsCode2 = util.EvalJScript("var cs = " + jsCode) as string;

            Assert.AreEqual(jsCode2, String.Empty);

        }
    }
}

using System;
using System.IO;
using ComicCatcher.App_Code.Helpers;
using Utils;

namespace ComicCatcher.App_Code.Utils;

public class ComicUtil
{
    public static MemoryStream GetPicture(string url)
    {
        int origTries = 5;
        int remainTries = origTries;
        while (remainTries >= 0)
        {
            try
            {
                var result = HttpUtil.getFileResponse(url, "", "icon");
                return result;
            }
            catch (Exception e)
            {
                //if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                //{
                //    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                //}
                System.Threading.Thread.Sleep(800);
                GC.Collect();
                remainTries--;
            }
        }
        throw new NullReferenceException(string.Format("GetPicture:連線發生錯誤，且重新測試超過{0}次！！", origTries));
    }

    public static MemoryStream GetPicture(string url, string reffer, string fileName)
    {
        int origTries = 20;
        int remainTries = origTries;
        while (remainTries >= 0)
        {
            try
            {
                var result = HttpUtil.getFileResponse(url, reffer, fileName);
                return result;
            }
            catch (Exception e)
            {
                //if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                //{
                //    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                //}
                System.Threading.Thread.Sleep(800);
                GC.Collect();
                remainTries--;
            }
        }
        throw new NullReferenceException(string.Format("GetPicture2:連線發生錯誤，且重新測試超過{0}次！！", origTries));
    }

    public static string GetContent(string url)
    {
        int origTries = 20;
        int remainTries = origTries;
        while (remainTries >= 0)
        {
            try
            {
                var result = HttpUtil.getResponse(url);
                return result;
            }
            catch (Exception e)
            {
                //if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                //{
                //    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                //}
                System.Threading.Thread.Sleep(800);
                GC.Collect();
                remainTries--;
            }
        }
        throw new NullReferenceException(string.Format("GetContent:連線發生錯誤，且重新測試超過{0}次！！", origTries));
    }

    public static string GetUtf8Content(string url, string reffer = "")
    {
        int origTries = 10;
        int remainTries = origTries;
        while (remainTries >= 0)
        {
            try
            {
                var result = HttpUtil.getUtf8Response(url, reffer);
                return result;
            }
            catch (Exception e)
            {
                //if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                //{
                //    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                //}
                System.Threading.Thread.Sleep(500);
                GC.Collect();
                remainTries--;
            }
        }
        throw new NullReferenceException(string.Format("GetUtf8Content:連線發生錯誤，且重新測試超過{0}次！！", origTries));
    }



    private IJsEngine engine = null;
    private ComicUtil(IJsEngine engine)
    {
        this.engine = engine;
    }

    public static ComicUtil CreateVsaEngine()
    {
        return new ComicUtil(new VsaEngine());
    }

    public object EvalJScript(string jscript) => engine.EvalJScript(jscript.StartsWith("eval") ? jscript : "eval(" + jscript.Trim(';') + ")");
    //        private static object _evaluator = null;
    //        private static Type _evaluatorType = null;
    //        private static readonly string _jscriptSource =
    //            @"package Evaluator
    //            {
    //               class Evaluator
    //               {
    //                  public function Eval(expr : String) : String 
    //                  { 
    //                     return eval(expr); 
    //                  }
    //               }
    //            }";


    //        public static object EvalJScript(string statement)
    //        {
    //            return _evaluatorType.InvokeMember(
    //                        "Eval",
    //                        BindingFlags.InvokeMethod,
    //                        null,
    //                        _evaluator,
    //                        new object[] { statement }
    //                     );
    //        }


    //        static ComicUtil()
    //        {
    //            ICodeCompiler compiler = new JScriptCodeProvider().CreateCompiler();

    //            CompilerParameters parameters = new CompilerParameters();
    //            parameters.GenerateInMemory = true;

    //            CompilerResults results = compiler.CompileAssemblyFromSource(parameters, _jscriptSource);

    //            Assembly assembly = results.CompiledAssembly;
    //            _evaluatorType = assembly.GetType("Evaluator.Evaluator");

    //            _evaluator = Activator.CreateInstance(_evaluatorType);
    //        }

    /// <summary>
    /// 取資料後存檔
    /// </summary>
    /// <param name="path"></param>
    public static void Download(string pictureUrl, string reffer, string fullFileName)
    {
        try
        {
            if (false == Directory.Exists(Path.GetDirectoryName(fullFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));
            }

            using (MemoryStream ms = ComicUtil.GetPicture(pictureUrl, reffer, Path.GetFileName(fullFileName)))
            {
                using (FileStream fs = new FileStream(fullFileName, FileMode.Create))
                {
                    ms.CopyTo(fs);
                    fs.Close();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("存檔時發生錯誤，原因：" + ex.ToString());
        }
    }
}
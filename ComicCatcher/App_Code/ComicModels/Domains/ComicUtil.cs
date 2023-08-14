using ComicCatcher.Helpers;
using ComicCatcher.Utils;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ComicCatcher.ComicModels.Domains;

public class ComicUtil
{
    public static void SetConcurrencyHttpRequest(int maxConnetions)
    {
        HttpClientUtil.SetConnections(maxConnetions);
    }

    public static async Task<Stream> GetPicture(string url, string refer = "")
    {
        int origTries = 10;
        int remainTries = origTries;
        while (remainTries >= 0)
        {
            try
            {
                var result = await HttpClientUtil.GetStreamResponse(url, refer);
                result.Position = 0;
                return result;
            }
            catch (Exception e)
            {
                if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                {
                    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                }
                await Task.Delay(5000);
                remainTries--;
            }
        }
        throw new NullReferenceException(string.Format("GetPicture:連線發生錯誤，且重新測試超過{0}次！！", origTries));
    }

    //public static string GetGbContent(string url)
    //{
    //    int origTries = 20;
    //    int remainTries = origTries;
    //    while (remainTries >= 0)
    //    {
    //        try
    //        {
    //            var result = HttpUtil.GetResponse(url);
    //            return result;
    //        }
    //        catch (Exception e)
    //        {
    //            //if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
    //            //{
    //            //    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
    //            //}
    //            Task.Delay(800);
    //            remainTries--;
    //        }
    //    }
    //    throw new NullReferenceException(string.Format("GetContent:連線發生錯誤，且重新測試超過{0}次！！", origTries));
    //}

    public static async Task<string> GetUtf8Content(string url, string reffer = "")
    {
        int origTries = 10;
        int remainTries = origTries;
        while (remainTries >= 0)
        {
            try
            {
                var result = await HttpClientUtil.GetStringResponse(url, reffer);
                return result;
            }
            catch (Exception e)
            {
                if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                {
                    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                }
                await Task.Delay(5000);
                remainTries--;
            }
        }
        throw new NullReferenceException(string.Format("GetUtf8Content:連線發生錯誤，且重新測試超過{0}次！！", origTries));
    }

    private IJsEngine engine;
    private ComicUtil(IJsEngine engine)
    {
        this.engine = engine;
    }

    public static ComicUtil CreateVsaEngine()
    {
        return new ComicUtil(new JintEngine());
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
}
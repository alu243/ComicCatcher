using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ComicCatcher.App_Code.Helpers;
using ComicCatcher.ComicModels;
using Utils;

namespace ComicCatcher.App_Code.ComicModels.Domains;

public class ComicUtil
{
    public static MemoryStream GetPicture(string url, string refer = "")
    {
        int origTries = 5;
        int remainTries = origTries;
        while (remainTries >= 0)
        {
            try
            {
                var result = HttpClientUtil.GetStreamResponse(url, refer).Result;
                return result;
            }
            catch (Exception e)
            {
                //if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                //{
                //    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                //}
                Thread.Sleep(800);
                remainTries--;
            }
        }
        throw new NullReferenceException(string.Format("GetPicture:連線發生錯誤，且重新測試超過{0}次！！", origTries));
    }

    public static string GetGbContent(string url)
    {
        int origTries = 20;
        int remainTries = origTries;
        while (remainTries >= 0)
        {
            try
            {
                var result = HttpUtil.GetResponse(url);
                return result;
            }
            catch (Exception e)
            {
                //if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                //{
                //    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                //}
                Thread.Sleep(800);
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
                var result = HttpClientUtil.GetStringResponse(url, reffer).Result;
                return result;
            }
            catch (Exception e)
            {
                //if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                //{
                //    NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                //}
                Thread.Sleep(500);
                GC.Collect();
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

    public static void DownloadChapter(DownloadChapterTask task)
    {
        if (false == Directory.Exists(task.Path)) Directory.CreateDirectory(task.Path);

        int threadCount = task.Downloader.GetRoot().ThreadCount;
        int startPage = 0;
        int upperPage = (threadCount > task.Chapter.Pages.Count ? task.Chapter.Pages.Count : threadCount); // 一次下載設定的頁數，如果剩不到40頁就下載剩下的頁數

        var pages = new List<ComicPage>(task.Chapter.Pages);
        do
        {
            var batch = pages.Take(threadCount);
            pages = pages.Skip(threadCount).ToList();
            List<Thread> threadPool = new List<Thread>();
            foreach (var page in batch)
            {
                Thread t = new Thread(DownloadPicture);
                t.IsBackground = true;
                t.Start(new DownloadPageTask(page, task.Path));
                threadPool.Add(t);
            }
            foreach (var t in threadPool) t.Join();
            threadPool.Clear();
        } while (pages.Count > 0);
    }

    private static void DownloadPicture(object downloadTask)
    {
        try
        {
            var threadMessage = $" Thread ID=[{Thread.CurrentThread.GetHashCode()}]";
            var tasker = (DownloadPageTask)downloadTask;
            using (MemoryStream ms = GetPicture(tasker.Page.Url, tasker.Page.Refer))
            {
                using (FileStream fs = new FileStream(tasker.GetFullPath(), FileMode.Create))
                {
                    ms.CopyTo(fs);
                    fs.Close();
                }
                ms.Close();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("下載漫畫圖片時發生錯誤，原因：" + ex);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Utils;
using System.IO;
using Microsoft.JScript;
using Microsoft.JScript.Vsa;
using System.CodeDom.Compiler;
using System.Reflection;
using Helpers;
using System.Threading;

namespace ComicModels
{
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
                    if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                    {
                        NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                    }
                    System.Threading.Thread.Sleep(800);
                    GC.Collect();
                    remainTries--;
                }
            }
            throw new NullReferenceException(String.Format("連線發生錯誤，且重新測試超過{0}次！！", origTries));
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
                    if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                    {
                        NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                    }
                    System.Threading.Thread.Sleep(800);
                    GC.Collect();
                    remainTries--;
                }
            }
            throw new NullReferenceException(String.Format("連線發生錯誤，且重新測試超過{0}次！！", origTries));
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
                    if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                    {
                        NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                    }
                    System.Threading.Thread.Sleep(800);
                    GC.Collect();
                    remainTries--;
                }
            }
            throw new NullReferenceException(String.Format("連線發生錯誤，且重新測試超過{0}次！！", origTries));
        }

        public static string GetUtf8Content(string url, string reffer = "")
        {
            int origTries = 20;
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
                    if ((origTries - remainTries) >= 5 && (origTries - remainTries) % 5 == 0)
                    {
                        NLogger.Error("讀取url內容發生錯誤(Thread ID=" + Thread.CurrentThread.GetHashCode().ToString() + "), 已重試 " + (origTries - remainTries) + "次," + url + Environment.NewLine + e.ToString());
                    }
                    System.Threading.Thread.Sleep(800);
                    GC.Collect();
                    remainTries--;
                }
            }
            throw new NullReferenceException(String.Format("連線發生錯誤，且重新測試超過{0}次！！", origTries));
        }



        private VsaEngine engine = null;
        public ComicUtil()
        {
            engine = VsaEngine.CreateEngine();
        }
        public object EvalJScript(string JScript)
        {
            object Result = null;
            try
            {
                Result = Eval.JScriptEvaluate(JScript, "unsafe", engine);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return Result;
        }




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
}

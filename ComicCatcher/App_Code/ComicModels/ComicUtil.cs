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
namespace ComicModels
{
    public class ComicUtil
    {
        public static MemoryStream GetPicture(string url)
        {
            return HttpUtil.getPictureResponse(url);
        }

        public static string GetContent(string url)
        {
            return HttpUtil.getResponse(url);
        }


        public static string GetUtf8Content(string url)
        {
            return HttpUtil.getUtf8Response(url);
        }



        private static VsaEngine _vsaEngine = VsaEngine.CreateEngine();
        public static object EvalJScript(string JScript)
        {
            object Result = null;
            try
            {
                Result = Eval.JScriptEvaluate(JScript, _vsaEngine);
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

﻿using Jint;

namespace ComicCatcherLib.Helpers;

public class JintEngine : IJsEngine
{
    //Microsoft.JScript.Vsa.VsaEngine engine;
    private Engine engine;
    public JintEngine()
    {
        engine = new Engine();
        //engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
    }
    public object EvalJScript(string jscript)
    {
        object result = null;
        try
        {
            result = engine.Execute(jscript).GetCompletionValue().ToObject();
            if (result is object[])
            {
                var strList = new List<string>();
                foreach (object obj in (object[])result)
                {
                    strList.Add(obj.ToString());
                }
                return strList.ToArray();
            }
            //result = this.engine.SetValue("cs", result).Execute("cs = 'okk';");
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return result;
    }
}

//public class VsaEngine : IJsEngine
//{
//    //Microsoft.JScript.Vsa.VsaEngine engine;
//    public VsaEngine()
//    {
//        //engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
//    }
//    public object EvalJScript(string jscript)
//    {
//        object result = null;
//        //try
//        //{
//        //    Microsoft.JScript.Eval Result = Eval.JScriptEvaluate(jscript, "unsafe", engine);
//        //}
//        //catch (Exception ex)
//        //{
//        //    return ex.Message;
//        //}

//        return result;
//    }
//}
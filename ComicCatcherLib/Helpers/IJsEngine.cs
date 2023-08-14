namespace ComicCatcherLib.Helpers;

public interface IJsEngine
{
    object EvalJScript(string jscript);
}
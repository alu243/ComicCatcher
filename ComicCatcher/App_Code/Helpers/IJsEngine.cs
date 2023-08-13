namespace ComicCatcher.App_Code.Helpers
{
    public interface IJsEngine
    {
        object EvalJScript(string jscript);
    }
}

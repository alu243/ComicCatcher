using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicCatcher.App_Code.Helpers
{
    public interface IJsEngine
    {
        object EvalJScript(string jscript);
    }
}

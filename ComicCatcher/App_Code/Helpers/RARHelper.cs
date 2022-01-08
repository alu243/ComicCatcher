using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;
using System.Threading;
namespace Helpers
{
    public class RARHelper
    {
        string _rarPath = String.Empty;

        public RARHelper(string rarPath)
        {
            this._rarPath = rarPath;
        }


        public void ArchiveDirectory(string path)
        {
            if (false == Directory.Exists(path)) return; // 目錄不存在
            string[] dirs = Directory.GetDirectories(path);
            if (dirs.Length > 0) throw new ArgumentNullException("請確認該資料夾下沒有子目錄後再進行壓縮！");

            string rarArgument = " a -df -r -rr1p -m5 -ep -ibck \"{0}\" \"{1}\"";


            string currRelatedPath = Path.GetFileName(path);
            string parentFullPath = Directory.GetParent(path).FullName;
            Utils.CMDUtil.ExecuteCommandAsync(new CommandObj() { fileName = this._rarPath, arguments = String.Format(rarArgument, currRelatedPath + ".rar", currRelatedPath), workdir = parentFullPath });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;
using System.Threading;
namespace RARHelper
{
    public class RARHelper
    {
        string _rarPath = String.Empty;

        //public RARHelper()
        //{
        //    //if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"WinRAR\WinRAR.exe")))
        //    //{
        //    //    _rarPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"WinRAR\WinRAR.exe");
        //    //}
        //    //else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"WinRAR\WinRAR.exe")))
        //    //{
        //    //    _rarPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"WinRAR\WinRAR.exe");
        //    //}
        //    if (File.Exists(Path.Combine(@"C:\Program Files (x86)", @"WinRAR\WinRAR.exe")))
        //    {
        //        _rarPath = Path.Combine(@"C:\Program Files (x86)", @"WinRAR\WinRAR.exe");
        //    }
        //    else if(File.Exists(Path.Combine(@"C:\Program Files", @"WinRAR\WinRAR.exe")))
        //    {
        //        _rarPath = Path.Combine(@"C:\Program Files", @"WinRAR\WinRAR.exe");
        //    }
        //}

        public RARHelper(string rarPath)
        {
            this._rarPath = rarPath;
        }


        public void archiveDirectory(string path)
        {
            if (false == Directory.Exists(path)) return; // 目錄不存在
            string[] dirs = Directory.GetDirectories(path);
            if (dirs.Length > 0) throw new ArgumentNullException("請確認該資料夾下沒有子目錄後再進行壓縮！");

            string rarArgument = " a -df -r -rr1p -m5 -ep -ibck \"{0}\" \"{1}\"";


            string currRelatedPath = Path.GetFileName(path);
            string parentFullPath = Directory.GetParent(path).FullName;
            CMDHelper.CMDHelper.ExecuteCommandAsync(new CommandObj() { fileName = this._rarPath, arguments = String.Format(rarArgument, currRelatedPath + ".rar", currRelatedPath), workdir = parentFullPath });
        }
    }
}

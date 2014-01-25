using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ComicCatcher
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());

            try
            {
                System.IO.File.Delete(@".\System.Data.SQLite.dll");
            }
            catch { /* doNothing */ }

        }
    }
}

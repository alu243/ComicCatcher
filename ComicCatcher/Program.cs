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

            //try
            //{
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            //}
            //catch { /* doNothing */ }
            //try
            //{
                Application.Run(new frmMain());
            //}
            //catch (Exception ex)
            //{
            //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("error.Log"))
            //    {
            //        sw.WriteLine(ex.ToString());
            //        sw.Flush();
            //        sw.Close();
            //    }
            //}

            //try
            //{
            //    System.IO.File.Delete(@".\System.Data.SQLite.dll");
            //}
            //catch { /* doNothing */ }

        }
    }
}

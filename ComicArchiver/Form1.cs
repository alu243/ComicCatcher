using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web.Caching;
namespace ComicArchiver
{
    public partial class Form1 : Form
    {
        private const string CACHE_KEY = "APPCACHEKEY";
        public Form1()
        {
            InitializeComponent();
        }

        protected void browseDirectory(string path)
        {
            try
            {
                path = path.Replace("\"", "");
                tvFolder.Nodes.Clear();
                TreeNode root = new TreeNode(Path.GetFileName(path), 0, 0);
                tvFolder.Nodes.Add(root);

                string[] dirs = Directory.GetDirectories(path);
                string[] rars = Directory.GetFiles(path, "*.rar");
                string[] newDirs = new string[dirs.Length + rars.Length];
                Array.Copy(dirs, newDirs, dirs.Length);
                Array.Copy(rars, 0, newDirs, dirs.Length, rars.Length);
                newDirs = newDirs.ToList().CustomSort().ToArray();
                Array.Reverse(newDirs);
                foreach (string dir in newDirs)
                {
                    if (0 != String.Compare(".RAR", Path.GetExtension(dir), true))
                        root.Nodes.Add(dir, Path.GetFileName(dir), 1, 1);
                    else
                        root.Nodes.Add(dir, Path.GetFileName(dir), 2, 2);
                }
                root.ExpandAll();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void txtPath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                browseDirectory(txtPath.Text.Trim());
            }
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            browseDirectory(txtPath.Text.Trim());
        }

        private void tvFolder_DoubleClick(object sender, EventArgs e)
        {
            archiveComic(tvFolder.SelectedNode.Name);
        }


        private void archiveComic(string path)
        {
            if (false == Directory.Exists(path)) return;
            string[] dirs = Directory.GetDirectories(path);
            if (dirs.Length > 0)
            {
                MessageBox.Show("請確認該資料夾下沒有子目錄後再進行壓縮！");
                return;
            }

            string rarCmd = "\"C:\\Program Files\\WinRAR\\WinRAR.exe\"";
            string rarArgument = " \"-cpComicShelf\" \"{0}\"  -r \"{1}\"";


            //MessageBox.Show(path);
            //MessageBox.Show(String.Format(rarCmd, path, path));
            //ExecuteCommandSync();
            ExecuteCommandAsync(new CommandObj() { fileName = rarCmd, arguments = String.Format(rarArgument, path + ".rar", path) });

            //browseDirectory(Directory.GetParent(path).FullName);
            //await Task.Delay()
        }

        private void tvFolder_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                archiveComic(tvFolder.SelectedNode.Name);
            }
        }

        public void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                //System.Diagnostics.ProcessStartInfo procStartInfo =
                //    new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + (command as string));
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo((command as CommandObj).fileName, (command as CommandObj).arguments);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                //MessageBox.Show(command as string);
                //MessageBox.Show(result);
                // Display the command output.
                //Console.WriteLine(result);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public void ExecuteCommandAsync(CommandObj command)
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSync));
                //Make the thread as background thread.
                objThread.IsBackground = true;
                //Set the Priority of the thread.
                objThread.Priority = ThreadPriority.Normal;
                //Start the thread.
                objThread.Start(command);
            }
            catch (ThreadStartException objException)
            {
                MessageBox.Show(objException.ToString());
            }
            catch (ThreadAbortException objException)
            {
                MessageBox.Show(objException.ToString());
            }
            catch (Exception objException)
            {
                MessageBox.Show(objException.ToString());
            }
        }

        private void txtPath_MouseClick(object sender, EventArgs e)
        {
            txtPath.SelectAll();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void txtPath_MouseClick(object sender, MouseEventArgs e)
        {
            if (String.IsNullOrEmpty(txtPath.SelectedText))
                txtPath.SelectAll();
        }

        private void txtPath_Enter(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtPath.SelectedText))
                txtPath.SelectAll();
        }

        private void btnFileNext_Click(object sender, EventArgs e)
        {
            string[] allDir = GetAllDir(txtPath.Text.Trim());
            int pos = FindCurrentDirectory(allDir);

            pos = (pos >= allDir.Length - 1 ? allDir.Length - 1 : pos + 1);
            txtPath.Text = allDir[pos].Trim();
            browseDirectory(txtPath.Text.Trim());
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnFilePrev_Click(object sender, EventArgs e)
        {
            string[] allDir = GetAllDir(txtPath.Text.Trim());
            int pos = FindCurrentDirectory(allDir);

            pos = (pos < 0 ? 0 : pos - 1);
            txtPath.Text = allDir[pos].Trim();
            browseDirectory(txtPath.Text.Trim());
        }

        private int FindCurrentDirectory(string[] allDir)
        {
            int i = 0;
            for (i = 0; i < allDir.Length; i++)
            {
                if (0 == String.Compare(txtPath.Text.Trim(), allDir[i].Trim(), true))
                    break;
            }
            return i;
        }

        private string[] GetAllDir(string path)
        {
            if (null == AppMain.Cache[CACHE_KEY])
            {
                DirectoryInfo di = Directory.GetParent(path);
                string[] dirs = Directory.GetDirectories(di.FullName);
                Array.Sort(dirs);
                AppMain.Cache.Insert(CACHE_KEY, dirs, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60));
            }
            if (0 >= ((string[])AppMain.Cache[CACHE_KEY]).Length)
            {
                DirectoryInfo di = Directory.GetParent(path);
                string[] dirs = Directory.GetDirectories(di.FullName);
                Array.Sort(dirs);
                AppMain.Cache.Insert(CACHE_KEY, dirs, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60));
            }
            return AppMain.Cache[CACHE_KEY] as string[];
        }

    }
    public class CommandObj
    {
        public string fileName { get; set; }
        public string arguments { get; set; }
    }
}

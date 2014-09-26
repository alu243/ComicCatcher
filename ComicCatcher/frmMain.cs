using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;

using Helpers;
using Utils;
using Models;
using ComicModels;
namespace ComicCatcher
{
    public partial class frmMain : Form
    {
        private static readonly Queue<Tasker> queue = new Queue<Tasker>();

        private Settings settings = null;

        private IComicCatcher xindm = new Xindm();

        //private DownloadedList dwnedList = null;

        public frmMain()
        {
            ExportInteropFile();
            InitializeComponent();
            NLogger.SetBox(this.txtInfo);
        }

        private void ExportInteropFile()
        {
            Assembly asm;
            Stream asmfs;
            asm = Assembly.GetExecutingAssembly();
            asmfs = asm.GetManifestResourceStream("ComicCatcher.x86.System.Data.SQLite.dll");
            //var files = asm.GetManifestResourceNames();

            using (FileStream fs = new FileStream(@".\System.Data.SQLite.dll", FileMode.Create, FileAccess.Write))
            {
                asmfs.Position = 0;
                int length = 4096;
                byte[] buffer = new Byte[length];
                int count = 0;
                while (0 < (count = asmfs.Read(buffer, 0, length)))
                {
                    fs.Write(buffer, 0, count);
                }
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

            InitialComicRootTree();
            lblUpdateDate.Text = "";
            lblUpdateChapter.Text = "";
            buildLocalComicDirComboBox();


            ConfigureSettings();
            GetSettings();

            RenewXML2DB();
            DownloadedList.LoadDB();
            //LoadDownloadList();
        }

        private void InitialComicRootTree()
        {
            tvComicTree.Nodes.Clear();

            ComicRoot cr = xindm.GetComicRoot();
            TreeNode root = new TreeNode(cr.WebSiteTitle);
            tvComicTree.Nodes.Add(root);
            root.Tag = cr;

            var groups = xindm.GetComicGroups();
            groups.ForEach(g =>
            {
                TreeNode tn = root.Nodes.Add(g.Url, g.Caption);
                tn.Tag = g;
            });
            tvComicTree.ExpandAll();
        }

        private void tvComicTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Cursor = System.Windows.Forms.Cursors.WaitCursor;
            //Application.DoEvents();
            try
            {
                if (null == tvComicTree.SelectedNode) return;

                #region IsComicNameNode
                if (NodeCheckUtil.IsComicNameNode(tvComicTree.SelectedNode))
                {
                    // 如果是漫畫名稱節點，就顯示icon圖片
                    lock (pbIcon)
                    {
                        pbIcon.Image = null;
                    }
                    ComicName comicName = tvComicTree.SelectedNode.Tag as ComicName;
                    try
                    {
                        lock (pbIcon)
                        {
                            pbIcon.Image = comicName.IconImage;
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogger.Error("顯示icon失敗," + ex.ToString());

                        lock (pbIcon)
                        {
                            pbIcon.Image = null;
                        }
                    }
                    finally
                    {
                        lock (pbIcon)
                        {
                            pbIcon_Paint(pbIcon, null);
                        }
                    }

                    // 顯示其餘資料
                    try
                    {
                        txtUrl.Text = tvComicTree.SelectedNode.Name;
                        lblUpdateDate.Text = comicName.LastUpdateDate;
                        lblUpdateChapter.Text = comicName.LastUpdateChapter;
                        if (false == cbRelateFolders.Items.Contains(tvComicTree.SelectedNode.Text)) cbRelateFolders.Items.Add(tvComicTree.SelectedNode.Text);
                        cbRelateFolders.Text = tvComicTree.SelectedNode.Text;
                    }
                    catch (Exception ex)
                    {
                        NLogger.Error("顯示資料失敗," + ex.ToString());
                        pbIcon.Image = null;
                    }
                }
                #endregion

                // 如果沒有子節點，就產生子節點
                if (tvComicTree.SelectedNode.Nodes.Count <= 0)
                {
                    buildComicNode(tvComicTree.SelectedNode);
                    tvComicTree.SelectedNode.Expand();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("取得節點訊息時發生錯誤：" + ex.ToString());
            }
            finally
            {
                Cursor = System.Windows.Forms.Cursors.Arrow;
                //Application.DoEvents();
            }
        }

        private void tvComicTree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                try
                {
                    Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    //Application.DoEvents();
                    buildComicNode(tvComicTree.SelectedNode);
                    tvComicTree.SelectedNode.Expand();
                }
                catch (Exception ex)
                {
                    NLogger.Error(ex.ToString());
                }
                finally
                {
                    Cursor = System.Windows.Forms.Cursors.Arrow;
                    //Application.DoEvents();
                }
            }
        }


        private void buildLocalComicDirComboBox()
        {
            try
            {
                string[] dirs = new string[0];
                cbRelateFolders.Items.Clear();
                foreach (string dir in dirs)
                {
                    cbRelateFolders.Items.Add(Path.GetFileName(dir));
                }
                cbRelateFolders.AutoCompleteCustomSource.Clear();
                cbRelateFolders.AutoCompleteCustomSource.AddRange(dirs);
            }
            catch (Exception ex)
            {
                NLogger.Error("目錄錯誤，原因：" + ex.ToString());
            }
        }

        private void cbFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            buildLocalComicTreeView();
        }

        #region 下載漫畫
        private void tvComicTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddDownloadTask(tvComicTree.SelectedNode);
        }

        private void tvComicTree_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                AddDownloadTask(tvComicTree.SelectedNode);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            AddDownloadTask(tvComicTree.SelectedNode);
        }

        private void AddDownloadTask(TreeNode tn)
        {
            if (false == NodeCheckUtil.IsChapterNode(tvComicTree.SelectedNode)) return;

            string taskname = tn.Parent.Text + "/" + tn.Text;
            string localPath = Path.Combine(Path.Combine(txtRootPath.Text, tn.Parent.Text), tn.Text);

            string downUrl = tn.Name;

            try
            {
                DownloadedList.AddDownloaded(xindm.GetComicRoot().WebSiteName, tn.Parent.Text, tn.Text);
            }
            catch { }

            TreeViewUtil.SetFontBold(tn.Parent);
            TreeViewUtil.SetFontBold(tn, Color.Blue);

            if ((queue.Count<Tasker>(q => q.name == taskname)) > 0)
            {
                NLogger.Info(taskname + " 已在下載排程，不重新加入！");
                statusMsg.Text = taskname + " 已在下載排程，不重新加入！";
                return;
            }

            if (true == File.Exists(localPath + ".rar"))
            {
                NLogger.Info(taskname + " 已壓縮封存，不加入排程下載！");
                statusMsg.Text = taskname + " 已壓縮封存，不加入排程下載！";
                return;
            }

            ComicChapter cc = tn.Tag as ComicChapter;
            List<string> downUrls = xindm.GetComicPages(cc).Select(p => p.Url).ToList(); ;
            if (true == chkUsingAlternativeUrl.Checked) // 確認是否使用替代網址下載圖片
            {
                downUrls.ForEach(p =>
                {
                    p = p.Replace(xindm.GetComicRoot().PicHost, xindm.GetComicRoot().PicHostAlternative);
                });
            }

            if (true == chkUsingAlternativeUrl.Checked)
                queue.Enqueue(new Tasker() { name = taskname, downloadPath = localPath, downloadUrls = downUrls });
            NLogger.Info(taskname + " 已加入下載排程！");
            statusMsg.Text = taskname + " 已加入下載排程！";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            statusMsg2.Text = "等待下載數=" + queue.Count.ToString() + "，正在下載數=" + (bgWorker.IsBusy ? "1" : "0");
            if (bgWorker.IsBusy) return;

            if (queue.Count <= 0) return;
            Tasker myTask = queue.Dequeue();
            bgWorker.RunWorkerAsync(myTask);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Tasker myTask = (Tasker)e.Argument;
            DownloadComic(myTask);
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerMsg msg = (WorkerMsg)e.UserState;
            if (false == String.IsNullOrEmpty(msg.statusMsg)) statusMsg.Text = msg.statusMsg;
            if (false == String.IsNullOrEmpty(msg.infoMsg)) NLogger.Info(msg.infoMsg);
        }

        private void DownloadComic(Tasker task)
        {
            if (false == Directory.Exists(task.downloadPath)) Directory.CreateDirectory(task.downloadPath);
            List<string> allPictureUrl = task.downloadUrls;

            // 因為 ManuelResetEvent 有 64 個上限限制，所以 thread Pool 手動寫
            //ThreadPool.SetMaxThreads(20, 40);
            //ManualResetEvent[] handles = new ManualResetEvent[allPictureUrl.Count];
            //for (int i = 0; i < allPictureUrl.Count; ++i)
            //{
            //    ThreadPool.QueueUserWorkItem(
            //        new WaitCallback(DownloadPicture),
            //        new Scheduler() { name = task.name, downloadPath = task.downloadPath, downloadUrl = allPictureUrl[i], handle = handles[i] }
            //        );
            //} // end of foreach

            //WaitHandle.WaitAll(handles);

            int threadCount = 40;
            int startPage = 0;
            int upperPage = (threadCount > allPictureUrl.Count ? allPictureUrl.Count : threadCount); // 一次下載40頁，如果剩不到40頁就下載剩下的頁數
            while (startPage < allPictureUrl.Count)
            {
                List<Thread> threadPool = new List<Thread>();
                for (int i = startPage; i < upperPage; ++i)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(DownloadPicture));
                    t.IsBackground = true;
                    t.Start(new DownloadPictureScheduler() { name = task.name, downloadPath = task.downloadPath, downloadUrl = allPictureUrl[i] });
                    threadPool.Add(t);
                }

                foreach (var t in threadPool)
                {
                    t.Join();
                }
                //workThreadPool.Clear();
                threadPool.Clear();
                startPage = upperPage;
                upperPage = (upperPage + threadCount > allPictureUrl.Count ? allPictureUrl.Count : upperPage + threadCount);
            }

            //statusMsg.Text = "[" + myTask.name + "]已經下載完成";
            bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = "[" + task.name + "]已經下載完成", infoMsg = "[" + task.name + "]已經下載完成" });
            Thread.Sleep(1);

        }

        private void DownloadPicture(Object scheduler)
        {
            string localPath = ((DownloadPictureScheduler)scheduler).downloadPath;
            string pictureUrl = ((DownloadPictureScheduler)scheduler).downloadUrl;

            // 只拿來顯示訊息用
            string tagname = "[" + ((DownloadPictureScheduler)scheduler).name + "]";
            string ThreadID = " Thread ID=[" + Thread.CurrentThread.GetHashCode().ToString() + "]";
            string fileName = Path.GetFileName(pictureUrl);

            bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = tagname + fileName + "下載中...", infoMsg = tagname + fileName + "下載中..." + ThreadID });
            Thread.Sleep(1);

            //Cursor = System.Windows.Forms.Cursors.WaitCursor;
            try
            {
                string tmpFile = Path.Combine(localPath, Path.GetFileName(pictureUrl)) + ".tmp";
                string cmpFile = Path.Combine(localPath, Path.GetFileName(pictureUrl)) + ".cmp";

                DonwloadUtil.donwload(pictureUrl, tmpFile);

                int i = 0;
                int testTimes = 50; // 檢查 50 次下載，如果還是都有問題，就跳出錯誤
                for (i = 0; i < testTimes; ++i)
                {
                    DonwloadUtil.donwload(pictureUrl, cmpFile);

                    if (FileUtil.CompareFileByMD5(tmpFile, cmpFile))
                        break;
                    else
                    {
                        bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = "", infoMsg = tagname + "檔案：" + fileName + "，比對失敗，重新下載比對！" + ThreadID });
                        Thread.Sleep(1);
                    }
                }

                if (i >= testTimes)
                {
                    throw new Exception("檔案：" + fileName + " 不完整(已重新下載了" + i.ToString() + "次)");
                }
                else
                {
                    FileUtil.MoveFile(tmpFile, Path.Combine(localPath, Path.GetFileName(pictureUrl)));
                }
            }
            catch (Exception ex)
            {
                bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = "", infoMsg = tagname + "下載失敗" + ThreadID + "，原因：" + ex.ToString() });
                Thread.Sleep(1);
            }
            finally
            {
                if (File.Exists(Path.Combine(localPath, Path.GetFileName(pictureUrl)) + ".cmp")) File.Delete(Path.Combine(localPath, Path.GetFileName(pictureUrl)) + ".cmp");

                bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = tagname + fileName + "下載完成...", infoMsg = tagname + fileName + "下載完成..." + ThreadID });
                Thread.Sleep(1);
            }
        }
        #endregion


        private void cbRelateFolders_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buildLocalComicTreeView();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ProxySetting.isUseProxy = chkIsUseProxy.Checked;
        }

        private void txtPath_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                buildLocalComicDirComboBox();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            buildLocalComicTreeView();
        }

        private void buildLocalComicTreeView()
        {
            try
            {
                string subPath = Path.Combine(txtRootPath.Text, cbRelateFolders.Text);
                if (false == Directory.Exists(subPath))
                {
                    lblCbMessage.Text = "路徑：" + subPath + " 不存在！";
                }
                else
                {
                    string[] dirs = Directory.GetDirectories(subPath);
                    string[] rars = Directory.GetFiles(subPath, "*.rar");
                    string[] newDirs = new string[dirs.Length + rars.Length];
                    Array.Copy(dirs, newDirs, dirs.Length);
                    Array.Copy(rars, 0, newDirs, dirs.Length, rars.Length);

                    newDirs = newDirs.ToList().CustomSort().ToArray();
                    Array.Reverse(newDirs);

                    TreeNode root = new TreeNode(cbRelateFolders.Text, 0, 0);
                    tvFolder.Nodes.Clear();
                    tvFolder.Nodes.Add(root);
                    foreach (string dir in newDirs)
                    {
                        if (0 == String.Compare(".RAR", Path.GetExtension(dir), true))
                            root.Nodes.Add(Path.GetFileName(dir), Path.GetFileName(dir), 1, 1); // rar icon
                        else
                            root.Nodes.Add(Path.GetFileName(dir), Path.GetFileName(dir), 2, 2); // 一般 icon
                    }
                    root.ExpandAll();

                    lblCbMessage.Text = "共" + newDirs.Count().ToString() + "筆資料！";
                }
            }
            catch (Exception ex)
            {
                NLogger.Error("無法開啟本地端漫畫," + ex.ToString());
            }

        }


        #region 尋找節點功能
        private void btnFind_Click(object sender, EventArgs e)
        {
            findNode(tvComicTree.Nodes, txtFind.Text.Trim());
        }
        private void txtFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                findNode(tvComicTree.Nodes, txtFind.Text.Trim());
            }
        }
        private bool findNode(TreeNodeCollection nodes, string findStr)
        {
            findStr = findStr.ToUpper();
            bool isStopFind = false;

            if (isStopFind) return true; // 確定找到後，依此flag決定要不要繼續找
            if (null == nodes) return false;

            foreach (TreeNode node in nodes)
            {
                if (isStopFind) return true; // 遞回時，已經在迴圈裡面的，需再判斷一次是不是已經找到node了

                // 遇到頁的節點，先點一下展開後再繼續找
                if (false == NodeCheckUtil.IsComicNameNode(node))
                {
                    if (0 >= node.Nodes.Count)
                    {
                        buildComicNodeForeground(node);
                        node.Collapse();
                    }
                    isStopFind = findNode(node.Nodes, findStr);
                }
                else
                {
                    if (node.Text.ToUpper().Contains(findStr) && false == isStopFind)
                    {
                        isStopFind = (MessageBox.Show(node.Parent.Text + "\\" + node.Text, "找到節點", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes);
                        if (isStopFind)
                        {
                            tvComicTree.SelectedNode = node;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region 全部收合按鈕
        private void btnCollapse_Click(object sender, EventArgs e)
        {
            tvComicTree.CollapseAll();
            tvComicTree.Nodes[0].Expand();
        }
        #endregion

        #region 打開本地漫畫
        private void tvFolder_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                OpenLocalComic();
        }

        private void tvFolder_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenLocalComic();
        }

        private void OpenLocalComic()
        {
            try
            {
                string myPath = Path.Combine(txtRootPath.Text, cbRelateFolders.Text, tvFolder.SelectedNode.Text);
                string arugment = String.Empty;
                if (0 == String.Compare(Path.GetExtension(myPath), ".RAR", true))
                {
                    arugment = " \"" + myPath + "\" ";
                }
                else
                {
                    string[] files = Directory.GetFiles(myPath);
                    Array.Sort(files);
                    arugment = " \"" + files[0] + "\" ";
                }
                Utils.CMDUtil.ExecuteCommandAsync(new CommandObj() { fileName = settings.PhotoProgramPath, arguments = arugment });
            }
            catch (Exception ex)
            {
                NLogger.Error("無法開啟本地漫畫檔案," + ex.ToString());
            }
        }
        #endregion

        #region 壓縮漫畫圖檔

        private void btnArchive_Click(object sender, EventArgs e)
        {
            ArchiveComic();
        }

        private void ArchiveComic()
        {
            RARHelper rar = null;
            try
            {
                rar = new RARHelper(settings.WinRARPath);
                rar.archiveDirectory(Path.Combine(txtRootPath.Text, cbRelateFolders.Text, tvFolder.SelectedNode.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                rar = null;
            }
        }
        #endregion

        #region 刪除本地漫畫
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = Path.Combine(txtRootPath.Text, cbRelateFolders.Text, tvFolder.SelectedNode.Text);
                if (System.Windows.Forms.DialogResult.Yes == MessageBox.Show("是否確定刪除" + myPath + "？", "刪除", MessageBoxButtons.YesNo))
                {
                    if (0 == String.Compare(Path.GetExtension(myPath), ".RAR", true))
                    {
                        File.Delete(myPath);
                    }
                    else
                    {
                        if (Directory.GetDirectories(myPath).Length <= 0)
                        {
                            Directory.Delete(myPath, true);
                        }
                        else
                        {
                            throw new Exception("目錄底下還有目錄");
                        }
                    }
                    MessageBox.Show("刪除完成！");
                    buildLocalComicTreeView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Settings Function
        private void chkUsingAlternativeUrl_CheckedChanged(object sender, EventArgs e)
        {
            chkIsUseProxy.Checked = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSettingsFromSettingTab();
                settings.save();
                GetSettings();
                MessageBox.Show("儲存完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show("儲存失敗：" + ex.ToString());
            }
        }

        private void ConfigureSettings()
        {
            settings = new Settings();
            try
            {
                settings = Settings.load();
            }
            catch (Exception ex)
            {
                NLogger.Error(MsgEnum.讀取設定失敗.ToString() + ex.ToString());
            }
            settings.save();
        }

        private void GetSettings()
        {
            /// 設定畫面
            setPhotoProgramPath.Text = settings.PhotoProgramPath;
            setWinRARPath.Text = settings.WinRARPath;
            setLocalPath.Text = settings.LocalPath;
            setLoadAllPicture.Checked = settings.LoadAllPicture;
            setUsingProxy.Checked = settings.UsingProxy;
            setProxyUrl.Text = settings.ProxyUrl;
            setProxyPort.Text = settings.ProxyPort.ToString();
            setBackGroundLoad.Checked = settings.BackGroundLoadNode;

            /// 使用畫面
            txtRootPath.Text = settings.LocalPath;
            chkLoadPhoto.Checked = settings.LoadAllPicture;
            chkIsUseProxy.Checked = settings.UsingProxy;
            chkBackGroundLoad.Checked = settings.BackGroundLoadNode;

            /// Proxy設定(要在使用畫面設定完成後)
            ProxySetting.isUseProxy = chkIsUseProxy.Checked;
            ProxySetting.ProxyUrl = settings.ProxyUrl;
            ProxySetting.ProxyPort = settings.ProxyPort;
        }

        private void LoadSettingsFromSettingTab()
        {
            settings.PhotoProgramPath = setPhotoProgramPath.Text.Trim();
            settings.WinRARPath = setWinRARPath.Text.Trim();
            settings.LocalPath = setLocalPath.Text.Trim();
            settings.LoadAllPicture = setLoadAllPicture.Checked;
            settings.UsingProxy = setUsingProxy.Checked;
            settings.ProxyUrl = setProxyUrl.Text.Trim();
            settings.BackGroundLoadNode = setBackGroundLoad.Checked;

            if (false == String.IsNullOrEmpty(setProxyPort.Text.Trim()))
                settings.ProxyPort = int.Parse(setProxyPort.Text.Trim());
        }

        //private void LoadDownloadList()
        //{
        //    dwnedList = new DownloadedList();
        //    try
        //    {
        //        dwnedList = DownloadedList.loadxml();
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogger.Error(MsgEnum.讀取已下載清單失敗.ToString() + ex.ToString());
        //    }
        //}
        #endregion

        #region Form元件顯示相關功能
        private void pbIcon_Paint(object sender, PaintEventArgs e)
        {
            if (null != (sender as PictureBox).Image && false == (sender as PictureBox).Image.Size.IsEmpty)
            {
                (sender as PictureBox).Visible = true;
            }
        }

        private void txtInfo_TextChanged(object sender, EventArgs e)
        {
            if (txtInfo.Lines.Length > 500)
            {
                txtInfo.SuspendLayout();
                txtInfo.Text = "";
                txtInfo.ResumeLayout();
            }
        }
        #endregion


        private void OpenDirectory_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(txtRootPath.Text, cbRelateFolders.Text));
        }

        private void RenewXML2DB()
        {
            //if (false == File.Exists(@".\ComicCatcher.s3db")
            //    && File.Exists(@".\donwloadedlist.bin")
            //    && File.Exists(@".\settings.xml"))
            if (File.Exists(@".\donwloadedlist.bin") && (false == File.Exists(@".\ComicCatcher.s3db")))
            {
                MessageBox.Show("按下確定後更新設定資料");
                try
                {
                    using (Form prompt = new Form())
                    {
                        prompt.Width = 300;
                        prompt.Height = 100;
                        Label textLabel = new Label() { Left = 50, Top = 20, Width = 300, Text = "資料更新中，請稍候..." };
                        prompt.Controls.Add(textLabel);
                        prompt.StartPosition = FormStartPosition.CenterScreen;
                        prompt.ControlBox = false;
                        prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                        prompt.Show();
                        Cursor = System.Windows.Forms.Cursors.WaitCursor;

                        DownloadedListOld.ImportToDB();
                        prompt.Hide();
                        MessageBox.Show("更新完成！");
                    }
                }
                finally
                {
                    Cursor = System.Windows.Forms.Cursors.Default;
                }
            }
        }

        #region BuildComicNode
        private void buildComicNode(TreeNode currNode)
        {
            try
            {
                if (chkBackGroundLoad.Checked)
                {
                    Thread t1 = new Thread(buildComicNodeBackground);
                    t1.IsBackground = true;
                    t1.Start(currNode);
                }
                else
                {
                    buildComicNodeForeground(currNode);
                }
            }
            catch (Exception ex)
            {
                NLogger.Error("buildComicNode," + ex.ToString());
            }
        }

        private void buildComicNodeForeground(TreeNode currNode)
        {
            try
            {
                // node.Name 就是 URL
                // 取得該頁下所有的漫畫清單
                if (NodeCheckUtil.IsListNode(currNode))
                {
                    buildComicNameNode(currNode);
                }
                else if (NodeCheckUtil.IsComicNameNode(currNode))
                {
                    buildComicChapterNode(currNode);
                }
            }
            catch (Exception ex)
            {
                NLogger.Error("buildComicNodeForeground," + ex.ToString());
                throw ex;
            }
        }

        private void buildComicNodeBackground(object objNode)
        {
            try
            {
                TreeNode currNode = objNode as TreeNode;
                if (NodeCheckUtil.IsListNode(currNode))
                {
                    buildComicNameNode(currNode);
                    foreach (TreeNode node in currNode.Nodes)
                    {
                        Thread t2 = new Thread(buildComicChapterNode);
                        t2.IsBackground = true;
                        t2.Start(node);
                    }
                }
                else if (NodeCheckUtil.IsComicNameNode(currNode))
                {
                    #region Lock的另一種作法
                    //bool isNotLocked = false;
                    //try
                    //{
                    //    isNotLocked = Monitor.TryEnter(currNode);
                    //}
                    //finally
                    //{
                    //    if (isNotLocked)
                    //    {
                    //        Monitor.Exit(currNode);
                    //    }
                    //}
                    //if (false == isNotLocked) return; 
                    #endregion
                    Thread t2 = new Thread(buildComicChapterNode);
                    t2.IsBackground = true;
                    t2.Start(currNode);
                }
            }
            catch (Exception ex)
            {
                NLogger.Error("buildComicNodeForeground," + ex.ToString());
                throw ex;
            }
        }

        private void buildComicNameNode(object listNode)
        {
            TreeNode currNode = listNode as TreeNode;
            if (null == currNode) return;
            if (false == NodeCheckUtil.IsListNode(currNode)) return;

            try
            {
                // 產生清單下的漫畫名稱內容子節點
                //NLogger.Info("********************************************");
                TreeViewUtil.ClearTreeNode(currNode);
                ComicGroup cg = currNode.Tag as ComicGroup;
                foreach (var comic in xindm.GetComicNames(cg))
                {
                    if (chkLoadPhoto.Checked)
                    {
                        Image img = comic.IconImage; // 此動作已內建 multithread 了
                    }

                    TreeNode nameNode = TreeViewUtil.BuildNode(comic, txtRootPath.Text);
                    lock (currNode.TreeView)
                    {
                        TreeViewUtil.AddTreeNode(currNode, nameNode);
                    }
                    //NLogger.Info(comic.Caption + "=" + comic.Url);
                }
                //NLogger.Info("********************************************");
            }
            catch (Exception ex)
            {
                NLogger.Error("產生漫畫名稱節點錯誤:" + currNode.Name);
                NLogger.Error(ex.ToString());
                TreeViewUtil.SetFontBold(currNode, Color.Red);
            }
        }

        public void buildComicChapterNode(object nameNode)
        {
            TreeNode currNode = nameNode as TreeNode;
            if (null == currNode) return;
            if (false == NodeCheckUtil.IsComicNameNode(currNode)) return;

            try
            {
                // 產生漫畫的回合子節點
                //NLogger.Info("********************************************");
                TreeViewUtil.ClearTreeNode(currNode); ;
                ComicName cn = currNode.Tag as ComicName;
                foreach (var chapter in xindm.GetComicChapters(cn))
                {
                    TreeNode chapterNode = TreeViewUtil.BuildNode(chapter, xindm.GetComicRoot().WebSiteName, currNode.Text);
                    lock (currNode)
                    {
                        TreeViewUtil.AddTreeNode(currNode, chapterNode);
                    }
                    //NLogger.Info(comic.Caption + "=" + comic.Url);
                }
                //NLogger.Info("********************************************");
            }
            catch (Exception ex)
            {
                NLogger.Error("產生漫畫回數節點錯誤:" + currNode.Name);
                NLogger.Error(ex.ToString());
                TreeViewUtil.SetFontBold(currNode, Color.Red);
            }
        }
        #endregion
    }
}

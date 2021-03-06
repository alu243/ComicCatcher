﻿using System;
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
using System.Threading.Tasks;

namespace ComicCatcher
{
    public partial class frmMain : Form
    {
        private static readonly Queue<Tasker> queue = new Queue<Tasker>();

        private Settings settings = null;

        private IComicCatcher comicSiteCatcher = null;

        private PathGroup pathGroup = new PathGroup();
        private IgnoreComic ignoreComic = new IgnoreComic();

        //private DownloadedList dwnedList = null;

        private List<Thread> threadPool = new List<Thread>();
        private Dictionary<string, bool> lockNodes = new Dictionary<string, bool>();
        private Dictionary<string, List<TreeNode>> comicNodes = new Dictionary<string, List<TreeNode>>();

        public frmMain()
        {
            try
            {
                ExportInteropFile();
                InitializeComponent();
                NLogger.SetBox(this.txtInfo);
            }
            catch (Exception ex)
            { MessageBox.Show("錯誤發生" + ex.ToString()); }
        }

        private void ExportInteropFile()
        {
            if (false == File.Exists(@".\System.Data.SQLite.dll"))
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
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //xindm.GetComicPages(new ComicChapter());
            lblCbMessage.Text = "";
            lblUpdateDate.Text = "";
            lblUpdateChapter.Text = "";
            buildLocalComicDirComboBox();


            ConfigureSettings();
            GetSettingsToView();

            RenewXML2DB();
            DownloadedList.LoadDB();
            //LoadDownloadList();


            pathGroup.Load();
            ignoreComic.Load();

            cbComicCatcher.SelectedIndex = 0;
            //ChangeComicSite(new Xindm());
        }

        private void ChangeComicSite(IComicCatcher newCatcher)
        {
            comicSiteCatcher = newCatcher;
            InitialComicRootTree();
        }

        private void InitialComicRootTree()
        {
            TreeViewUtil.ClearNodes(tvComicTree);
            GC.Collect();
            ComicWebRoot cr = comicSiteCatcher.GetComicWebRoot();
            TreeNode root = new TreeNode(cr.WebSiteTitle);
            tvComicTree.Nodes.Add(root);
            root.Tag = cr;

            var groups = comicSiteCatcher.GetComicWebPages();
            groups.ForEach(g =>
            {
                TreeNode tn = root.Nodes.Add(g.Url, g.Caption);
                tn.Tag = g;
                tn.ImageIndex = 3;
                tn.SelectedImageIndex = 3;
            });
            tvComicTree.ExpandAll();
            for (int i = 0; i < 5; i++)
            {
                tvComicTree.SelectedNode = tvComicTree.Nodes[0].Nodes[i];
            }
            tvComicTree.SelectedNode = tvComicTree.Nodes[0];
        }

        private void tvComicTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Cursor = System.Windows.Forms.Cursors.WaitCursor;
            if (null == tvComicTree.SelectedNode) return;
            try
            {

                #region IsComicNameNode
                if (NodeCheckUtil.IsComicNameNode(tvComicTree.SelectedNode))
                {
                    // 如果是漫畫名稱節點，就顯示icon圖片
                    lock (pbIcon)
                    {
                        pbIcon.Image = null;
                    }
                    ComicNameInWebPage comicName = tvComicTree.SelectedNode.Tag as ComicNameInWebPage;
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
                        string pathGroupName = pathGroup.GetGroupName(tvComicTree.SelectedNode.Text);

                        string today = DateTime.Today.ToString("MM月dd号");
                        string yestoday = DateTime.Today.AddDays(-1).ToString("MM月dd号");
                        string beforeYestoday = DateTime.Today.AddDays(-2).ToString("MM月dd号");

                        txtUrl.Text = tvComicTree.SelectedNode.Name;
                        lblUpdateDate.Text = comicName.LastUpdateDate.Replace("今天", today).Replace("昨天", yestoday).Replace("前天", beforeYestoday);
                        if (lblUpdateDate.Text.Contains("分钟前更新"))
                        {
                            lblUpdateDate.Text = today + lblUpdateDate.Text;
                        }
                        lblUpdateChapter.Text = comicName.LastUpdateChapter;
                        if (false == cbRelateFolders.Items.Contains(pathGroupName)) cbRelateFolders.Items.Add(pathGroupName);

                        cbRelateFolders.Text = pathGroupName;

                        // 章節在另一個 treeview 顯示
                        string fullPath = tvComicTree.SelectedNode.FullPath;
                        TreeNode nd = new TreeNode("站");
                        nd.Nodes.Add("頁");
                        nd.Nodes[0].Nodes.Add(tvComicTree.SelectedNode.Text);
                        if (comicNodes.ContainsKey(fullPath))
                        {
                            nd.Nodes[0].Nodes[0].Nodes.AddRange(comicNodes[fullPath].ToArray());
                        }
                        treeViewnComicName.Nodes.Clear();
                        treeViewnComicName.BeginUpdate();
                        treeViewnComicName.Nodes.Add(nd);
                        treeViewnComicName.ExpandAll();
                        treeViewnComicName.EndUpdate();

                    }
                    catch (Exception ex)
                    {
                        NLogger.Error("顯示資料失敗," + ex.ToString());
                        pbIcon.Image = null;
                    }
                }
                #endregion

                //清單如果沒有子節點，就產生子節點
                if (NodeCheckUtil.IsListNode(tvComicTree.SelectedNode))
                {
                    if (tvComicTree.SelectedNode.Nodes.Count <= 0)
                    {
                        buildComicNode(tvComicTree.SelectedNode);
                        tvComicTree.SelectedNode.Expand();
                    }
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
            AddDownloadTask((sender as TreeView).SelectedNode);
        }

        private void tvComicTree_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                AddDownloadTask((sender as TreeView).SelectedNode);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            //AddDownloadTask(tvComicTree.SelectedNode);
        }

        private void AddDownloadTask(TreeNode tn)
        {
            if (false == NodeCheckUtil.IsChapterNode(tn)) return;

            string taskname = tn.Parent.Text + "/" + tn.Text;
            string localPath = Path.Combine(Path.Combine(txtRootPath.Text, pathGroup.GetGroupName(tn.Parent.Text)), tn.Text);

            string downUrl = tn.Name;

            try
            {
                DownloadedList.AddDownloaded(comicSiteCatcher.GetComicWebRoot().WebSiteName, tn.Parent.Text, tn.Text);
            }
            catch { }

            string fullPath1 = tn.Parent.FullPath;
            string fullPath2 = tvComicTree.SelectedNode.FullPath;
            TreeViewUtil.SetFontBold(tvComicTree.SelectedNode);
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

            ComicChapterInName cc = tn.Tag as ComicChapterInName;
            //List<string> downUrls = xindm.GetComicPages(cc).Select(p => p.Url).ToList(); ;
            //List<string> downFileNames = xindm.GetComicPages(cc).Select(p => p.PageFileName).ToList(); ;
            //if (true == chkUsingAlternativeUrl.Checked) // 使用替代網址下載圖片
            //{
            //    downUrls.ForEach(p =>
            //    {
            //        p = p.Replace(xindm.GetComicRoot().PicHost, xindm.GetComicRoot().PicHostAlternative);
            //    });
            //}

            //queue.Enqueue(new Tasker() { name = taskname, downloadPath = localPath, downloadUrls = downUrls, downloadFileNames = downFileNames});
            queue.Enqueue(new Tasker() { name = taskname, downloadPath = localPath, downloadChapter = cc, downloader = comicSiteCatcher });

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
            if (chkSaveWebSiteName.Checked)
            {
                string parentPath = Path.GetDirectoryName(task.downloadPath);
                string currentDir = task.downloadPath.Replace(parentPath, String.Empty).Trim('\\');
                task.downloadPath = parentPath + "\\" + task.downloader.GetComicWebRoot().WebSiteName + "_" + currentDir;
            }
            if (false == Directory.Exists(task.downloadPath)) Directory.CreateDirectory(task.downloadPath);

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
            bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = "[" + task.name + "]準備開始下載", infoMsg = "[" + task.name + "]準備開始下載" });
            var pages = task.downloader.GetComicPages(task.downloadChapter);
            bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = "[" + task.name + "]準備開始下載", infoMsg = "[" + task.name + "]，共" + pages.Count + "頁" });


            int threadCount = comicSiteCatcher.GetComicWebRoot().ThreadCount;
            int startPage = 0;
            int upperPage = (threadCount > pages.Count ? pages.Count : threadCount); // 一次下載設定的頁數，如果剩不到40頁就下載剩下的頁數
            while (startPage < pages.Count)
            {
                List<Thread> comicThreadPool = new List<Thread>();
                for (int i = startPage; i < upperPage; ++i)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(DownloadPicture));
                    t.IsBackground = true;
                    t.Start(new DownloadPictureScheduler() { name = task.name, downloadPath = task.downloadPath, downloadUrl = pages[i].Url, downloadFileName = pages[i].PageFileName, reffer = pages[i].Reffer ?? "" });
                    comicThreadPool.Add(t);
                }

                foreach (var t in comicThreadPool)
                {
                    t.Join();
                }
                comicThreadPool.Clear();
                GC.Collect();
                startPage = upperPage;
                upperPage = (upperPage + threadCount > pages.Count ? pages.Count : upperPage + threadCount);
            }

            //statusMsg.Text = "[" + myTask.name + "]已經下載完成";
            bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = "[" + task.name + "]已經下載完成", infoMsg = "[" + task.name + "]已經下載完成" });
            if (chkArchiveDownloadedFile.Checked)
            {
                RARHelper rar = null;
                try
                {
                    rar = new RARHelper(settings.WinRARPath);
                    rar.archiveDirectory(task.downloadPath);
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
            Thread.Sleep(1);

        }

        private void DownloadPicture(Object scheduler)
        {
            string localPath = ((DownloadPictureScheduler)scheduler).downloadPath;
            string pictureUrl = ((DownloadPictureScheduler)scheduler).downloadUrl;
            string pictureName = ((DownloadPictureScheduler)scheduler).downloadFileName;
            string reffer = ((DownloadPictureScheduler)scheduler).reffer;
            // 只拿來顯示訊息用
            string tagname = "[" + ((DownloadPictureScheduler)scheduler).name + "]";
            string ThreadID = " Thread ID=[" + Thread.CurrentThread.GetHashCode().ToString() + "]";

            string tmpFile = Path.Combine(localPath, pictureName) + ".tmp";
            string cmpFile = Path.Combine(localPath, pictureName) + ".cmp";

            //bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = tagname + pictureName + "下載中...", infoMsg = tagname + pictureName + "下載中..." + "[" + pictureUrl + "]" + ThreadID });
            NLogger.Info(tagname + pictureName + "下載中..." + "[" + pictureUrl + "]" + ThreadID);

            //Cursor = System.Windows.Forms.Cursors.WaitCursor;
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                DonwloadUtil.donwload(pictureUrl, reffer, Path.Combine(localPath, pictureName));
                sw.Stop();
            }
            catch (Exception ex)
            {
                sw.Stop();
                bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = "", infoMsg = tagname + "下載失敗" + ThreadID + "，原因：" + ex.ToString() + string.Format("({0}ms)", sw.ElapsedMilliseconds) });
            }
            finally
            {
                bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = tagname + pictureName + "下載完成...", infoMsg = tagname + pictureName + "下載完成..." + ThreadID + string.Format("({0}ms)", sw.ElapsedMilliseconds) });
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
                    lblCbMessage.Text = "路徑：" + cbRelateFolders.Text + " 不存在！";
                    tvFolder.Nodes.Clear();
                    TreeNode root = new TreeNode(cbRelateFolders.Text, 0, 0);
                    tvFolder.Nodes.Add(root);
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
                        //buildComicNodeForeground(node);
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

        private void btnBatchArchive_Click(object sender, EventArgs e)
        {

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
                string myPath = "";
                if (tvFolder.SelectedNode.Level == 0)
                {
                    myPath = Path.Combine(txtRootPath.Text, cbRelateFolders.Text);
                }
                else
                {
                    myPath = Path.Combine(txtRootPath.Text, cbRelateFolders.Text, tvFolder.SelectedNode.Text);
                }
                if (DialogResult.Yes == MessageBox.Show("是否確定刪除" + myPath + "？", "刪除", MessageBoxButtons.YesNo))
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
                        else if (tvFolder.SelectedNode.Level == 0)
                        {
                            Directory.Delete(myPath, true);
                        }
                        else
                        {
                            throw new Exception("目錄底下還有目錄");
                        }
                    }
                    if (tvFolder.SelectedNode.Level == 0)
                    {
                        SQLiteHelper.VACCUM();
                        if (cbRelateFolders.Text == tvComicTree.SelectedNode.Text)
                        {
                            TreeViewUtil.SetFontRegular(tvComicTree.SelectedNode);
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
        private void chkArchiveDownloadedFile_CheckedChanged(object sender, EventArgs e)
        {
            //chkIsUseProxy.Checked = false;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSettingsFromSettingTab();
                settings.save();
                GetSettingsToView();
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

        private void GetSettingsToView()
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
            setSaveWebSiteName.Checked = settings.SaveWebSiteName;
            setArchiveDownloadedFile.Checked = settings.ArchiveDownloadedFile;

            /// 使用畫面
            txtRootPath.Text = settings.LocalPath;
            chkSaveWebSiteName.Checked = settings.SaveWebSiteName;
            chkLoadPhoto.Checked = settings.LoadAllPicture;
            chkIsUseProxy.Checked = settings.UsingProxy;
            chkArchiveDownloadedFile.Checked = settings.ArchiveDownloadedFile;
            //chkBackGroundLoad.Checked = settings.BackGroundLoadNode;

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
            settings.SaveWebSiteName = setSaveWebSiteName.Checked;
            settings.ArchiveDownloadedFile = setArchiveDownloadedFile.Checked;

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
                var t = Task.Factory.StartNew((node) => { buildComicNodeBackground(node); }, currNode);
            }
            catch (Exception ex)
            {
                NLogger.Error("buildComicNode," + ex.ToString());
            }
        }

        //private void buildComicNodeForeground(TreeNode currNode)
        //{
        //    try
        //    {
        //        // node.Name 就是 URL
        //        // 取得該頁下所有的漫畫清單
        //        if (NodeCheckUtil.IsListNode(currNode))
        //        {
        //            buildComicNameNode(currNode);
        //        }
        //        else if (NodeCheckUtil.IsComicNameNode(currNode))
        //        {
        //            buildComicChapterNode(currNode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogger.Error("buildComicNodeForeground," + ex.ToString());
        //        throw ex;
        //    }
        //}

        private void buildComicNodeBackground(object objNode)
        {
            TreeNode currNode = objNode as TreeNode;
            if (currNode == null) return;
            try
            {
                if (NodeCheckUtil.IsListNode(currNode))
                {
                    buildComicNameNode(currNode);
                    var nodeList = currNode.Nodes.Cast<TreeNode>().ToList();
                    Parallel.ForEach(nodeList, node =>
                    {
                        buildComicChapterNode(node);
                    });
                }
                else if (NodeCheckUtil.IsComicNameNode(currNode))
                {
                    buildComicChapterNode(currNode);
                }
            }
            catch (Exception ex)
            {
                NLogger.Error("buildComicNodeBackground," + ex.ToString());
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
                if (currNode != null)
                {
                    lock (comicNodes)
                    {
                        if (lockNodes.ContainsKey(currNode.FullPath))
                        {
                            this.comicNodes[currNode.FullPath] = new List<TreeNode>();
                        }
                        else
                        {
                            this.comicNodes.Add(currNode.FullPath, new List<TreeNode>());
                        }
                    }

                    if (lockNodes.ContainsKey(currNode.FullPath)) return;

                    lock (lockNodes)
                    {
                        lockNodes.Add(currNode.FullPath, true);
                    }
                }

                // 產生清單下的漫畫名稱內容子節點
                //NLogger.Info("********************************************");
                TreeViewUtil.ClearTreeNode(currNode);
                GC.Collect();
                ComicWebPage cg = currNode.Tag as ComicWebPage;
                var names = new List<TreeNode>();
                foreach (var comic in comicSiteCatcher.GetComicNames(cg))
                {
                    if (ignoreComic.ContainsKey(comic.Url))
                    {
                        continue;
                    }

                    if (chkLoadPhoto.Checked)
                    {
                        Image img = comic.IconImage; // 此動作已內建 multithread 了
                    }

                    //TreeNode nameNode = TreeViewUtil.BuildNode(comic, txtRootPath.Text);
                    string groupName = pathGroup.GetGroupName(comic.Caption);
                    TreeNode nameNode = TreeViewUtil.BuildNode(comic, txtRootPath.Text, groupName);
                    names.Add(nameNode);
                    //NLogger.Info(comic.Caption + "=" + comic.Url);
                }
                TreeViewUtil.AddTreeNodes(currNode, names);

                //NLogger.Info("********************************************");
            }
            catch (Exception ex)
            {
                NLogger.Error("產生漫畫名稱節點錯誤:" + currNode.Name);
                NLogger.Error(ex.ToString());
                TreeViewUtil.SetFontBold(currNode, Color.Red);
            }
            finally
            {
                if (currNode != null)
                {
                    lock (currNode)
                    {
                        if (lockNodes.ContainsKey(currNode.FullPath))
                        {
                            lock (lockNodes)
                            {
                                lockNodes.Remove(currNode.FullPath);
                            }
                        }
                    }
                }
            }
        }

        public void buildComicChapterNode(object nameNode)
        {
            TreeNode currNode = nameNode as TreeNode;
            if (null == currNode) return;
            if (false == NodeCheckUtil.IsComicNameNode(currNode)) return;

            try
            {
                string fullPath = currNode.FullPath;
                // 產生漫畫的回合子節點
                //NLogger.Info("********************************************");
                TreeViewUtil.ClearTreeNode(currNode);
                GC.Collect();
                ComicNameInWebPage cn = currNode.Tag as ComicNameInWebPage;
                var chapters = new List<TreeNode>();
                foreach (var chapter in comicSiteCatcher.GetComicChapters(cn))
                {
                    TreeNode chapterNode = TreeViewUtil.BuildNode(chapter, comicSiteCatcher.GetComicWebRoot().WebSiteName, currNode.Text);
                    //chapterNode.ImageIndex = 5;
                    //chapterNode.SelectedImageIndex = 5;
                    chapters.Add(chapterNode);
                    //NLogger.Info(comic.Caption + "=" + comic.Url);
                }
                comicNodes[fullPath] = chapters;
                //TreeViewUtil.AddTreeNodes(currNode, chapters);
                //currNode.ImageIndex = 5;
                //currNode.SelectedImageIndex = 5;

                //NLogger.Info("********************************************");
            }
            catch (Exception ex)
            {
                NLogger.Error("產生漫畫回數節點錯誤:" + currNode.Name);
                NLogger.Error(ex.ToString());
                TreeViewUtil.SetFontBold(currNode, Color.Red);
            }
            finally
            {

            }
        }
        #endregion

        private void cbComicCatcher_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbComicCatcher.Text.ToUpper())
            {
                case "XINDM":
                    chkIsUseProxy.Checked = true;
                    chkIsUseProxy.Enabled = true;
                    ChangeComicSite(new Xindm());
                    break;
                case "DM5":
                    chkIsUseProxy.Checked = false;
                    chkIsUseProxy.Enabled = false;
                    ChangeComicSite(new Dm5());
                    break;
                case "SEEMH":
                    chkIsUseProxy.Checked = false;
                    chkIsUseProxy.Enabled = false;
                    ChangeComicSite(new Dm5());
                    break;
            }
        }

        private void btnShowEditModal_Click(object sender, EventArgs e)
        {
            frmEditPathGroup f = new frmEditPathGroup(SettingEnum.PathGroup);
            f.ShowDialog(this);
            this.pathGroup.Load();
        }
        private void btnShowExceptModal_Click(object sender, EventArgs e)
        {
            frmEditPathGroup f = new frmEditPathGroup(SettingEnum.IgnoreComic);
            f.ShowDialog(this);
            this.ignoreComic.Load();
        }

        private void btnAppendTo_Click(object sender, EventArgs e)
        {
            try
            {

                if (tvComicTree.Nodes != null && tvComicTree.Nodes.Count <= 1)
                {
                    tvComicTree.Nodes.Insert(0, "自行添加");
                    tvComicTree.Nodes[0].Nodes.Insert(0, "第1頁");
                }

                var urls = tvComicTree.Nodes[0].Nodes[0].Nodes.Cast<TreeNode>().ToList().Select(p => p.Name).ToList();
                if (false == urls.Any(p => p == txtUrl.Text))
                {
                    var nameNode = BuildComicNameNode(txtUrl.Text);
                    tvComicTree.Nodes[0].Nodes[0].Nodes.Add(nameNode);
                    tvComicTree.Nodes[0].ExpandAll();
                    buildComicChapterNode(nameNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("不是有效的url:" + ex.Message);
            }
        }

        private TreeNode BuildComicNameNode(string url)
        {
            //Caption = "A", IconImage = null, IconUrl = "", LastUpdateChapter = "", LastUpdateDate = "", 
            string htmlContent = ComicUtil.GetUtf8Content(url);
            Dm5 dm5 = new Dm5();
            ComicNameInWebPage comic = dm5.GetComicName(url);
            string groupName = pathGroup.GetGroupName(comic.Caption);
            TreeNode nameNode = TreeViewUtil.BuildNode(comic, txtRootPath.Text, groupName);
            return nameNode;


            //// 圣诞迷城"; 
            //Regex rc = new Regex(@"DM5_COMIC_MNAME="".*?""", RegexOptions.Compiled);
            //string caption = rc.Match(htmlContent).Value;
            //if (false == string.IsNullOrEmpty(caption))
            //{
            //    caption = caption.Replace(@"DM5_COMIC_MNAME=""", "").Replace(@"""", "");
            //}
            //Regex iconOut = new Regex(@"class=""innr91"".*?</div>", RegexOptions.Compiled);
            //Regex iconInner = new Regex(@"src="".*?""", RegexOptions.Compiled);
            //string iconurl = iconOut.Match(htmlContent).Value;
            //if (false == string.IsNullOrEmpty(iconurl))
            //{
            //    iconurl = iconInner.Match(iconurl).Value;
            //    if (false == string.IsNullOrEmpty(iconurl))
            //    {
            //        iconurl = iconurl.Replace(@"src=""", "").Replace(@"""", "");
            //    }
            //}
            //Regex rLastDate = new Regex(@"更新时间：.*?<", RegexOptions.Compiled);
            //string lastUpdateDate = rLastDate.Match(htmlContent).Value;
            //if (false == String.IsNullOrEmpty(lastUpdateDate))
            //{
            //    lastUpdateDate = lastUpdateDate.Replace(@"更新时间：", "").Replace(@"<", "");
            //}
            //Regex rLastChapter = new Regex(@"最新章节：.*?<", RegexOptions.Compiled);
            //string lastUpdateChapter = rLastChapter.Match(htmlContent).Value;
            //if (false == String.IsNullOrEmpty(lastUpdateDate))
            //{
            //    lastUpdateChapter = lastUpdateChapter.Replace(@"最新章节：", "").Replace(@"<", "");
            //}

            //ComicNameInWebPage comic = new ComicNameInWebPage()
            //{
            //    IconUrl = iconurl, // 取得漫畫首頁圖像連結
            //    LastUpdateDate = lastUpdateDate, // 取得最近更新日期
            //    LastUpdateChapter = lastUpdateChapter, // 取得最近更新回數
            //    Url = url,
            //    Caption = CharsetConvertUtil.ToTraditional(caption)
            //};
        }

        private void AddIgnoreComic_Click(object sender, EventArgs e)
        {
            try
            {
                ignoreComic.AddIgnoreComic(tvComicTree.SelectedNode.Name, tvComicTree.SelectedNode.Text);
                if (lockNodes.ContainsKey(tvComicTree.SelectedNode.FullPath))
                {
                    lockNodes.Remove(tvComicTree.SelectedNode.FullPath);
                }
                TreeViewUtil.ClearTreeNode(tvComicTree.SelectedNode);
                tvComicTree.Nodes.Remove(tvComicTree.SelectedNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void tvComicTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                //NodeCheckUtil.IsComicNameNode(tvComicTree.SelectedNode)
                if (NodeCheckUtil.IsComicNameNode(e.Node) && e.Node == tvComicTree.SelectedNode)
                {
                    Point p = MousePosition;//取得滑鼠位置
                    exceptMenu.Show(p);//顯示右鍵選單
                }
            }
        }

        private readonly Size CHECK_BOX_SIZE = new Size(13, 13);
        private readonly Size IMAGE_SIZE = new Size(16, 16);
        private void tvComicTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            var font = e.Node.NodeFont == null ? (sender as TreeView).Font : e.Node.NodeFont;
            if (e.Node.ImageIndex == 99) // if there is no image
            {
                //int imagewidths = 16;
                //int textheight = 16;
                //int x = e.Node.Bounds.Left - 3 - imagewidths / 2;
                //int y = (e.Bounds.Top + e.Bounds.Bottom) / 2 + 1;
                //Point point = new Point(x - imagewidths / 2, y - textheight / 2); // the new location for the text to be drawn


                Rectangle textRect = e.Node.Bounds;
                textRect.Offset(new Point(-18));
                textRect.Offset(1, 1);
                textRect.Width -= 2;
                textRect.Height -= 2;


                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, textRect, e.Node.ForeColor);
                e.DrawDefault = false;
            }
            else
                e.DrawDefault = true;
            // drawn at the default location
            //TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, e.Node.ForeColor);






            //            //Get the backcolor and forecolor
            //            Color backColor, foreColor;

            //if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected)
            //{
            //    backColor = SystemColors.Highlight;
            //    foreColor = SystemColors.HighlightText;
            //}

            //else if ((e.State & TreeNodeStates.Hot) == TreeNodeStates.Hot)
            //{
            //    backColor = SystemColors.HotTrack;
            //    foreColor = SystemColors.HighlightText;
            //}
            //else
            //{
            //    backColor = e.Node.BackColor;
            //    foreColor = e.Node.ForeColor;
            //}

            ////Calculate the text rectangle.
            //Rectangle textRect = e.Node.Bounds;
            ////textRect.Offset(new Point(-IMAGE_SIZE.Width - 3));
            //textRect.Offset(new Point(-18));
            //textRect.Offset(1, 1);
            //textRect.Width -= 2;
            //textRect.Height -= 2;

            ////The first level nodes has images but no checkboxes.
            ////The second level nodes has checkboxes but no images.
            ////The other level nodes is drawn by default.
            //if (e.Node.Level >= 2 && e.Node.ImageIndex == 99)
            //{
            //    try
            //    {
            //        //Draw the background.
            //        //using (SolidBrush brush = new SolidBrush(backColor))
            //        //{
            //        //    e.Graphics.FillRectangle(brush, textRect);
            //        //}

            //        //Draw the image.
            //        //if (e.Node.TreeView.ImageList != null && e.Node.ImageIndex >= 0
            //        //    && e.Node.ImageIndex < e.Node.TreeView.ImageList.Images.Count)
            //        //{
            //        //    Image img = this.tvComicTree.ImageList.Images[e.Node.ImageIndex];
            //        //    if (img != null)
            //        //        e.Graphics.DrawImage(img, new Point(textRect.X - img.Width - 1, textRect.Y + 1));
            //        //}

            //        //Draw the text.
            //        TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.NodeFont, textRect, foreColor, backColor);
            //        //Draw the focused rectangle.
            //        if ((e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused)
            //        {
            //            ControlPaint.DrawFocusRectangle(e.Graphics, textRect, foreColor, backColor);
            //        }
            //    }
            //    catch (Exception exp)
            //    {
            //        Console.WriteLine(exp.ToString());
            //    }

            //    e.DrawDefault = false;
            //}
            //else
            //{
            //    e.DrawDefault = true;
            //}
        }
    }
}

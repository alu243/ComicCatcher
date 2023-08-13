using ComicCatcher.App_Code.ComicModels;
using ComicCatcher.App_Code.ComicModels.Domains;
using ComicCatcher.App_Code.DbModel;
using ComicCatcher.App_Code.Models;
using ComicCatcher.ComicModels;
using ComicCatcher.ComicModels.Domains;
using ComicCatcher.DbModel;
using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace ComicCatcher
{
    public partial class frmMain : Form
    {
        private static readonly Queue<DownloadChapterTask> queue = new();

        private Settings settings = null;

        private IComicCatcher comicSiteCatcher = null;

        private PathGroupDic pathGroupDic = null;
        private IgnoreComicDic ignoreComicDic = null;

        public frmMain()
        {
            try
            {
                //ExportInteropFile();
                InitializeComponent();
                NLogger.SetBox(this.txtInfo);
            }
            catch (Exception ex)
            { MessageBox.Show("錯誤發生" + ex.ToString()); }
        }

        //private void ExportInteropFile()
        //{
        //    if (false == File.Exists(@".\System.Data.SQLite.dll"))
        //    {
        //        Assembly asm;
        //        Stream asmfs;
        //        asm = Assembly.GetExecutingAssembly();
        //        asmfs = asm.GetManifestResourceStream("ComicCatcher.x86.System.Data.SQLite.dll");
        //        //var files = asm.GetManifestResourceNames();

        //        using (FileStream fs = new FileStream(@".\System.Data.SQLite.dll", FileMode.Create, FileAccess.Write))
        //        {
        //            asmfs.Position = 0;
        //            int length = 4096;
        //            byte[] buffer = new Byte[length];
        //            int count = 0;
        //            while (0 < (count = asmfs.Read(buffer, 0, length)))
        //            {
        //                fs.Write(buffer, 0, count);
        //            }
        //        }
        //    }
        //}

        private void frmMain_Load(object sender, EventArgs e)
        {
            RenewSettings2Db();
            settings = Settings.Load();

            DownloadedList.Load();
            pathGroupDic = PathGroupDic.Load();
            ignoreComicDic = IgnoreComicDic.Load();

            lblCbMessage.Text = "";
            lblUpdateDate.Text = "";
            lblUpdateChapter.Text = "";
            BuildLocalComicDirComboBox();
            BindSettingsToView();

            cbComicCatcher.SelectedIndex = 0;
            //ChangeComicSite(new Xindm());
        }

        private void RenewSettings2Db()
        {
            if (File.Exists(@".\settings.xml"))
            {
                try
                {
                    var oldSettings = SettingsOld.load();
                    var newSettings = new Settings()
                    {
                        ArchiveDownloadedFile = oldSettings.ArchiveDownloadedFile,
                        BackGroundLoadNode = oldSettings.BackGroundLoadNode,
                        LoadAllPicture = oldSettings.LoadAllPicture,
                        LocalPath = oldSettings.LocalPath,
                        PhotoProgramPath = oldSettings.PhotoProgramPath,
                        ProxyPort = oldSettings.ProxyPort,
                        ProxyUrl = oldSettings.ProxyUrl,
                        SaveWebSiteName = oldSettings.SaveWebSiteName,
                        UsingProxy = oldSettings.UsingProxy,
                        WinRARPath = oldSettings.WinRARPath
                    };
                    newSettings.Save();
                    File.Delete(@".\settings.xml");
                }
                finally
                {
                    Cursor = System.Windows.Forms.Cursors.Default;
                }
            }
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
            ComicRoot cr = comicSiteCatcher.GetRoot();
            TreeNode root = new TreeNode(cr.Caption);
            tvComicTree.Nodes.Add(root);
            root.Tag = cr;

            var groups = comicSiteCatcher.GetPaginations();
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

                #region IsComicNameNode then ShowComicData
                if (TreeViewUtil.IsComicNameNode(tvComicTree.SelectedNode))
                {
                    // 如果是漫畫名稱節點，就顯示icon圖片
                    pbIcon.Image = null;
                    var comicEntity = tvComicTree.SelectedNode.Tag as ComicEntity;
                    try
                    {
                        lock (pbIcon)
                        {
                            pbIcon.Image = comicEntity.IconImage;
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogger.Error("顯示icon失敗," + ex.ToString());
                        pbIcon.Image = null;
                    }
                    finally
                    {
                        pbIcon_Paint(pbIcon, null);
                    }

                    // 顯示其餘資料
                    try
                    {
                        string pathGroupName = pathGroupDic.GetGroupName(tvComicTree.SelectedNode.Text);

                        string today = DateTime.Today.ToString("MM月dd号");
                        string yestoday = DateTime.Today.AddDays(-1).ToString("MM月dd号");
                        string beforeYestoday = DateTime.Today.AddDays(-2).ToString("MM月dd号");

                        txtUrl.Text = tvComicTree.SelectedNode.Name;
                        lblUpdateDate.Text = comicEntity.LastUpdateDate.Replace("今天", today).Replace("昨天", yestoday).Replace("前天", beforeYestoday);
                        if (lblUpdateDate.Text.Contains("分钟前更新"))
                        {
                            lblUpdateDate.Text = today + lblUpdateDate.Text;
                        }
                        lblUpdateChapter.Text = comicEntity.LastUpdateChapter;
                        if (false == cbRelateFolders.Items.Contains(pathGroupName)) cbRelateFolders.Items.Add(pathGroupName);

                        cbRelateFolders.Text = pathGroupName;

                        // 章節在另一個 treeview 顯示

                        //nd.Nodes[0].Nodes.Add(tvComicTree.SelectedNode.Text);

                        var comic = tvComicTree.SelectedNode.Tag as ComicEntity;
                        TreeNode nd = new TreeNode(comic.Caption);
                        if (comic != null)
                        {
                            foreach (var chapter in comic.Chapters)
                            {
                                var chapterNode = TreeViewUtil.BuildNode(chapter, comicSiteCatcher.GetRoot().WebSiteName, comic.Caption);
                                nd.Nodes.Add(chapterNode);
                            }
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
                if (TreeViewUtil.IsPaginationNode(tvComicTree.SelectedNode))
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
                Cursor = Cursors.Arrow;
                //Application.DoEvents();
            }
        }

        private void tvComicTree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
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


        private void BuildLocalComicDirComboBox()
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
            if (false == TreeViewUtil.IsChapterNode(tn)) return;

            string taskname = tn.Parent.Text + "/" + tn.Text;
            string localPath = Path.Combine(Path.Combine(txtRootPath.Text, pathGroupDic.GetGroupName(tn.Parent.Text)), tn.Text);

            try
            {
                DownloadedList.AddDownloaded(comicSiteCatcher.GetRoot().WebSiteName, tn.Parent.Text, tn.Text);
            }
            catch { }

            //string fullPath1 = tn.Parent.FullPath;
            //string fullPath2 = tvComicTree.SelectedNode.FullPath;
            TreeViewUtil.SetFontBold(tvComicTree.SelectedNode);
            TreeViewUtil.SetFontBold(tn, Color.Blue);

            if ((queue.Count<DownloadChapterTask>(q => q.Name == taskname)) > 0)
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
            queue.Enqueue(new DownloadChapterTask() { Name = taskname, Path = localPath, Chapter = cc, Downloader = comicSiteCatcher });

            NLogger.Info(taskname + " 已加入下載排程！");
            statusMsg.Text = taskname + " 已加入下載排程！";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            statusMsg2.Text = "等待下載數=" + queue.Count.ToString() + "，正在下載數=" + (bgWorker.IsBusy ? "1" : "0");
            if (bgWorker.IsBusy) return;

            if (queue.Count <= 0) return;
            DownloadChapterTask myTask = queue.Dequeue();
            bgWorker.RunWorkerAsync(myTask);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DownloadChapterTask myTask = (DownloadChapterTask)e.Argument;
            DownloadComic(myTask);
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerMsg msg = (WorkerMsg)e.UserState;
            if (false == String.IsNullOrEmpty(msg.statusMsg)) statusMsg.Text = msg.statusMsg;
            if (false == String.IsNullOrEmpty(msg.infoMsg)) NLogger.Info(msg.infoMsg);
        }

        private void DownloadComic(DownloadChapterTask task)
        {
            if (false == Directory.Exists(task.Path)) Directory.CreateDirectory(task.Path);

            bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = $"[{task.Name}]準備開始下載", infoMsg = $"[{task.Name}]準備開始下載" });
            var pages = task.Downloader.GetPages(task.Chapter);
            bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = $"[{task.Name}]準備開始下載", infoMsg = $"[{task.Name}]，共" + pages.Count + "頁" });

            ComicUtil.DownloadChapter(task);

            GC.Collect();

            //statusMsg.Text = "[" + myTask.name + "]已經下載完成";
            bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = $"[{task.Name}]已經下載完成", infoMsg = $"[{task.Name}]已經下載完成" });
            if (chkArchiveDownloadedFile.Checked && File.Exists(settings.WinRARPath))
            {
                RarHelper rar = new RarHelper(settings.WinRARPath);
                try
                {
                    rar.ArchiveDirectory(task.Path);
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
                BuildLocalComicDirComboBox();
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
                if (false == TreeViewUtil.IsComicNameNode(node))
                {
                    if (0 >= node.Nodes.Count)
                    {
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
            RarHelper rar = null;
            try
            {
                rar = new RarHelper(settings.WinRARPath);
                rar.ArchiveDirectory(Path.Combine(txtRootPath.Text, cbRelateFolders.Text, tvFolder.SelectedNode.Text));
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
                settings.Save();
                BindSettingsToView();
                MessageBox.Show("儲存完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show("儲存失敗：" + ex.ToString());
            }
        }

        private void BindSettingsToView()
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

        private void buildComicNodeBackground(object objNode)
        {
            TreeNode currNode = objNode as TreeNode;
            if (currNode == null) return;
            try
            {
                if (TreeViewUtil.IsPaginationNode(currNode))
                {
                    buildComicNameNode(currNode);
                    //var nodeList = currNode.Nodes.Cast<TreeNode>().ToList();
                    //Parallel.ForEach(nodeList, node =>
                    //{
                    //    buildComicChapterNode(node);
                    //});
                }
                //else if (TreeViewUtil.IsComicNameNode(currNode))
                //{
                //    comicSiteCatcher.GetChapters(cn);

                //    buildComicChapterNode(currNode);
                //}
            }
            catch (Exception ex)
            {
                NLogger.Error("buildComicNodeBackground," + ex.ToString());
                throw ex;
            }
        }

        private void buildComicNameNode(TreeNode paginationNode)
        {
            if (null == paginationNode) return;
            if (false == TreeViewUtil.IsPaginationNode(paginationNode)) return;

            try
            {
                var nameNodes = new List<TreeNode>();

                TreeViewUtil.ClearTreeNode(paginationNode);
                var cg = paginationNode.Tag as ComicPagination;
                comicSiteCatcher.GetComics(cg, this.settings.LoadAllPicture);
                cg.Comics = cg.Comics.Where(c => false == ignoreComicDic.IsIgnored(c.Url)).ToList();
                foreach (var comic in cg.Comics)
                {
                    string groupName = pathGroupDic.GetGroupName(comic.Caption);
                    TreeNode nameNode = TreeViewUtil.BuildNode(comic, txtRootPath.Text, groupName);
                    nameNodes.Add(nameNode);
                    // 完成 comicNode 時載入chapter 節點
                    Task.Run(() => this.comicSiteCatcher.GetChapters(comic));
                }
                TreeViewUtil.AddTreeNodes(paginationNode, nameNodes);
            }
            catch (Exception ex)
            {
                NLogger.Error("產生漫畫名稱節點錯誤:" + paginationNode.Name);
                NLogger.Error(ex.ToString());
                TreeViewUtil.SetFontBold(paginationNode, Color.Red);
            }
        }

        //public void buildComicChapterNode(object nameNode)
        //{
        //    TreeNode currNode = nameNode as TreeNode;
        //    if (null == currNode) return;
        //    if (false == NodeCheckUtil.IsComicNameNode(currNode)) return;

        //    try
        //    {
        //        //string fullPath = currNode.FullPath;
        //        // 產生漫畫的回合子節點
        //        //NLogger.Info("********************************************");
        //        TreeViewUtil.ClearTreeNode(currNode);
        //        GC.Collect();
        //        ComicEntity cn = currNode.Tag as ComicEntity;
        //        comicSiteCatcher.GetChapters(cn);
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogger.Error("產生漫畫回數節點錯誤:" + currNode.Name);
        //        NLogger.Error(ex.ToString());
        //        TreeViewUtil.SetFontBold(currNode, Color.Red);
        //    }
        //    finally
        //    {

        //    }
        //}
        #endregion

        private void cbComicCatcher_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbComicCatcher.Text.ToUpper())
            {
                case "XINDM":
                    ChangeComicSite(new Xindm());
                    break;
                case "DM5":
                    ChangeComicSite(new Dm5());
                    break;
            }
        }

        private void btnShowEditModal_Click(object sender, EventArgs e)
        {
            ComicSettingsGroup f = new ComicSettingsGroup(SettingEnum.PathGroup);
            f.ShowDialog(this);
            this.pathGroupDic = PathGroupDic.Load();
        }
        private void btnShowExceptModal_Click(object sender, EventArgs e)
        {
            ComicSettingsGroup f = new ComicSettingsGroup(SettingEnum.IgnoreComic);
            f.ShowDialog(this);
            this.ignoreComicDic = IgnoreComicDic.Load();
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
                    var nameNode = BuildOneComicNameNode(txtUrl.Text);
                    tvComicTree.Nodes[0].Nodes[0].Nodes.Add(nameNode);
                    tvComicTree.Nodes[0].ExpandAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("不是有效的url:" + ex.Message);
            }
        }

        private TreeNode BuildOneComicNameNode(string url)
        {
            Dm5 dm5 = new Dm5();
            ComicEntity comic = dm5.GetSingleComicName(url);
            var groupName = pathGroupDic.GetGroupName(comic.Caption);
            var nameNode = TreeViewUtil.BuildNode(comic, txtRootPath.Text, groupName);
            return nameNode;
        }

        private void AddIgnoreComic_Click(object sender, EventArgs e)
        {
            try
            {
                ignoreComicDic.Add(tvComicTree.SelectedNode.Name, tvComicTree.SelectedNode.Text);
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
                if (TreeViewUtil.IsComicNameNode(e.Node) && e.Node == tvComicTree.SelectedNode)
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

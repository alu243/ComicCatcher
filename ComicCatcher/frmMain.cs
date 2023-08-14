using ComicCatcher.Helpers;
using ComicCatcher.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using ComicCatcherLib.ComicModels;
using ComicCatcherLib.DbModel;
using ComicCatcherLib.Utils;
using ComicCatcherLib.Helpers;
using ComicCatcherLib.ComicModels.Domains;
using ComicCatcherLib.Models;

namespace ComicCatcher
{
    public partial class frmMain : Form
    {
        private static readonly Queue<DownloadChapterRequest> queue = new();

        private Settings settings = null;

        private IComicCatcher catcher = null;

        private PathGroupDic pathGroupDic = null;
        private IgnoreComicDic ignoreComicDic = null;
        frmView view;

        public frmMain()
        {
            try
            {
                ExportInteropFile();
                InitializeComponent();
                view = new frmView();
                NLogger.SetBox(this.txtInfo);
            }
            catch (Exception ex)
            { MessageBox.Show("錯誤發生" + ex.ToString()); }
        }

        private void ExportInteropFile()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream asmfs;
            var resources = new List<string>() { "e_sqlite3.dll", "WebView2Loader.dll" };

            foreach (var resource in resources)
            {
                if (false == File.Exists(@$".\{resource}"))
                {
                    asmfs = asm.GetManifestResourceStream($"ComicCatcher.x64.{resource}");
                    using (FileStream fs = new FileStream(@$".\{resource}", FileMode.Create, FileAccess.Write))
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
        }

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
                    Cursor = Cursors.Default;
                }
            }
        }

        private void ChangeComicSite(IComicCatcher newCatcher)
        {
            catcher = newCatcher;
            InitialComicRootTree();
        }

        private void InitialComicRootTree()
        {
            ComicRoot root = catcher.GetRoot();

            TreeViewUtil.ClearTree(tvComicTree);
            TreeNode rootNode = TreeViewUtil.BuildNode(root);
            tvComicTree.Nodes.Add(rootNode);

            foreach (var pagination in root.Paginations)
            {
                var paginationNode = TreeViewUtil.BuildNode(pagination);
                rootNode.Nodes.Add(paginationNode);
            }
            tvComicTree.ExpandAll();

            // 預設讀取前5頁的資料
            //BuildComicNameNode(rootNode.Nodes[0], false);
            for (int i = 0; i < 5; i++)
            {
                // 背景更新，直接用 rootNodes.Nodes[i] 順序不知道為什麼是亂掉
                var pageNode = tvComicTree.Nodes[0].Nodes[i];
                Task.Run(async () => await BuildComicNameNode(pageNode, false));
            }
            tvComicTree.SelectedNode = tvComicTree.Nodes[0];
        }

        private void tvComicTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (null == tvComicTree.SelectedNode) return;
            try
            {
                var currNode = tvComicTree.SelectedNode;
                //清單如果沒有子節點，就產生子節點(漫畫)
                if (TreeViewUtil.IsPaginationNode(currNode))
                {
                    Task.Run(async () => await BuildComicNameNode(currNode, false));
                }

                #region IsComicNameNode then ShowComicData
                else if (TreeViewUtil.IsComicNameNode(currNode))
                {
                    this.BindComicNodeToView(currNode);
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("取得節點訊息時發生錯誤：" + ex.ToString());
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void BindComicNodeToView(TreeNode comicNode)
        {
            // 如果是漫畫名稱節點，就顯示icon圖片
            var comic = comicNode.Tag as ComicEntity;
            #region BindLogo
            try
            {
                pbIcon.Image = null;
                if (comic.ImageState == ComicState.Created)
                {
                    pbIcon.Image = null;
                }
                else if (comic.ImageState == ComicState.Processing)
                {
                    pbIcon.Image = null;
                }
                else if (comic.ImageState == ComicState.ImageLoaded)
                {
                    pbIcon.Image = Image.FromStream(comic.IconImage);
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
            #endregion

            // 顯示其餘資料
            try
            {
                #region BindTextBox
                string pathGroupName = pathGroupDic.GetGroupName(tvComicTree.SelectedNode.Text);

                string today = DateTime.Today.ToString("MM月dd号");
                string yestoday = DateTime.Today.AddDays(-1).ToString("MM月dd号");
                string beforeYestoday = DateTime.Today.AddDays(-2).ToString("MM月dd号");

                txtUrl.Text = tvComicTree.SelectedNode.Name;
                lblUpdateDate.Text = comic.LastUpdateDate.Replace("今天", today).Replace("昨天", yestoday).Replace("前天", beforeYestoday);
                if (lblUpdateDate.Text.Contains("分钟前更新"))
                {
                    lblUpdateDate.Text = today + lblUpdateDate.Text;
                }
                lblUpdateChapter.Text = comic.LastUpdateChapter;
                if (false == cbRelateFolders.Items.Contains(pathGroupName)) cbRelateFolders.Items.Add(pathGroupName);

                cbRelateFolders.Text = pathGroupName;
                #endregion

                // 章節在另一個 treeview 顯示
                #region BindChapter

                TreeNode nd = new TreeNode(comic.Caption);
                if (comic.ListState == ComicState.Processing)
                {
                    nd.Nodes.Add("讀取中...");
                }
                else if (comic.ListState == ComicState.Created)
                {
                    nd.Nodes.Add("等待讀取...");
                }
                else if (comic.ListState == ComicState.ListError)
                {
                    nd.Nodes.Add("讀取錯誤...");
                }
                else if (comic.ListState == ComicState.ListLoaded)
                {
                    foreach (var chapter in comic.Chapters)
                    {
                        var chapterNode = TreeViewUtil.BuildNode(chapter, catcher.GetRoot().WebSiteName, comic.Caption);
                        nd.Nodes.Add(chapterNode);
                    }
                }
                treeViewnComicName.Nodes.Clear();
                treeViewnComicName.BeginUpdate();
                treeViewnComicName.Nodes.Add(nd);
                treeViewnComicName.ExpandAll();
                treeViewnComicName.EndUpdate();
                #endregion
            }
            catch (Exception ex)
            {
                NLogger.Error("顯示漫畫節點資料失敗," + ex.ToString());
                pbIcon.Image = null;
            }
        }

        private void tvComicTree_KeyUp(object sender, KeyEventArgs e)
        {
            // Reload
            if (e.KeyCode == Keys.F5)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    TreeViewUtil.ClearSubNode(tvComicTree.SelectedNode);

                    // 分頁重設
                    var treeNode = tvComicTree.SelectedNode;
                    if (TreeViewUtil.IsPaginationNode(treeNode))
                    {
                        BuildComicNameNode(treeNode, true);
                        //tvComicTree.SelectedNode.Expand();
                    }
                    // 漫畫重讀章節
                    else if (TreeViewUtil.IsComicNameNode(treeNode))
                    {
                        this.catcher.LoadChapters(treeNode.Tag as ComicEntity);
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Error(ex.ToString());
                }
                finally
                {
                    Cursor = Cursors.Arrow;
                    //Application.DoEvents();
                }
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

        private void AddDownloadTask(TreeNode tn)
        {
            if (false == TreeViewUtil.IsChapterNode(tn)) return;

            string taskname = tn.Parent.Text + "/" + tn.Text;
            string localPath = Path.Combine(Path.Combine(txtRootPath.Text, pathGroupDic.GetGroupName(tn.Parent.Text)), tn.Text);

            try
            {
                DownloadedList.AddDownloaded(catcher.GetRoot().WebSiteName, tn.Parent.Text, tn.Text);
            }
            catch { }

            TreeViewUtil.SetFontBold(tvComicTree.SelectedNode);
            TreeViewUtil.SetFontBold(tn, Color.Blue);

            if ((queue.Count(q => q.Name == taskname)) > 0)
            {
                NLogger.Info(taskname + " 已在下載排程，不重新加入！");
                statusMsg.Text = taskname + " 已在下載排程，不重新加入！";
                return;
            }

            if (File.Exists(localPath + ".rar"))
            {
                NLogger.Info(taskname + " 已壓縮封存，不加入排程下載！");
                statusMsg.Text = taskname + " 已壓縮封存，不加入排程下載！";
                return;
            }

            var chapter = tn.Tag as ComicChapter;
            queue.Enqueue(new DownloadChapterRequest() { Name = taskname, Path = localPath, Chapter = chapter });

            NLogger.Info(taskname + " 已加入下載排程！");
            statusMsg.Text = taskname + " 已加入下載排程！";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            statusMsg2.Text = "等待下載數=" + queue.Count.ToString() + "，正在下載數=" + (bgWorker.IsBusy ? "1" : "0");
            if (bgWorker.IsBusy) return;

            if (queue.Count <= 0) return;
            DownloadChapterRequest myTask = queue.Dequeue();
            bgWorker.RunWorkerAsync(myTask);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DownloadChapterRequest myTask = (DownloadChapterRequest)e.Argument;
            DownloadComic(myTask);
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is WorkerMsg msg)
            {
                if (false == String.IsNullOrEmpty(msg.StatusMsg)) statusMsg.Text = msg.StatusMsg;
                if (false == String.IsNullOrEmpty(msg.InfoMsg)) NLogger.Info(msg.InfoMsg);
            }
            else if (e.UserState is string sMsg)
            {
                statusMsg.Text = sMsg;
            }
        }

        private void DownloadComic(DownloadChapterRequest task)
        {
            task.ReportProgressAction = bgWorker.ReportProgress;

            bgWorker.ReportProgress(0, new WorkerMsg() { StatusMsg = $"[{task.Name}]準備開始下載", InfoMsg = $"[{task.Name}]準備開始下載" });

            this.catcher.DownloadChapter(task).Wait();

            //statusMsg.Text = "[" + myTask.name + "]已經下載完成";
            bgWorker.ReportProgress(0, new WorkerMsg() { StatusMsg = $"[{task.Name}]已經下載完成", InfoMsg = $"[{task.Name}]已經下載完成" });
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
            Task.Delay(1).Wait();
        }
        #endregion


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


        #region BuildComicNode
        private async Task BuildComicNameNode(TreeNode paginationNode, bool rebuild)
        {
            if (false == TreeViewUtil.IsPaginationNode(paginationNode)) return;
            //if (false == rebuild && paginationNode.Nodes.Count > 0) return;
            if (false == rebuild && (paginationNode.Tag as ComicPagination).ListState != ComicState.Created) return;
            try
            {
                (paginationNode.Tag as ComicPagination).ListState = ComicState.Created;
                await this.catcher.LoadComics(paginationNode.Tag as ComicPagination, ignoreComicDic.GetDictionary());

                var nameNodes = new List<TreeNode>();
                foreach (var comic in (paginationNode.Tag as ComicPagination).Comics)
                {
                    string groupName = pathGroupDic.GetGroupName(comic.Caption);
                    TreeNode nameNode = TreeViewUtil.BuildNode(comic, txtRootPath.Text, groupName);
                    nameNodes.Add(nameNode);
                }
                TreeViewUtil.ClearSubNode(paginationNode);
                TreeViewUtil.AddTreeNodes(paginationNode, nameNodes);
            }
            catch (Exception ex)
            {
                NLogger.Error("產生漫畫名稱節點錯誤:" + paginationNode.Name);
                NLogger.Error(ex.ToString());
                TreeViewUtil.SetFontBold(paginationNode, Color.Red);
            }
        }

        private async Task<TreeNode> BuildOneComicNameNode(string url)
        {
            Dm5 dm5 = new Dm5();
            var comic = await dm5.GetSingleComicName(url);
            var groupName = pathGroupDic.GetGroupName(comic.Caption);
            var nameNode = TreeViewUtil.BuildNode(comic, txtRootPath.Text, groupName);
            return nameNode;
        }
        #endregion

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

        #region 本地端漫畫節點
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
        private void cbRelateFolders_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buildLocalComicTreeView();
            }
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
                CMDUtil.ExecuteCommandAsync(new CommandObj() { fileName = settings.PhotoProgramPath, arguments = arugment });
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

        /// <summary>
        /// 儲存設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private void tvComicTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            var font = e.Node.NodeFont == null ? (sender as TreeView).Font : e.Node.NodeFont;
            if (e.Node.ImageIndex == 99) // if there is no image
            {
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
        }

        #endregion


        private void OpenDirectory_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", Path.Combine(txtRootPath.Text, cbRelateFolders.Text));
            }
            catch (Exception ex)
            {
                NLogger.Error(ex.ToString());
            }
        }

        private void cbComicCatcher_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbComicCatcher.Text.ToUpper())
            {
                //case "XINDM":
                //    ChangeComicSite(new Xindm());
                //    break;
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

        private async void btnAppendTo_Click(object sender, EventArgs e)
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
                    var nameNode = await BuildOneComicNameNode(txtUrl.Text);
                    tvComicTree.Nodes[0].Nodes[0].Nodes.Add(nameNode);
                    tvComicTree.Nodes[0].ExpandAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("不是有效的url:" + ex.Message);
            }
        }

        private void AddIgnoreComic_Click(object sender, EventArgs e)
        {
            try
            {
                ignoreComicDic.Add(tvComicTree.SelectedNode.Name, tvComicTree.SelectedNode.Text);
                TreeViewUtil.ClearSubNode(tvComicTree.SelectedNode);
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

        private void btnViewHtml_Click(object sender, EventArgs e)
        {
            var viewPath = Path.Combine(txtRootPath.Text, "view.html");
            var files = Directory.GetFiles(Path.Combine(txtRootPath.Text, cbRelateFolders.Text, tvFolder.SelectedNode.Text));
            using (StreamWriter writer = new StreamWriter(viewPath))
            {
                writer.WriteLine(@"<html><header></header><body onload=""sc()"">");
                foreach (var file in files)
                {
                    //style=""min-wdith:70%;width:95%;""
                    writer.WriteLine($@"<div><img src=""file:///{file}"" alt=""Snow""></div>");
                }
                writer.WriteLine("<script>function sc(){window.scrollTo({ top: 0, left: 0, behavior: 'smooth' });}</script></body></html>");

            }
            view.SetPath(viewPath);
            view.ShowDialog(this);

            //WebView2 wb2 = new WebView2();
            //wb2.EnsureCoreWebView2Async().Wait();
            //wb2.CoreWebView2.Navigate(viewPath);
            //wb2.Show();
        }
    }
}

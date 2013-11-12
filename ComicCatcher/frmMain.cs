using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using ComicCatcher.App_Code;
using ComicCatcher.App_Code.Util;
using ComicCatcher.App_Code.Comic;
using System.Threading;
using System.Text.RegularExpressions;
namespace ComicCatcher
{
    public partial class frmMain : Form
    {
        private static readonly Queue<Tasker> queue = new Queue<Tasker>();

        private Settings settings = null;

        private DownloadedList dwnedList = null;

        public frmMain()
        {
            InitializeComponent();
            //txtWeburl.GotFocus += new EventHandler(textBox_GotFocus);
        }

        //private void textBox_GotFocus(object sender, EventArgs e)
        //{
        //    ((TextBox)sender).SelectAll();
        //}

        private void tvComicTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (null != tvComicTree.SelectedNode)
                {
                    txtUrl.Text = tvComicTree.SelectedNode.Name;

                    // 如果是漫畫名稱節點，就顯示圖片
                    if (IsComicNameNode(tvComicTree.SelectedNode))
                    {
                        pbIcon.Image = null;
                        try
                        {
                            ComicBase comicData = tvComicTree.SelectedNode.Tag as ComicBase;
                            if (null != comicData)
                            {
                                if (null == comicData.imageData || comicData.imageData.Length <= 0)
                                {
                                    comicData.imageData = HttpHelper.getPictureResponse(tvComicTree.SelectedNode.ImageKey);
                                }
                                pbIcon.Image = Image.FromStream(comicData.imageData);
                                lblUpdateDate.Text = comicData.updateDate;
                                lblUpdateChapter.Text = comicData.updateChapter;
                            }
                            else
                            {
                                pbIcon.Image = Image.FromStream(HttpHelper.getPictureResponse(tvComicTree.SelectedNode.ImageKey));
                            }

                            if (false == cbRelateFolders.Items.Contains(tvComicTree.SelectedNode.Text)) cbRelateFolders.Items.Add(tvComicTree.SelectedNode.Text);

                            cbRelateFolders.Text = tvComicTree.SelectedNode.Text;

                            pbIcon_Paint(pbIcon, null);
                        }
                        catch { }
                    }

                    // 如果沒有子節點，就產生子節點
                    if (tvComicTree.SelectedNode.Nodes.Count <= 0)
                    {
                        try
                        {
                            Cursor = System.Windows.Forms.Cursors.WaitCursor;
                            buildComicSubNode(tvComicTree.SelectedNode);
                            tvComicTree.SelectedNode.Expand();
                        }
                        finally
                        {
                            Cursor = System.Windows.Forms.Cursors.Arrow;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("取得節點訊息時發生錯誤：" + ex.ToString());
            }
        }

        private void buildComicSubNode(TreeNode currNode)
        {
            // node.Name 就是 URL
            // 取得該頁下所有的漫畫清單
            //txtInfo.Text = "";
            if (IsListNode(currNode))
            {
                currNode.Nodes.Clear();
                // 產生清單下的漫畫名稱內容子節點
                txtInfo.AppendText("********************************************" + Environment.NewLine);
                using (ComicList cp = new ComicList(currNode.Name))
                {
                    var comicList = cp.getComicBaseList();
                    foreach (var comic in comicList)
                    {
                        try
                        {
                            // 預設在展開分頁時載入全部縮圖(抓檔時比較快)
                            if (chkLoadPhoto.Checked)
                            {
                                comic.imageData = HttpHelper.getPictureResponse(comic.iconUrl); // getComicIcon
                            }
                        }
                        catch
                        { }
                        TreeNode tn = new TreeNode();
                        tn.Name = comic.url;
                        tn.Text = comic.description;
                        tn.ImageKey = comic.iconUrl;
                        tn.Tag = comic;
                        currNode.Nodes.Add(tn);
                        //currNode.Nodes.Add(comic.url, comic.description, comic.iconUrl);
                        txtInfo.AppendText(comic.description + "=" + comic.url + Environment.NewLine);
                    }
                }
                txtInfo.AppendText("********************************************" + Environment.NewLine);
            }
            else if (IsComicNameNode(currNode))
            {
                currNode.Nodes.Clear();
                // 產生漫畫的回合子節點
                using (ComicName cv = new ComicName(currNode.Name))
                {
                    var chapterList = cv.getComicBaseList();
                    //txtInfo.AppendText("********************************************" + Environment.NewLine);
                    foreach (var comic in chapterList)
                    {
                        TreeNode tn = currNode.Nodes.Add(comic.url, comic.description);
                        // 如果是已下載過的點，變粗體
                        if (null != dwnedList && dwnedList.HasDownloaded(currNode.Text, comic.description))
                        {
                            tn.NodeFont = new Font(tvComicTree.Font, FontStyle.Bold);
                        }

                        //txtInfo.AppendText(comic.description + "=" + comic.url + Environment.NewLine);
                    }
                    //txtInfo.AppendText("********************************************" + Environment.NewLine);
                }
            }
            else if (IsChapterNode(currNode))
            {
                //currNode.Nodes.Clear();
                //using (ComicChapter cp = new ComicChapter(currNode.Name))
                //{
                //    var picUrlList = cp.genPictureUrl();
                //    txtInfo.AppendText("********************************************" + Environment.NewLine);
                //    for (int i = 0; i < picUrlList.Count; ++i)
                //    {
                //        txtInfo.AppendText(i.ToString().PadLeft(3, '0') + ".jpg = " + picUrlList[i] + Environment.NewLine);
                //    }
                //    txtInfo.AppendText("********************************************" + Environment.NewLine);
                //}
            }
        }

        private void BuildComicRootTree()
        {
            tvComicTree.Nodes.Clear();
            TreeNode root = new TreeNode(Xindm.Title);
            tvComicTree.Nodes.Add(root);
            string pagePtn = Xindm.ListUrl + "?page={0}&classid=8&tempid=17&line=72";
            for (int i = 0; i < 117; ++i)
            {
                root.Nodes.Add(String.Format(pagePtn, i.ToString()), "第" + (i + 1).ToString().PadLeft(2, '0') + "頁");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BuildComicRootTree();
            tvComicTree.ExpandAll();
            lblUpdateDate.Text = "";
            lblUpdateChapter.Text = "";
            genSubDirInComboPath();

            settings = new Settings();
            try
            {
                settings = Settings.load();
            }
            catch { }
            settings.save();
            SyncSettingToForm();

            dwnedList = new DownloadedList();
            try
            {
                dwnedList = DownloadedList.load();
            }
            catch { }


            //XMLBuilder myXML = new XMLBuilder();
            //myXML.CreateDefaultXML();
        }

        private void SyncSettingToForm()
        {
            /// 設定畫面
            setPhotoProgramPath.Text = settings.PhotoProgramPath;
            setWinRARPath.Text = settings.WinRARPath;
            setLocalPath.Text = settings.LocalPath;
            setLoadAllPicture.Checked = settings.LoadAllPicture;
            setUsingProxy.Checked = settings.UsingProxy;
            setProxyUrl.Text = settings.ProxyUrl;
            setProxyPort.Text = settings.ProxyPort.ToString();

            /// 使用畫面
            txtRootPath.Text = settings.LocalPath;
            chkLoadPhoto.Checked = settings.LoadAllPicture;
            chkIsUseProxy.Checked = settings.UsingProxy;

            /// Proxy設定(要在使用畫面設定完成後)
            App_Code.Util.UsingProxy.isUseProxy = chkIsUseProxy.Checked;
            App_Code.Util.UsingProxy.ProxyUrl = settings.ProxyUrl;
            App_Code.Util.UsingProxy.ProxyPort = settings.ProxyPort;

        }

        private void SyncSettingFromForm()
        {
            settings.PhotoProgramPath = setPhotoProgramPath.Text.Trim();
            settings.WinRARPath = setWinRARPath.Text.Trim();
            settings.LocalPath = setLocalPath.Text.Trim();
            settings.LoadAllPicture = setLoadAllPicture.Checked;
            settings.UsingProxy = setUsingProxy.Checked;
            settings.ProxyUrl = setProxyUrl.Text.Trim();
            if (false == String.IsNullOrEmpty(setProxyPort.Text.Trim()))
                settings.ProxyPort = int.Parse(setProxyPort.Text.Trim());
        }


        private void btnShowpath_Click(object sender, EventArgs e)
        {
        }

        private void genSubDirInComboPath()
        {
            try
            {
                string[] dirs = new string[0];
                //dirs = Directory.GetDirectories(txtRootPath.Text);
                //string[] rars = Directory.GetFiles(txtRootPath.Text, "*.rar");
                //string[] newDirs = new string[dirs.Length + rars.Length];
                //Array.Copy(dirs, newDirs, dirs.Length);
                //Array.Copy(rars, 0, newDirs, dirs.Length, rars.Length);
                //Array.Sort(newDirs);
                //newDirs = newDirs.ToList().CustomSort().ToArray();
                //Array.Reverse(newDirs);
                cbRelateFolders.Items.Clear();
                //foreach (string dir in newDirs)
                foreach (string dir in dirs)
                {
                    cbRelateFolders.Items.Add(Path.GetFileName(dir));
                }
                cbRelateFolders.AutoCompleteCustomSource.Clear();
                cbRelateFolders.AutoCompleteCustomSource.AddRange(dirs);
            }
            catch (Exception ex)
            {
                MessageBox.Show("尋找目錄錯誤，原因：" + ex.ToString());
            }
        }

        private void cbFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            buildDirectoryTreeView();
        }

        /// <summary>
        /// 產生右邊資料夾的內容
        /// </summary>
        private void buildDirectoryTreeView()
        {
            try
            {
                string subPath = Path.Combine(txtRootPath.Text, cbRelateFolders.Text);
                if (false == Directory.Exists(subPath))
                {
                    //MessageBox.Show("路徑：" + subPath + " 不存在！");
                    lblCbMessage.Text = "路徑：" + subPath + " 不存在！";
                }
                else
                {
                    TreeNode root = new TreeNode(cbRelateFolders.Text, 0, 0);

                    string[] dirs = Directory.GetDirectories(subPath);
                    string[] rars = Directory.GetFiles(subPath, "*.rar");
                    string[] newDirs = new string[dirs.Length + rars.Length];
                    Array.Copy(dirs, newDirs, dirs.Length);
                    Array.Copy(rars, 0, newDirs, dirs.Length, rars.Length);
                    //Array.Sort(newDirs);
                    newDirs = newDirs.ToList().CustomSort().ToArray();
                    Array.Reverse(newDirs);
                    tvFolder.Nodes.Clear();
                    tvFolder.Nodes.Add(root);
                    foreach (string dir in newDirs)
                    {
                        //string test = Regex.Replace(dir, @"(\d+)", m => m.Value.PadLeft(10, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'));
                        //MatchCollection mm = Regex.Matches(dir, @"(\d+)|(\D+)");
                        if (0 == String.Compare(".RAR", Path.GetExtension(dir), true))
                            root.Nodes.Add(Path.GetFileName(dir), Path.GetFileName(dir), 1, 1);
                        else
                            root.Nodes.Add(Path.GetFileName(dir), Path.GetFileName(dir), 2, 2);
                    }
                    lblCbMessage.Text = "共" + newDirs.Count().ToString() + "筆資料！";

                    root.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void txtPath_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                genSubDirInComboPath();
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (false == IsChapterNode(tvComicTree.SelectedNode)) return;

            AddTask(tvComicTree.SelectedNode);
        }

        private void AddTask(TreeNode tn)
        {
            string taskname = tn.Parent.Text + "/" + tn.Text;
            string localPath = Path.Combine(Path.Combine(txtRootPath.Text, tn.Parent.Text), tn.Text);
            string downUrl = tn.Name;

            try
            {
                dwnedList.AddDownloaded(tn.Parent.Text, tn.Text);
                dwnedList.save();
            }
            catch { }

            tn.NodeFont = new Font(tvComicTree.Font, FontStyle.Bold);

            if ((queue.Count<Tasker>(q => q.name == taskname)) > 0)
            {
                txtInfo.AppendText(taskname + " 已在下載排程，不重新加入！" + Environment.NewLine);
                statusMsg.Text = taskname + " 已在下載排程，不重新加入！";
                return;
            }

            if (true == File.Exists(localPath + ".rar"))
            {
                txtInfo.AppendText(taskname + " 已壓縮封存，不加入排程下載！" + Environment.NewLine);
                statusMsg.Text = taskname + " 已壓縮封存，不加入排程下載！";
                return;
            }

            //queue.Enqueue(new Scheduler() { name = taskname, downloadPath = localPath, downloadUrl = downUrl, alternativeUrl = downUrl.Replace(Xindm.PicHost, Xindm.PicHostAlternative) });
            // 下載網址加入排程(還沒有到下載圖片)
            queue.Enqueue(new Tasker() { name = taskname, downloadPath = localPath, downloadUrl = downUrl, usingAlternativeUrl = chkUsingAlternativeUrl.Checked });
            txtInfo.AppendText(taskname + " 已加入下載排程！" + Environment.NewLine);
            statusMsg.Text = taskname + " 已加入下載排程！";
        }

        private void DownloadComic(Tasker task)
        {
            string downUrl = task.downloadUrl;

            if (false == Directory.Exists(task.downloadPath)) Directory.CreateDirectory(task.downloadPath);
            using (ComicChapter cp = new ComicChapter(downUrl))
            {
                List<string> allPictureUrl = cp.genPictureUrl();

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
                int upperPage = (threadCount > allPictureUrl.Count ? allPictureUrl.Count : threadCount); // 目前多執行緒下載頁數
                while (startPage < allPictureUrl.Count)
                {
                    List<Thread> threadPool = new List<Thread>();
                    for (int i = startPage; i < upperPage; ++i)
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(DownloadPicture));
                        threadPool.Add(t);
                        // 圖片加入下載排程
                        //t.Start(new Scheduler() { name = task.name, downloadPath = task.downloadPath, downloadUrl = allPictureUrl[i], alternativeUrl = allPictureUrl[i].Replace(Xindm.PicHost, Xindm.PicHostAlternative) });
                        if (task.usingAlternativeUrl) // 使用替代網址下載(較慢)
                        {
                            t.Start(new DownloadPictureScheduler() { name = task.name, downloadPath = task.downloadPath, downloadUrl = allPictureUrl[i].Replace(Xindm.PicHost, Xindm.PicHostAlternative) });
                        }
                        else // 使用正常網址下載(較快)
                        {
                            t.Start(new DownloadPictureScheduler() { name = task.name, downloadPath = task.downloadPath, downloadUrl = allPictureUrl[i] });
                        }
                    } // end of foreach
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
            } // end of using

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

                #region 原本要作自動切換下載，但是遇到會檔proxy ip，所以作罷
                //// 檢查圖片檔是不是有問題
                //if (false == String.IsNullOrEmpty(pictureUrlAlter)) // 如果沒有替代網址，那也沒用
                //{
                //    if (0 != pictureUrl.CompareTo(pictureUrlAlter)) // 已經換成替代網址的話，也不用再看有沒有問題
                //    {
                //        FileInfo f = new FileInfo(tmpFile);
                //        if (f.Exists)
                //        {
                //            if (f.Length <= 3072) // 圖片檔小於等資 3k 就當作是有問題的資料
                //            {
                //                pictureUrl = pictureUrlAlter;
                //                tmpFile = Path.Combine(localPath, Path.GetFileName(pictureUrl)) + ".tmp";
                //                cmpFile = Path.Combine(localPath, Path.GetFileName(pictureUrl)) + ".cmp";
                //                DonwloadHelper.donwload(pictureUrl, tmpFile);
                //            }
                //        }
                //        f = null;
                //    }
                //} 
                #endregion
                
                DonwloadHelper.donwload(pictureUrl, tmpFile);

                // 檢查 50 次下載，如果還是都有問題，就跳出錯誤
                int i = 0;
                int testTimes = 50;
                for (i = 0; i < testTimes; ++i)
                {
                    DonwloadHelper.donwload(pictureUrl, cmpFile);
                    if (FileUtil.CompareMD5(tmpFile, cmpFile))
                    {
                        if (File.Exists(cmpFile)) File.Delete(cmpFile);
                        break;
                    }
                    else
                    {
                        // 把暫存圖片檔刪除，以後來下載的比較檔作為下一次比較的暫存檔
                        FileUtil.MoveFileOverwrite(cmpFile, tmpFile);
                        // txtInfo.AppendText("[" + myTask.name + "]檔案：" + tmpFile.Replace(".tmp", "") + "，比對失敗，重新下載比對！" + Environment.NewLine);
                        bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = "", infoMsg = tagname + "檔案：" + fileName + "，比對失敗，重新下載比對！" + ThreadID });
                        System.Threading.Thread.Sleep(1);
                    }
                }

                if (i >= testTimes)
                {
                    throw new Exception("檔案：" + fileName + " 不完整(已重新下載了" + i.ToString() + "次)");
                }
                else
                {
                    // rename file
                    FileUtil.MoveFileOverwrite(tmpFile, Path.Combine(localPath, Path.GetFileName(pictureUrl)));
                }
            }
            catch (Exception ex)
            {
                //txtInfo.AppendText("[" + myTask.name + "]下載失敗，原因：" + ex.ToString() + Environment.NewLine);
                bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = "", infoMsg = tagname + "下載失敗" + ThreadID + "，原因：" + ex.ToString() });
                Thread.Sleep(1);
            }
            finally
            {
                if (File.Exists(Path.Combine(localPath, Path.GetFileName(pictureUrl)) + ".cmp")) File.Delete(Path.Combine(localPath, Path.GetFileName(pictureUrl)) + ".cmp");

                //Cursor = System.Windows.Forms.Cursors.Arrow;
                // statusMsg.Text = "[" + myTask.name + "]下載完成..." + Path.GetFileName(pictureUrl);
                bgWorker.ReportProgress(0, new WorkerMsg() { statusMsg = tagname + fileName + "下載完成...", infoMsg = tagname + fileName + "下載完成..." + ThreadID });
                Thread.Sleep(1);
            }

            //try
            //{
            //    if (null != ((Scheduler)pictureSchedule).handle)
            //        ((Scheduler)pictureSchedule).handle.Set();
            //}
            //catch { }
        }

        #region 節點類別 Web -> List -> Comic -> Chapter
        /// <summary>
        /// 是否為顯示回數(集數)的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        private bool IsChapterNode(TreeNode tn)
        {
            if (null == tn) return false;

            if (false == tn.Name.Contains(Xindm.WebUrl)) return false; // 節點本身必需要是包含此內容
            // 如果父節點是漫畫名稱節點，表示此節點是回數(集數 )節點
            return IsComicNameNode(tn.Parent);
        }

        /// <summary>
        /// 是否為顯示漫畫名稱的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        private bool IsComicNameNode(TreeNode tn)
        {
            if (null == tn) return false;

            if (false == tn.Name.Contains(Xindm.WebUrl)) return false; // 節點本身必需要是包含此內容
            // 如果父節點是清單連結，表示此連結是漫畫名稱連結
            return IsListNode(tn.Parent);
        }

        /// <summary>
        /// 是否為顯示漫畫清單的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        private bool IsListNode(TreeNode tn)
        {
            if (null == tn) return false;
            return tn.Name.Contains(Xindm.ListUrl);
        }

        /// <summary>
        /// 是否為顯示漫畫網站的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        private bool IsWebNode(TreeNode tn)
        {
            if (null == tn) return false;

            return null == tn.Parent;
        }

        #endregion

        private void txtInfo_TextChanged(object sender, EventArgs e)
        {
            if (txtInfo.Lines.Length > 10000) txtInfo.Text = "";
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
            if (false == String.IsNullOrEmpty(msg.infoMsg)) txtInfo.AppendText(msg.infoMsg + Environment.NewLine);
        }

        private void tvComicTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IsChapterNode(tvComicTree.SelectedNode))
            {
                AddTask(tvComicTree.SelectedNode);
            }
        }

        private void tvComicTree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                try
                {
                    Cursor = System.Windows.Forms.Cursors.WaitCursor;
                    Application.DoEvents();
                    buildComicSubNode(tvComicTree.SelectedNode);
                }
                finally
                {
                    tvComicTree.Enabled = true;
                    Cursor = System.Windows.Forms.Cursors.Arrow;
                    Application.DoEvents();
                }
            }
        }

        private void cbRelateFolders_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buildDirectoryTreeView();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            App_Code.Util.UsingProxy.isUseProxy = chkIsUseProxy.Checked;
        }

        private void pbIcon_Paint(object sender, PaintEventArgs e)
        {
            if (null != (sender as PictureBox).Image && false == (sender as PictureBox).Image.Size.IsEmpty)
            {
                (sender as PictureBox).Visible = true;
            }
        }

        private void tvComicTree_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                //MessageBox.Show(tvComicTree.SelectedNode.Text);
                if (IsChapterNode(tvComicTree.SelectedNode))
                {
                    AddTask(tvComicTree.SelectedNode);
                }
            }
        }

        #region 尋找節點功能
        private void btnFind_Click(object sender, EventArgs e)
        {
            Boolean loadPhoto = chkLoadPhoto.Checked;
            chkLoadPhoto.Checked = false;
            findNode(tvComicTree.Nodes, txtFind.Text.Trim());
            chkLoadPhoto.Checked = loadPhoto;
        }
        private void txtFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                Boolean loadPhoto = chkLoadPhoto.Checked;
                chkLoadPhoto.Checked = false;
                findNode(tvComicTree.Nodes, txtFind.Text.Trim());
                chkLoadPhoto.Checked = loadPhoto;
            }
        }
        private bool findNode(TreeNodeCollection nodes, string findStr)
        {
            bool stopFind = false;
            if (stopFind) return true; // 確定找到後，依此flag決定要不要繼續找

            if (null == nodes) return false;
            if (0 >= nodes.Count) return false;
            foreach (TreeNode node in nodes)
            {
                if (stopFind) return true; // 已經在迴圈裡面的，再判斷一次是不是已經找到node了
                // 遇到頁的節點，先點一下展開後再繼續找
                if (Regex.IsMatch(node.Text, @"第\d+頁") && !stopFind)
                {
                    if (0 >= node.Nodes.Count)
                    {
                        tvComicTree.SelectedNode = node;
                        node.Collapse();
                    }
                }
                stopFind = findNode(node.Nodes, findStr);
                if (node.Text.Contains(findStr) && !stopFind)
                {
                    stopFind = (MessageBox.Show(node.Parent.Text + "\\" + node.Text, "找到節點", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes);
                    if (stopFind)
                    {
                        tvComicTree.SelectedNode = node;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            tvComicTree.CollapseAll();
            tvComicTree.Nodes[0].Expand();
        }

        private void tvFolder_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenComic();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            ArchiveComic();
        }

        private void OpenComic()
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
                CMDHelper.CMDHelper.ExecuteCommandAsync(new CommandObj() { fileName = settings.PhotoProgramPath, arguments = arugment });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ArchiveComic()
        {
            RARHelper.RARHelper rar = null;
            try
            {
                rar = new RARHelper.RARHelper(settings.WinRARPath);
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
                    buildDirectoryTreeView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SyncSettingFromForm();
                settings.save();
                SyncSettingToForm();
                MessageBox.Show("儲存完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show("儲存失敗：" + ex.ToString());
            }
        }

        private void tvFolder_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                OpenComic();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            buildDirectoryTreeView();
        }

        private void chkUsingAlternativeUrl_CheckedChanged(object sender, EventArgs e)
        {
            chkIsUseProxy.Checked = false;
        }
    }
}

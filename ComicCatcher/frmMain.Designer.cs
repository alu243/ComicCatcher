namespace ComicCatcher
{
    partial class frmMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("新動漫(xindm)");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            treeViewnComicName = new System.Windows.Forms.TreeView();
            btnAppendTo = new System.Windows.Forms.Button();
            cbComicCatcher = new System.Windows.Forms.ComboBox();
            chkArchiveDownloadedFile = new System.Windows.Forms.CheckBox();
            btnCollapse = new System.Windows.Forms.Button();
            chkLoadPhoto = new System.Windows.Forms.CheckBox();
            btnFind = new System.Windows.Forms.Button();
            txtFind = new System.Windows.Forms.TextBox();
            pbIcon = new System.Windows.Forms.PictureBox();
            lblUpdateChapter = new System.Windows.Forms.Label();
            lblUpdateDate = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            chkIsUseProxy = new System.Windows.Forms.CheckBox();
            txtUrl = new System.Windows.Forms.TextBox();
            tvComicTree = new System.Windows.Forms.TreeView();
            btnShowExceptModal = new System.Windows.Forms.Button();
            btnShowEditModal = new System.Windows.Forms.Button();
            btnOpenDirectory = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();
            tvFolderImageList1 = new System.Windows.Forms.ImageList(components);
            btnArchive = new System.Windows.Forms.Button();
            lblCbMessage = new System.Windows.Forms.Label();
            cbRelateFolders = new System.Windows.Forms.ComboBox();
            btnRefresh = new System.Windows.Forms.Button();
            txtRootPath = new System.Windows.Forms.TextBox();
            tvFolder = new System.Windows.Forms.TreeView();
            tabPage2 = new System.Windows.Forms.TabPage();
            txtInfo = new System.Windows.Forms.TextBox();
            tabPage3 = new System.Windows.Forms.TabPage();
            setArchiveDownloadedFile = new System.Windows.Forms.CheckBox();
            setSaveWebSiteName = new System.Windows.Forms.CheckBox();
            setBackGroundLoad = new System.Windows.Forms.CheckBox();
            gbProxy = new System.Windows.Forms.GroupBox();
            label7 = new System.Windows.Forms.Label();
            setProxyPort = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            setProxyUrl = new System.Windows.Forms.TextBox();
            setUsingProxy = new System.Windows.Forms.CheckBox();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            setLoadAllPicture = new System.Windows.Forms.CheckBox();
            btnSave = new System.Windows.Forms.Button();
            setLocalPath = new System.Windows.Forms.TextBox();
            setWinRARPath = new System.Windows.Forms.TextBox();
            setPhotoProgramPath = new System.Windows.Forms.TextBox();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            statusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            statusMsg2 = new System.Windows.Forms.ToolStripStatusLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            bgWorker = new System.ComponentModel.BackgroundWorker();
            bgWorkMain = new System.ComponentModel.BackgroundWorker();
            fontDialog1 = new System.Windows.Forms.FontDialog();
            exceptMenu = new System.Windows.Forms.ContextMenuStrip(components);
            AddIgnoreComic = new System.Windows.Forms.ToolStripMenuItem();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbIcon).BeginInit();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            gbProxy.SuspendLayout();
            statusStrip1.SuspendLayout();
            exceptMenu.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1184, 679);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(splitContainer1);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(1176, 651);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "基本功能";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            splitContainer1.Panel1.Controls.Add(treeViewnComicName);
            splitContainer1.Panel1.Controls.Add(btnAppendTo);
            splitContainer1.Panel1.Controls.Add(cbComicCatcher);
            splitContainer1.Panel1.Controls.Add(chkArchiveDownloadedFile);
            splitContainer1.Panel1.Controls.Add(btnCollapse);
            splitContainer1.Panel1.Controls.Add(chkLoadPhoto);
            splitContainer1.Panel1.Controls.Add(btnFind);
            splitContainer1.Panel1.Controls.Add(txtFind);
            splitContainer1.Panel1.Controls.Add(pbIcon);
            splitContainer1.Panel1.Controls.Add(lblUpdateChapter);
            splitContainer1.Panel1.Controls.Add(lblUpdateDate);
            splitContainer1.Panel1.Controls.Add(label2);
            splitContainer1.Panel1.Controls.Add(label1);
            splitContainer1.Panel1.Controls.Add(chkIsUseProxy);
            splitContainer1.Panel1.Controls.Add(txtUrl);
            splitContainer1.Panel1.Controls.Add(tvComicTree);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            splitContainer1.Panel2.Controls.Add(btnShowExceptModal);
            splitContainer1.Panel2.Controls.Add(btnShowEditModal);
            splitContainer1.Panel2.Controls.Add(btnOpenDirectory);
            splitContainer1.Panel2.Controls.Add(btnDelete);
            splitContainer1.Panel2.Controls.Add(btnArchive);
            splitContainer1.Panel2.Controls.Add(lblCbMessage);
            splitContainer1.Panel2.Controls.Add(cbRelateFolders);
            splitContainer1.Panel2.Controls.Add(btnRefresh);
            splitContainer1.Panel2.Controls.Add(txtRootPath);
            splitContainer1.Panel2.Controls.Add(tvFolder);
            splitContainer1.Size = new System.Drawing.Size(1170, 645);
            splitContainer1.SplitterDistance = 776;
            splitContainer1.TabIndex = 3;
            splitContainer1.TabStop = false;
            // 
            // treeViewnComicName
            // 
            treeViewnComicName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            treeViewnComicName.BackColor = System.Drawing.SystemColors.InactiveBorder;
            treeViewnComicName.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            treeViewnComicName.HideSelection = false;
            treeViewnComicName.Indent = 5;
            treeViewnComicName.Location = new System.Drawing.Point(292, 36);
            treeViewnComicName.Name = "treeViewnComicName";
            treeViewnComicName.ShowLines = false;
            treeViewnComicName.ShowRootLines = false;
            treeViewnComicName.Size = new System.Drawing.Size(273, 605);
            treeViewnComicName.TabIndex = 20;
            treeViewnComicName.KeyPress += tvComicTree_KeyPress;
            treeViewnComicName.MouseDoubleClick += tvComicTree_MouseDoubleClick;
            // 
            // btnAppendTo
            // 
            btnAppendTo.Location = new System.Drawing.Point(68, 8);
            btnAppendTo.Name = "btnAppendTo";
            btnAppendTo.Size = new System.Drawing.Size(44, 22);
            btnAppendTo.TabIndex = 19;
            btnAppendTo.Text = "添加";
            btnAppendTo.UseVisualStyleBackColor = true;
            btnAppendTo.Click += btnAppendTo_Click;
            // 
            // cbComicCatcher
            // 
            cbComicCatcher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbComicCatcher.FormattingEnabled = true;
            cbComicCatcher.Items.AddRange(new object[] { "dm5", "seemh", "xindm", "" });
            cbComicCatcher.Location = new System.Drawing.Point(3, 9);
            cbComicCatcher.Name = "cbComicCatcher";
            cbComicCatcher.Size = new System.Drawing.Size(63, 23);
            cbComicCatcher.TabIndex = 3;
            cbComicCatcher.SelectedIndexChanged += cbComicCatcher_SelectedIndexChanged;
            // 
            // chkArchiveDownloadedFile
            // 
            chkArchiveDownloadedFile.AutoSize = true;
            chkArchiveDownloadedFile.Checked = true;
            chkArchiveDownloadedFile.CheckState = System.Windows.Forms.CheckState.Checked;
            chkArchiveDownloadedFile.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            chkArchiveDownloadedFile.Location = new System.Drawing.Point(588, 124);
            chkArchiveDownloadedFile.Margin = new System.Windows.Forms.Padding(2);
            chkArchiveDownloadedFile.Name = "chkArchiveDownloadedFile";
            chkArchiveDownloadedFile.Size = new System.Drawing.Size(86, 19);
            chkArchiveDownloadedFile.TabIndex = 10;
            chkArchiveDownloadedFile.Text = "下載後壓縮";
            chkArchiveDownloadedFile.UseVisualStyleBackColor = true;
            chkArchiveDownloadedFile.CheckedChanged += chkArchiveDownloadedFile_CheckedChanged;
            // 
            // btnCollapse
            // 
            btnCollapse.Location = new System.Drawing.Point(685, 97);
            btnCollapse.Name = "btnCollapse";
            btnCollapse.Size = new System.Drawing.Size(88, 23);
            btnCollapse.TabIndex = 9;
            btnCollapse.TabStop = false;
            btnCollapse.Text = "全部收合(C)";
            btnCollapse.UseVisualStyleBackColor = true;
            btnCollapse.Click += btnCollapse_Click;
            // 
            // chkLoadPhoto
            // 
            chkLoadPhoto.AutoSize = true;
            chkLoadPhoto.Checked = true;
            chkLoadPhoto.CheckState = System.Windows.Forms.CheckState.Checked;
            chkLoadPhoto.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            chkLoadPhoto.Location = new System.Drawing.Point(588, 84);
            chkLoadPhoto.Margin = new System.Windows.Forms.Padding(2);
            chkLoadPhoto.Name = "chkLoadPhoto";
            chkLoadPhoto.Size = new System.Drawing.Size(146, 19);
            chkLoadPhoto.TabIndex = 7;
            chkLoadPhoto.Text = "展開頁時載入全部縮圖";
            chkLoadPhoto.UseVisualStyleBackColor = true;
            // 
            // btnFind
            // 
            btnFind.Location = new System.Drawing.Point(725, 36);
            btnFind.Name = "btnFind";
            btnFind.Size = new System.Drawing.Size(48, 23);
            btnFind.TabIndex = 6;
            btnFind.Text = "尋找";
            btnFind.UseVisualStyleBackColor = true;
            btnFind.Click += btnFind_Click;
            // 
            // txtFind
            // 
            txtFind.Location = new System.Drawing.Point(588, 37);
            txtFind.Name = "txtFind";
            txtFind.Size = new System.Drawing.Size(133, 23);
            txtFind.TabIndex = 5;
            txtFind.KeyPress += txtFind_KeyPress;
            // 
            // pbIcon
            // 
            pbIcon.BackColor = System.Drawing.Color.SaddleBrown;
            pbIcon.Location = new System.Drawing.Point(573, 216);
            pbIcon.Margin = new System.Windows.Forms.Padding(5);
            pbIcon.Name = "pbIcon";
            pbIcon.Padding = new System.Windows.Forms.Padding(2);
            pbIcon.Size = new System.Drawing.Size(200, 290);
            pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pbIcon.TabIndex = 17;
            pbIcon.TabStop = false;
            pbIcon.Visible = false;
            pbIcon.Paint += pbIcon_Paint;
            // 
            // lblUpdateChapter
            // 
            lblUpdateChapter.AutoSize = true;
            lblUpdateChapter.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblUpdateChapter.ForeColor = System.Drawing.Color.Blue;
            lblUpdateChapter.Location = new System.Drawing.Point(584, 195);
            lblUpdateChapter.Name = "lblUpdateChapter";
            lblUpdateChapter.Size = new System.Drawing.Size(117, 16);
            lblUpdateChapter.TabIndex = 14;
            lblUpdateChapter.Text = "lblUpdateChapter";
            // 
            // lblUpdateDate
            // 
            lblUpdateDate.AutoSize = true;
            lblUpdateDate.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblUpdateDate.ForeColor = System.Drawing.Color.Red;
            lblUpdateDate.Location = new System.Drawing.Point(585, 159);
            lblUpdateDate.Name = "lblUpdateDate";
            lblUpdateDate.Size = new System.Drawing.Size(97, 16);
            lblUpdateDate.TabIndex = 12;
            lblUpdateDate.Text = "lblUpdateDate";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(585, 180);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(65, 12);
            label2.TabIndex = 13;
            label2.Text = "更新回數：";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(586, 142);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(65, 12);
            label1.TabIndex = 11;
            label1.Text = "更新日期：";
            // 
            // chkIsUseProxy
            // 
            chkIsUseProxy.AutoSize = true;
            chkIsUseProxy.Checked = true;
            chkIsUseProxy.CheckState = System.Windows.Forms.CheckState.Checked;
            chkIsUseProxy.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            chkIsUseProxy.Location = new System.Drawing.Point(588, 104);
            chkIsUseProxy.Margin = new System.Windows.Forms.Padding(2);
            chkIsUseProxy.Name = "chkIsUseProxy";
            chkIsUseProxy.Size = new System.Drawing.Size(57, 19);
            chkIsUseProxy.TabIndex = 8;
            chkIsUseProxy.Text = "Proxy";
            chkIsUseProxy.UseVisualStyleBackColor = true;
            chkIsUseProxy.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // txtUrl
            // 
            txtUrl.Location = new System.Drawing.Point(115, 8);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new System.Drawing.Size(658, 23);
            txtUrl.TabIndex = 1;
            // 
            // tvComicTree
            // 
            tvComicTree.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            tvComicTree.BackColor = System.Drawing.SystemColors.InactiveBorder;
            tvComicTree.Font = new System.Drawing.Font("新細明體", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            tvComicTree.HideSelection = false;
            tvComicTree.Location = new System.Drawing.Point(4, 36);
            tvComicTree.Name = "tvComicTree";
            treeNode1.Name = "http://www.xindm.cn";
            treeNode1.Text = "新動漫(xindm)";
            tvComicTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { treeNode1 });
            tvComicTree.ShowRootLines = false;
            tvComicTree.Size = new System.Drawing.Size(282, 605);
            tvComicTree.TabIndex = 4;
            tvComicTree.DrawNode += tvComicTree_DrawNode;
            tvComicTree.AfterSelect += tvComicTree_AfterSelect;
            tvComicTree.NodeMouseClick += tvComicTree_NodeMouseClick;
            tvComicTree.KeyPress += tvComicTree_KeyPress;
            tvComicTree.KeyUp += tvComicTree_KeyUp;
            tvComicTree.MouseDoubleClick += tvComicTree_MouseDoubleClick;
            // 
            // btnShowExceptModal
            // 
            btnShowExceptModal.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnShowExceptModal.Location = new System.Drawing.Point(253, 57);
            btnShowExceptModal.Name = "btnShowExceptModal";
            btnShowExceptModal.Size = new System.Drawing.Size(82, 29);
            btnShowExceptModal.TabIndex = 19;
            btnShowExceptModal.TabStop = false;
            btnShowExceptModal.Text = "編輯例外";
            btnShowExceptModal.UseVisualStyleBackColor = true;
            btnShowExceptModal.Click += btnShowExceptModal_Click;
            // 
            // btnShowEditModal
            // 
            btnShowEditModal.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnShowEditModal.Location = new System.Drawing.Point(165, 57);
            btnShowEditModal.Name = "btnShowEditModal";
            btnShowEditModal.Size = new System.Drawing.Size(82, 29);
            btnShowEditModal.TabIndex = 17;
            btnShowEditModal.TabStop = false;
            btnShowEditModal.Text = "編輯群組";
            btnShowEditModal.UseVisualStyleBackColor = true;
            btnShowEditModal.Click += btnShowEditModal_Click;
            // 
            // btnOpenDirectory
            // 
            btnOpenDirectory.Font = new System.Drawing.Font("新細明體", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnOpenDirectory.Location = new System.Drawing.Point(306, 200);
            btnOpenDirectory.Name = "btnOpenDirectory";
            btnOpenDirectory.Size = new System.Drawing.Size(29, 102);
            btnOpenDirectory.TabIndex = 7;
            btnOpenDirectory.Text = "開啟目錄";
            btnOpenDirectory.UseVisualStyleBackColor = true;
            btnOpenDirectory.Click += OpenDirectory_Click;
            // 
            // btnDelete
            // 
            btnDelete.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnDelete.ImageList = tvFolderImageList1;
            btnDelete.Location = new System.Drawing.Point(306, 106);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(29, 88);
            btnDelete.TabIndex = 6;
            btnDelete.Text = "刪除";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // tvFolderImageList1
            // 
            tvFolderImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            tvFolderImageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("tvFolderImageList1.ImageStream");
            tvFolderImageList1.TransparentColor = System.Drawing.Color.Transparent;
            tvFolderImageList1.Images.SetKeyName(0, "my_computer2.png");
            tvFolderImageList1.Images.SetKeyName(1, "rar");
            tvFolderImageList1.Images.SetKeyName(2, "dir");
            tvFolderImageList1.Images.SetKeyName(3, "books.png");
            tvFolderImageList1.Images.SetKeyName(4, "book.png");
            tvFolderImageList1.Images.SetKeyName(5, "chapter.png");
            tvFolderImageList1.Images.SetKeyName(6, "book2.png");
            // 
            // btnArchive
            // 
            btnArchive.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnArchive.Location = new System.Drawing.Point(84, 57);
            btnArchive.Name = "btnArchive";
            btnArchive.Size = new System.Drawing.Size(75, 29);
            btnArchive.TabIndex = 4;
            btnArchive.TabStop = false;
            btnArchive.Text = "壓縮(&A)";
            btnArchive.UseVisualStyleBackColor = true;
            btnArchive.Click += btnArchive_Click;
            // 
            // lblCbMessage
            // 
            lblCbMessage.AutoSize = true;
            lblCbMessage.Font = new System.Drawing.Font("新細明體", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblCbMessage.ForeColor = System.Drawing.Color.Blue;
            lblCbMessage.Location = new System.Drawing.Point(3, 89);
            lblCbMessage.Name = "lblCbMessage";
            lblCbMessage.Size = new System.Drawing.Size(85, 14);
            lblCbMessage.TabIndex = 16;
            lblCbMessage.Text = "lblCbMessage";
            // 
            // cbRelateFolders
            // 
            cbRelateFolders.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            cbRelateFolders.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            cbRelateFolders.BackColor = System.Drawing.Color.Lavender;
            cbRelateFolders.Dock = System.Windows.Forms.DockStyle.Top;
            cbRelateFolders.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            cbRelateFolders.FormattingEnabled = true;
            cbRelateFolders.Location = new System.Drawing.Point(0, 27);
            cbRelateFolders.Name = "cbRelateFolders";
            cbRelateFolders.Size = new System.Drawing.Size(390, 24);
            cbRelateFolders.TabIndex = 2;
            cbRelateFolders.SelectedIndexChanged += cbFolders_SelectedIndexChanged;
            cbRelateFolders.KeyUp += cbRelateFolders_KeyUp;
            // 
            // btnRefresh
            // 
            btnRefresh.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnRefresh.Location = new System.Drawing.Point(3, 57);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(75, 29);
            btnRefresh.TabIndex = 3;
            btnRefresh.TabStop = false;
            btnRefresh.Text = "更新(&R)";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // txtRootPath
            // 
            txtRootPath.BackColor = System.Drawing.Color.Honeydew;
            txtRootPath.Dock = System.Windows.Forms.DockStyle.Top;
            txtRootPath.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtRootPath.Location = new System.Drawing.Point(0, 0);
            txtRootPath.Name = "txtRootPath";
            txtRootPath.Size = new System.Drawing.Size(390, 27);
            txtRootPath.TabIndex = 1;
            txtRootPath.Text = "Q:\\Comic\\ComicShelf";
            txtRootPath.KeyPress += txtPath_KeyPress;
            // 
            // tvFolder
            // 
            tvFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            tvFolder.BackColor = System.Drawing.Color.Honeydew;
            tvFolder.ImageIndex = 0;
            tvFolder.ImageList = tvFolderImageList1;
            tvFolder.Location = new System.Drawing.Point(3, 106);
            tvFolder.Name = "tvFolder";
            tvFolder.SelectedImageIndex = 0;
            tvFolder.Size = new System.Drawing.Size(300, 528);
            tvFolder.TabIndex = 5;
            tvFolder.KeyPress += tvFolder_KeyPress;
            tvFolder.MouseDoubleClick += tvFolder_MouseDoubleClick;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(txtInfo);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(1176, 651);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "訊息";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtInfo
            // 
            txtInfo.BackColor = System.Drawing.SystemColors.Info;
            txtInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            txtInfo.Location = new System.Drawing.Point(3, 3);
            txtInfo.Multiline = true;
            txtInfo.Name = "txtInfo";
            txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtInfo.Size = new System.Drawing.Size(1170, 645);
            txtInfo.TabIndex = 7;
            txtInfo.Text = resources.GetString("txtInfo.Text");
            txtInfo.TextChanged += txtInfo_TextChanged;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(setArchiveDownloadedFile);
            tabPage3.Controls.Add(setSaveWebSiteName);
            tabPage3.Controls.Add(setBackGroundLoad);
            tabPage3.Controls.Add(gbProxy);
            tabPage3.Controls.Add(label5);
            tabPage3.Controls.Add(label4);
            tabPage3.Controls.Add(label3);
            tabPage3.Controls.Add(setLoadAllPicture);
            tabPage3.Controls.Add(btnSave);
            tabPage3.Controls.Add(setLocalPath);
            tabPage3.Controls.Add(setWinRARPath);
            tabPage3.Controls.Add(setPhotoProgramPath);
            tabPage3.Location = new System.Drawing.Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new System.Drawing.Size(1176, 651);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "設定資料";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // setArchiveDownloadedFile
            // 
            setArchiveDownloadedFile.AutoSize = true;
            setArchiveDownloadedFile.Checked = true;
            setArchiveDownloadedFile.CheckState = System.Windows.Forms.CheckState.Checked;
            setArchiveDownloadedFile.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            setArchiveDownloadedFile.Location = new System.Drawing.Point(313, 224);
            setArchiveDownloadedFile.Margin = new System.Windows.Forms.Padding(2);
            setArchiveDownloadedFile.Name = "setArchiveDownloadedFile";
            setArchiveDownloadedFile.Size = new System.Drawing.Size(86, 19);
            setArchiveDownloadedFile.TabIndex = 33;
            setArchiveDownloadedFile.Text = "下載後壓縮";
            setArchiveDownloadedFile.UseVisualStyleBackColor = true;
            // 
            // setSaveWebSiteName
            // 
            setSaveWebSiteName.AutoSize = true;
            setSaveWebSiteName.Checked = true;
            setSaveWebSiteName.CheckState = System.Windows.Forms.CheckState.Checked;
            setSaveWebSiteName.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            setSaveWebSiteName.Location = new System.Drawing.Point(32, 224);
            setSaveWebSiteName.Margin = new System.Windows.Forms.Padding(2);
            setSaveWebSiteName.Name = "setSaveWebSiteName";
            setSaveWebSiteName.Size = new System.Drawing.Size(122, 19);
            setSaveWebSiteName.TabIndex = 32;
            setSaveWebSiteName.Text = "存檔目錄加上前綴";
            setSaveWebSiteName.UseVisualStyleBackColor = true;
            // 
            // setBackGroundLoad
            // 
            setBackGroundLoad.AutoSize = true;
            setBackGroundLoad.Checked = true;
            setBackGroundLoad.CheckState = System.Windows.Forms.CheckState.Checked;
            setBackGroundLoad.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            setBackGroundLoad.Location = new System.Drawing.Point(32, 275);
            setBackGroundLoad.Margin = new System.Windows.Forms.Padding(2);
            setBackGroundLoad.Name = "setBackGroundLoad";
            setBackGroundLoad.Size = new System.Drawing.Size(98, 19);
            setBackGroundLoad.TabIndex = 31;
            setBackGroundLoad.Text = "自動載入資訊";
            setBackGroundLoad.UseVisualStyleBackColor = true;
            // 
            // gbProxy
            // 
            gbProxy.Controls.Add(label7);
            gbProxy.Controls.Add(setProxyPort);
            gbProxy.Controls.Add(label6);
            gbProxy.Controls.Add(setProxyUrl);
            gbProxy.Controls.Add(setUsingProxy);
            gbProxy.Location = new System.Drawing.Point(32, 306);
            gbProxy.Name = "gbProxy";
            gbProxy.Size = new System.Drawing.Size(244, 121);
            gbProxy.TabIndex = 30;
            gbProxy.TabStop = false;
            gbProxy.Text = "Proxy設定";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(16, 82);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(61, 15);
            label7.TabIndex = 34;
            label7.Text = "ProxyPort";
            // 
            // setProxyPort
            // 
            setProxyPort.Location = new System.Drawing.Point(76, 77);
            setProxyPort.Name = "setProxyPort";
            setProxyPort.Size = new System.Drawing.Size(69, 23);
            setProxyPort.TabIndex = 32;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(16, 54);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(61, 15);
            label6.TabIndex = 33;
            label6.Text = "ProxyURL";
            // 
            // setProxyUrl
            // 
            setProxyUrl.Location = new System.Drawing.Point(76, 49);
            setProxyUrl.Name = "setProxyUrl";
            setProxyUrl.Size = new System.Drawing.Size(155, 23);
            setProxyUrl.TabIndex = 31;
            // 
            // setUsingProxy
            // 
            setUsingProxy.AutoSize = true;
            setUsingProxy.Checked = true;
            setUsingProxy.CheckState = System.Windows.Forms.CheckState.Checked;
            setUsingProxy.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            setUsingProxy.Location = new System.Drawing.Point(16, 27);
            setUsingProxy.Margin = new System.Windows.Forms.Padding(2);
            setUsingProxy.Name = "setUsingProxy";
            setUsingProxy.Size = new System.Drawing.Size(81, 19);
            setUsingProxy.TabIndex = 30;
            setUsingProxy.Text = "使用Proxy";
            setUsingProxy.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(32, 189);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(79, 15);
            label5.TabIndex = 25;
            label5.Text = "本地圖片路徑";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(32, 137);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(77, 15);
            label4.TabIndex = 24;
            label4.Text = "WinRAR路徑";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(32, 85);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(79, 15);
            label3.TabIndex = 23;
            label3.Text = "看圖軟體路徑";
            // 
            // setLoadAllPicture
            // 
            setLoadAllPicture.AutoSize = true;
            setLoadAllPicture.Checked = true;
            setLoadAllPicture.CheckState = System.Windows.Forms.CheckState.Checked;
            setLoadAllPicture.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            setLoadAllPicture.Location = new System.Drawing.Point(32, 250);
            setLoadAllPicture.Margin = new System.Windows.Forms.Padding(2);
            setLoadAllPicture.Name = "setLoadAllPicture";
            setLoadAllPicture.Size = new System.Drawing.Size(146, 19);
            setLoadAllPicture.TabIndex = 4;
            setLoadAllPicture.Text = "展開頁時載入全部縮圖";
            setLoadAllPicture.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(32, 21);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(92, 23);
            btnSave.TabIndex = 0;
            btnSave.Text = "儲存設定(&S)";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // setLocalPath
            // 
            setLocalPath.Location = new System.Drawing.Point(115, 184);
            setLocalPath.Name = "setLocalPath";
            setLocalPath.Size = new System.Drawing.Size(819, 23);
            setLocalPath.TabIndex = 3;
            // 
            // setWinRARPath
            // 
            setWinRARPath.Location = new System.Drawing.Point(115, 132);
            setWinRARPath.Name = "setWinRARPath";
            setWinRARPath.Size = new System.Drawing.Size(819, 23);
            setWinRARPath.TabIndex = 2;
            // 
            // setPhotoProgramPath
            // 
            setPhotoProgramPath.Location = new System.Drawing.Point(115, 80);
            setPhotoProgramPath.Name = "setPhotoProgramPath";
            setPhotoProgramPath.Size = new System.Drawing.Size(819, 23);
            setPhotoProgramPath.TabIndex = 1;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { statusMsg, statusMsg2 });
            statusStrip1.Location = new System.Drawing.Point(0, 682);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(1184, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // statusMsg
            // 
            statusMsg.ForeColor = System.Drawing.Color.Blue;
            statusMsg.Name = "statusMsg";
            statusMsg.Size = new System.Drawing.Size(15, 17);
            statusMsg.Text = "[]";
            // 
            // statusMsg2
            // 
            statusMsg2.Name = "statusMsg2";
            statusMsg2.Size = new System.Drawing.Size(15, 17);
            statusMsg2.Text = "[]";
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 800;
            timer1.Tick += timer1_Tick;
            // 
            // bgWorker
            // 
            bgWorker.WorkerReportsProgress = true;
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.ProgressChanged += bgWorker_ProgressChanged;
            // 
            // exceptMenu
            // 
            exceptMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            exceptMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { AddIgnoreComic });
            exceptMenu.Name = "exceptMenu";
            exceptMenu.Size = new System.Drawing.Size(191, 26);
            // 
            // AddIgnoreComic
            // 
            AddIgnoreComic.Name = "AddIgnoreComic";
            AddIgnoreComic.Size = new System.Drawing.Size(190, 22);
            AddIgnoreComic.Text = "加入例外網頁(不顯示)";
            AddIgnoreComic.Click += AddIgnoreComic_Click;
            // 
            // frmMain
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            ClientSize = new System.Drawing.Size(1184, 704);
            Controls.Add(statusStrip1);
            Controls.Add(tabControl1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "frmMain";
            Text = "漫畫下載器 2018/02/18 0.0.4.002";
            Load += frmMain_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbIcon).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            gbProxy.ResumeLayout(false);
            gbProxy.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            exceptMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TreeView tvComicTree;
        private System.Windows.Forms.TextBox txtRootPath;
        private System.Windows.Forms.TreeView tvFolder;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.ComboBox cbRelateFolders;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusMsg;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.ToolStripStatusLabel statusMsg2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox chkIsUseProxy;
        private System.ComponentModel.BackgroundWorker bgWorkMain;
        private System.Windows.Forms.Label lblUpdateChapter;
        private System.Windows.Forms.Label lblUpdateDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.ImageList tvFolderImageList1;
        private System.Windows.Forms.Label lblCbMessage;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.CheckBox chkLoadPhoto;
        private System.Windows.Forms.Button btnCollapse;
        private System.Windows.Forms.Button btnArchive;
        private System.Windows.Forms.TextBox setPhotoProgramPath;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox setLocalPath;
        private System.Windows.Forms.TextBox setWinRARPath;
        private System.Windows.Forms.CheckBox setLoadAllPicture;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox gbProxy;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox setProxyPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox setProxyUrl;
        private System.Windows.Forms.CheckBox setUsingProxy;
        private System.Windows.Forms.CheckBox chkArchiveDownloadedFile;
        private System.Windows.Forms.Button btnOpenDirectory;
        private System.Windows.Forms.CheckBox setBackGroundLoad;
        private System.Windows.Forms.ComboBox cbComicCatcher;
        private System.Windows.Forms.CheckBox setSaveWebSiteName;
        private System.Windows.Forms.Button btnShowEditModal;
        private System.Windows.Forms.Button btnAppendTo;
        private System.Windows.Forms.Button btnShowExceptModal;
        private System.Windows.Forms.ContextMenuStrip exceptMenu;
        private System.Windows.Forms.ToolStripMenuItem AddIgnoreComic;
        private System.Windows.Forms.CheckBox setArchiveDownloadedFile;
        private System.Windows.Forms.TreeView treeViewnComicName;
    }
}


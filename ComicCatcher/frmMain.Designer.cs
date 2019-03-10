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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("新動漫(xindm)");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewnComicName = new System.Windows.Forms.TreeView();
            this.btnAppendTo = new System.Windows.Forms.Button();
            this.chkSaveWebSiteName = new System.Windows.Forms.CheckBox();
            this.cbComicCatcher = new System.Windows.Forms.ComboBox();
            this.chkArchiveDownloadedFile = new System.Windows.Forms.CheckBox();
            this.btnCollapse = new System.Windows.Forms.Button();
            this.chkLoadPhoto = new System.Windows.Forms.CheckBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.lblUpdateChapter = new System.Windows.Forms.Label();
            this.lblUpdateDate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkIsUseProxy = new System.Windows.Forms.CheckBox();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.tvComicTree = new System.Windows.Forms.TreeView();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnShowExceptModal = new System.Windows.Forms.Button();
            this.btnShowEditModal = new System.Windows.Forms.Button();
            this.btnOpenDirectory = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.tvFolderImageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnArchive = new System.Windows.Forms.Button();
            this.lblCbMessage = new System.Windows.Forms.Label();
            this.cbRelateFolders = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.txtRootPath = new System.Windows.Forms.TextBox();
            this.tvFolder = new System.Windows.Forms.TreeView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.setArchiveDownloadedFile = new System.Windows.Forms.CheckBox();
            this.setSaveWebSiteName = new System.Windows.Forms.CheckBox();
            this.setBackGroundLoad = new System.Windows.Forms.CheckBox();
            this.gbProxy = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.setProxyPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.setProxyUrl = new System.Windows.Forms.TextBox();
            this.setUsingProxy = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.setLoadAllPicture = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.setLocalPath = new System.Windows.Forms.TextBox();
            this.setWinRARPath = new System.Windows.Forms.TextBox();
            this.setPhotoProgramPath = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusMsg2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.bgWorkMain = new System.ComponentModel.BackgroundWorker();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.exceptMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddIgnoreComic = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.gbProxy.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.exceptMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1184, 679);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1176, 647);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "基本功能";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.splitContainer1.Panel1.Controls.Add(this.treeViewnComicName);
            this.splitContainer1.Panel1.Controls.Add(this.btnAppendTo);
            this.splitContainer1.Panel1.Controls.Add(this.chkSaveWebSiteName);
            this.splitContainer1.Panel1.Controls.Add(this.cbComicCatcher);
            this.splitContainer1.Panel1.Controls.Add(this.chkArchiveDownloadedFile);
            this.splitContainer1.Panel1.Controls.Add(this.btnCollapse);
            this.splitContainer1.Panel1.Controls.Add(this.chkLoadPhoto);
            this.splitContainer1.Panel1.Controls.Add(this.btnFind);
            this.splitContainer1.Panel1.Controls.Add(this.txtFind);
            this.splitContainer1.Panel1.Controls.Add(this.pbIcon);
            this.splitContainer1.Panel1.Controls.Add(this.lblUpdateChapter);
            this.splitContainer1.Panel1.Controls.Add(this.lblUpdateDate);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.chkIsUseProxy);
            this.splitContainer1.Panel1.Controls.Add(this.txtUrl);
            this.splitContainer1.Panel1.Controls.Add(this.tvComicTree);
            this.splitContainer1.Panel1.Controls.Add(this.btnDownload);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.splitContainer1.Panel2.Controls.Add(this.btnShowExceptModal);
            this.splitContainer1.Panel2.Controls.Add(this.btnShowEditModal);
            this.splitContainer1.Panel2.Controls.Add(this.btnOpenDirectory);
            this.splitContainer1.Panel2.Controls.Add(this.btnDelete);
            this.splitContainer1.Panel2.Controls.Add(this.btnArchive);
            this.splitContainer1.Panel2.Controls.Add(this.lblCbMessage);
            this.splitContainer1.Panel2.Controls.Add(this.cbRelateFolders);
            this.splitContainer1.Panel2.Controls.Add(this.btnRefresh);
            this.splitContainer1.Panel2.Controls.Add(this.txtRootPath);
            this.splitContainer1.Panel2.Controls.Add(this.tvFolder);
            this.splitContainer1.Size = new System.Drawing.Size(1170, 641);
            this.splitContainer1.SplitterDistance = 776;
            this.splitContainer1.TabIndex = 3;
            this.splitContainer1.TabStop = false;
            // 
            // treeViewnComicName
            // 
            this.treeViewnComicName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewnComicName.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.treeViewnComicName.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.treeViewnComicName.HideSelection = false;
            this.treeViewnComicName.Indent = 5;
            this.treeViewnComicName.Location = new System.Drawing.Point(292, 36);
            this.treeViewnComicName.Name = "treeViewnComicName";
            this.treeViewnComicName.ShowLines = false;
            this.treeViewnComicName.ShowRootLines = false;
            this.treeViewnComicName.Size = new System.Drawing.Size(273, 601);
            this.treeViewnComicName.TabIndex = 20;
            this.treeViewnComicName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tvComicTree_KeyPress);
            this.treeViewnComicName.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tvComicTree_MouseDoubleClick);
            // 
            // btnAppendTo
            // 
            this.btnAppendTo.Location = new System.Drawing.Point(68, 8);
            this.btnAppendTo.Name = "btnAppendTo";
            this.btnAppendTo.Size = new System.Drawing.Size(44, 22);
            this.btnAppendTo.TabIndex = 19;
            this.btnAppendTo.Text = "添加";
            this.btnAppendTo.UseVisualStyleBackColor = true;
            this.btnAppendTo.Click += new System.EventHandler(this.btnAppendTo_Click);
            // 
            // chkSaveWebSiteName
            // 
            this.chkSaveWebSiteName.AutoSize = true;
            this.chkSaveWebSiteName.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.chkSaveWebSiteName.Location = new System.Drawing.Point(588, 64);
            this.chkSaveWebSiteName.Margin = new System.Windows.Forms.Padding(2);
            this.chkSaveWebSiteName.Name = "chkSaveWebSiteName";
            this.chkSaveWebSiteName.Size = new System.Drawing.Size(178, 22);
            this.chkSaveWebSiteName.TabIndex = 18;
            this.chkSaveWebSiteName.Text = "存檔目錄加上前綴";
            this.chkSaveWebSiteName.UseVisualStyleBackColor = true;
            // 
            // cbComicCatcher
            // 
            this.cbComicCatcher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbComicCatcher.FormattingEnabled = true;
            this.cbComicCatcher.Items.AddRange(new object[] {
            "dm5",
            "seemh",
            "xindm",
            ""});
            this.cbComicCatcher.Location = new System.Drawing.Point(3, 9);
            this.cbComicCatcher.Name = "cbComicCatcher";
            this.cbComicCatcher.Size = new System.Drawing.Size(63, 26);
            this.cbComicCatcher.TabIndex = 3;
            this.cbComicCatcher.SelectedIndexChanged += new System.EventHandler(this.cbComicCatcher_SelectedIndexChanged);
            // 
            // chkArchiveDownloadedFile
            // 
            this.chkArchiveDownloadedFile.AutoSize = true;
            this.chkArchiveDownloadedFile.Checked = true;
            this.chkArchiveDownloadedFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkArchiveDownloadedFile.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.chkArchiveDownloadedFile.Location = new System.Drawing.Point(588, 124);
            this.chkArchiveDownloadedFile.Margin = new System.Windows.Forms.Padding(2);
            this.chkArchiveDownloadedFile.Name = "chkArchiveDownloadedFile";
            this.chkArchiveDownloadedFile.Size = new System.Drawing.Size(124, 22);
            this.chkArchiveDownloadedFile.TabIndex = 10;
            this.chkArchiveDownloadedFile.Text = "下載後壓縮";
            this.chkArchiveDownloadedFile.UseVisualStyleBackColor = true;
            this.chkArchiveDownloadedFile.CheckedChanged += new System.EventHandler(this.chkArchiveDownloadedFile_CheckedChanged);
            // 
            // btnCollapse
            // 
            this.btnCollapse.Location = new System.Drawing.Point(685, 97);
            this.btnCollapse.Name = "btnCollapse";
            this.btnCollapse.Size = new System.Drawing.Size(88, 23);
            this.btnCollapse.TabIndex = 9;
            this.btnCollapse.TabStop = false;
            this.btnCollapse.Text = "全部收合(C)";
            this.btnCollapse.UseVisualStyleBackColor = true;
            this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
            // 
            // chkLoadPhoto
            // 
            this.chkLoadPhoto.AutoSize = true;
            this.chkLoadPhoto.Checked = true;
            this.chkLoadPhoto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLoadPhoto.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.chkLoadPhoto.Location = new System.Drawing.Point(588, 84);
            this.chkLoadPhoto.Margin = new System.Windows.Forms.Padding(2);
            this.chkLoadPhoto.Name = "chkLoadPhoto";
            this.chkLoadPhoto.Size = new System.Drawing.Size(214, 22);
            this.chkLoadPhoto.TabIndex = 7;
            this.chkLoadPhoto.Text = "展開頁時載入全部縮圖";
            this.chkLoadPhoto.UseVisualStyleBackColor = true;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(725, 36);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(48, 23);
            this.btnFind.TabIndex = 6;
            this.btnFind.Text = "尋找";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(588, 37);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(133, 29);
            this.txtFind.TabIndex = 5;
            this.txtFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFind_KeyPress);
            // 
            // pbIcon
            // 
            this.pbIcon.BackColor = System.Drawing.Color.SaddleBrown;
            this.pbIcon.Location = new System.Drawing.Point(573, 216);
            this.pbIcon.Margin = new System.Windows.Forms.Padding(5);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Padding = new System.Windows.Forms.Padding(2);
            this.pbIcon.Size = new System.Drawing.Size(200, 290);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIcon.TabIndex = 17;
            this.pbIcon.TabStop = false;
            this.pbIcon.Visible = false;
            this.pbIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.pbIcon_Paint);
            // 
            // lblUpdateChapter
            // 
            this.lblUpdateChapter.AutoSize = true;
            this.lblUpdateChapter.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblUpdateChapter.ForeColor = System.Drawing.Color.Blue;
            this.lblUpdateChapter.Location = new System.Drawing.Point(584, 195);
            this.lblUpdateChapter.Name = "lblUpdateChapter";
            this.lblUpdateChapter.Size = new System.Drawing.Size(168, 24);
            this.lblUpdateChapter.TabIndex = 14;
            this.lblUpdateChapter.Text = "lblUpdateChapter";
            // 
            // lblUpdateDate
            // 
            this.lblUpdateDate.AutoSize = true;
            this.lblUpdateDate.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblUpdateDate.ForeColor = System.Drawing.Color.Red;
            this.lblUpdateDate.Location = new System.Drawing.Point(585, 159);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(139, 24);
            this.lblUpdateDate.TabIndex = 12;
            this.lblUpdateDate.Text = "lblUpdateDate";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(585, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 18);
            this.label2.TabIndex = 13;
            this.label2.Text = "更新回數：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(586, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 11;
            this.label1.Text = "更新日期：";
            // 
            // chkIsUseProxy
            // 
            this.chkIsUseProxy.AutoSize = true;
            this.chkIsUseProxy.Checked = true;
            this.chkIsUseProxy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsUseProxy.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.chkIsUseProxy.Location = new System.Drawing.Point(588, 104);
            this.chkIsUseProxy.Margin = new System.Windows.Forms.Padding(2);
            this.chkIsUseProxy.Name = "chkIsUseProxy";
            this.chkIsUseProxy.Size = new System.Drawing.Size(73, 22);
            this.chkIsUseProxy.TabIndex = 8;
            this.chkIsUseProxy.Text = "Proxy";
            this.chkIsUseProxy.UseVisualStyleBackColor = true;
            this.chkIsUseProxy.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(115, 8);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(658, 29);
            this.txtUrl.TabIndex = 1;
            // 
            // tvComicTree
            // 
            this.tvComicTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvComicTree.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.tvComicTree.Font = new System.Drawing.Font("新細明體", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tvComicTree.HideSelection = false;
            this.tvComicTree.Location = new System.Drawing.Point(4, 36);
            this.tvComicTree.Name = "tvComicTree";
            treeNode1.Name = "http://www.xindm.cn";
            treeNode1.Text = "新動漫(xindm)";
            this.tvComicTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvComicTree.ShowRootLines = false;
            this.tvComicTree.Size = new System.Drawing.Size(282, 601);
            this.tvComicTree.TabIndex = 4;
            this.tvComicTree.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.tvComicTree_DrawNode);
            this.tvComicTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvComicTree_AfterSelect);
            this.tvComicTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvComicTree_NodeMouseClick);
            this.tvComicTree.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tvComicTree_KeyPress);
            this.tvComicTree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvComicTree_KeyUp);
            this.tvComicTree.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tvComicTree_MouseDoubleClick);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(573, 514);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.TabStop = false;
            this.btnDownload.Text = "下載";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Visible = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnShowExceptModal
            // 
            this.btnShowExceptModal.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnShowExceptModal.Location = new System.Drawing.Point(253, 57);
            this.btnShowExceptModal.Name = "btnShowExceptModal";
            this.btnShowExceptModal.Size = new System.Drawing.Size(82, 29);
            this.btnShowExceptModal.TabIndex = 19;
            this.btnShowExceptModal.TabStop = false;
            this.btnShowExceptModal.Text = "編輯例外";
            this.btnShowExceptModal.UseVisualStyleBackColor = true;
            this.btnShowExceptModal.Click += new System.EventHandler(this.btnShowExceptModal_Click);
            // 
            // btnShowEditModal
            // 
            this.btnShowEditModal.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnShowEditModal.Location = new System.Drawing.Point(165, 57);
            this.btnShowEditModal.Name = "btnShowEditModal";
            this.btnShowEditModal.Size = new System.Drawing.Size(82, 29);
            this.btnShowEditModal.TabIndex = 17;
            this.btnShowEditModal.TabStop = false;
            this.btnShowEditModal.Text = "編輯群組";
            this.btnShowEditModal.UseVisualStyleBackColor = true;
            this.btnShowEditModal.Click += new System.EventHandler(this.btnShowEditModal_Click);
            // 
            // btnOpenDirectory
            // 
            this.btnOpenDirectory.Font = new System.Drawing.Font("新細明體", 14F);
            this.btnOpenDirectory.Location = new System.Drawing.Point(306, 200);
            this.btnOpenDirectory.Name = "btnOpenDirectory";
            this.btnOpenDirectory.Size = new System.Drawing.Size(29, 102);
            this.btnOpenDirectory.TabIndex = 7;
            this.btnOpenDirectory.Text = "開啟目錄";
            this.btnOpenDirectory.UseVisualStyleBackColor = true;
            this.btnOpenDirectory.Click += new System.EventHandler(this.OpenDirectory_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnDelete.ImageList = this.tvFolderImageList1;
            this.btnDelete.Location = new System.Drawing.Point(306, 106);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(29, 88);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "刪除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // tvFolderImageList1
            // 
            this.tvFolderImageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tvFolderImageList1.ImageStream")));
            this.tvFolderImageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.tvFolderImageList1.Images.SetKeyName(0, "my_computer2.png");
            this.tvFolderImageList1.Images.SetKeyName(1, "rar");
            this.tvFolderImageList1.Images.SetKeyName(2, "dir");
            this.tvFolderImageList1.Images.SetKeyName(3, "books.png");
            this.tvFolderImageList1.Images.SetKeyName(4, "book.png");
            this.tvFolderImageList1.Images.SetKeyName(5, "chapter.png");
            this.tvFolderImageList1.Images.SetKeyName(6, "book2.png");
            // 
            // btnArchive
            // 
            this.btnArchive.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnArchive.Location = new System.Drawing.Point(84, 57);
            this.btnArchive.Name = "btnArchive";
            this.btnArchive.Size = new System.Drawing.Size(75, 29);
            this.btnArchive.TabIndex = 4;
            this.btnArchive.TabStop = false;
            this.btnArchive.Text = "壓縮(&A)";
            this.btnArchive.UseVisualStyleBackColor = true;
            this.btnArchive.Click += new System.EventHandler(this.btnArchive_Click);
            // 
            // lblCbMessage
            // 
            this.lblCbMessage.AutoSize = true;
            this.lblCbMessage.Font = new System.Drawing.Font("新細明體", 10F);
            this.lblCbMessage.ForeColor = System.Drawing.Color.Blue;
            this.lblCbMessage.Location = new System.Drawing.Point(3, 89);
            this.lblCbMessage.Name = "lblCbMessage";
            this.lblCbMessage.Size = new System.Drawing.Size(114, 20);
            this.lblCbMessage.TabIndex = 16;
            this.lblCbMessage.Text = "lblCbMessage";
            // 
            // cbRelateFolders
            // 
            this.cbRelateFolders.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbRelateFolders.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbRelateFolders.BackColor = System.Drawing.Color.Lavender;
            this.cbRelateFolders.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbRelateFolders.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cbRelateFolders.FormattingEnabled = true;
            this.cbRelateFolders.Location = new System.Drawing.Point(0, 36);
            this.cbRelateFolders.Name = "cbRelateFolders";
            this.cbRelateFolders.Size = new System.Drawing.Size(390, 32);
            this.cbRelateFolders.TabIndex = 2;
            this.cbRelateFolders.SelectedIndexChanged += new System.EventHandler(this.cbFolders_SelectedIndexChanged);
            this.cbRelateFolders.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cbRelateFolders_KeyUp);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRefresh.Location = new System.Drawing.Point(3, 57);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 29);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.TabStop = false;
            this.btnRefresh.Text = "更新(&R)";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // txtRootPath
            // 
            this.txtRootPath.BackColor = System.Drawing.Color.Honeydew;
            this.txtRootPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtRootPath.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtRootPath.Location = new System.Drawing.Point(0, 0);
            this.txtRootPath.Name = "txtRootPath";
            this.txtRootPath.Size = new System.Drawing.Size(390, 36);
            this.txtRootPath.TabIndex = 1;
            this.txtRootPath.Text = "Q:\\Comic\\ComicShelf";
            this.txtRootPath.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPath_KeyPress);
            // 
            // tvFolder
            // 
            this.tvFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvFolder.BackColor = System.Drawing.Color.Honeydew;
            this.tvFolder.ImageIndex = 0;
            this.tvFolder.ImageList = this.tvFolderImageList1;
            this.tvFolder.Location = new System.Drawing.Point(3, 106);
            this.tvFolder.Name = "tvFolder";
            this.tvFolder.SelectedImageIndex = 0;
            this.tvFolder.Size = new System.Drawing.Size(300, 524);
            this.tvFolder.TabIndex = 5;
            this.tvFolder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tvFolder_KeyPress);
            this.tvFolder.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tvFolder_MouseDoubleClick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtInfo);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1176, 647);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "訊息";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.SystemColors.Info;
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfo.Location = new System.Drawing.Point(3, 3);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(1170, 641);
            this.txtInfo.TabIndex = 7;
            this.txtInfo.Text = resources.GetString("txtInfo.Text");
            this.txtInfo.TextChanged += new System.EventHandler(this.txtInfo_TextChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.setArchiveDownloadedFile);
            this.tabPage3.Controls.Add(this.setSaveWebSiteName);
            this.tabPage3.Controls.Add(this.setBackGroundLoad);
            this.tabPage3.Controls.Add(this.gbProxy);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.setLoadAllPicture);
            this.tabPage3.Controls.Add(this.btnSave);
            this.tabPage3.Controls.Add(this.setLocalPath);
            this.tabPage3.Controls.Add(this.setWinRARPath);
            this.tabPage3.Controls.Add(this.setPhotoProgramPath);
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1176, 647);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "設定資料";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // setArchiveDownloadedFile
            // 
            this.setArchiveDownloadedFile.AutoSize = true;
            this.setArchiveDownloadedFile.Checked = true;
            this.setArchiveDownloadedFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.setArchiveDownloadedFile.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.setArchiveDownloadedFile.Location = new System.Drawing.Point(313, 224);
            this.setArchiveDownloadedFile.Margin = new System.Windows.Forms.Padding(2);
            this.setArchiveDownloadedFile.Name = "setArchiveDownloadedFile";
            this.setArchiveDownloadedFile.Size = new System.Drawing.Size(124, 22);
            this.setArchiveDownloadedFile.TabIndex = 33;
            this.setArchiveDownloadedFile.Text = "下載後壓縮";
            this.setArchiveDownloadedFile.UseVisualStyleBackColor = true;
            // 
            // setSaveWebSiteName
            // 
            this.setSaveWebSiteName.AutoSize = true;
            this.setSaveWebSiteName.Checked = true;
            this.setSaveWebSiteName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.setSaveWebSiteName.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.setSaveWebSiteName.Location = new System.Drawing.Point(32, 224);
            this.setSaveWebSiteName.Margin = new System.Windows.Forms.Padding(2);
            this.setSaveWebSiteName.Name = "setSaveWebSiteName";
            this.setSaveWebSiteName.Size = new System.Drawing.Size(178, 22);
            this.setSaveWebSiteName.TabIndex = 32;
            this.setSaveWebSiteName.Text = "存檔目錄加上前綴";
            this.setSaveWebSiteName.UseVisualStyleBackColor = true;
            // 
            // setBackGroundLoad
            // 
            this.setBackGroundLoad.AutoSize = true;
            this.setBackGroundLoad.Checked = true;
            this.setBackGroundLoad.CheckState = System.Windows.Forms.CheckState.Checked;
            this.setBackGroundLoad.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.setBackGroundLoad.Location = new System.Drawing.Point(32, 275);
            this.setBackGroundLoad.Margin = new System.Windows.Forms.Padding(2);
            this.setBackGroundLoad.Name = "setBackGroundLoad";
            this.setBackGroundLoad.Size = new System.Drawing.Size(142, 22);
            this.setBackGroundLoad.TabIndex = 31;
            this.setBackGroundLoad.Text = "自動載入資訊";
            this.setBackGroundLoad.UseVisualStyleBackColor = true;
            // 
            // gbProxy
            // 
            this.gbProxy.Controls.Add(this.label7);
            this.gbProxy.Controls.Add(this.setProxyPort);
            this.gbProxy.Controls.Add(this.label6);
            this.gbProxy.Controls.Add(this.setProxyUrl);
            this.gbProxy.Controls.Add(this.setUsingProxy);
            this.gbProxy.Location = new System.Drawing.Point(32, 306);
            this.gbProxy.Name = "gbProxy";
            this.gbProxy.Size = new System.Drawing.Size(244, 121);
            this.gbProxy.TabIndex = 30;
            this.gbProxy.TabStop = false;
            this.gbProxy.Text = "Proxy設定";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 18);
            this.label7.TabIndex = 34;
            this.label7.Text = "ProxyPort";
            // 
            // setProxyPort
            // 
            this.setProxyPort.Location = new System.Drawing.Point(76, 77);
            this.setProxyPort.Name = "setProxyPort";
            this.setProxyPort.Size = new System.Drawing.Size(69, 29);
            this.setProxyPort.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 18);
            this.label6.TabIndex = 33;
            this.label6.Text = "ProxyURL";
            // 
            // setProxyUrl
            // 
            this.setProxyUrl.Location = new System.Drawing.Point(76, 49);
            this.setProxyUrl.Name = "setProxyUrl";
            this.setProxyUrl.Size = new System.Drawing.Size(155, 29);
            this.setProxyUrl.TabIndex = 31;
            // 
            // setUsingProxy
            // 
            this.setUsingProxy.AutoSize = true;
            this.setUsingProxy.Checked = true;
            this.setUsingProxy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.setUsingProxy.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.setUsingProxy.Location = new System.Drawing.Point(16, 27);
            this.setUsingProxy.Margin = new System.Windows.Forms.Padding(2);
            this.setUsingProxy.Name = "setUsingProxy";
            this.setUsingProxy.Size = new System.Drawing.Size(109, 22);
            this.setUsingProxy.TabIndex = 30;
            this.setUsingProxy.Text = "使用Proxy";
            this.setUsingProxy.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 189);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 18);
            this.label5.TabIndex = 25;
            this.label5.Text = "本地圖片路徑";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 18);
            this.label4.TabIndex = 24;
            this.label4.Text = "WinRAR路徑";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 18);
            this.label3.TabIndex = 23;
            this.label3.Text = "看圖軟體路徑";
            // 
            // setLoadAllPicture
            // 
            this.setLoadAllPicture.AutoSize = true;
            this.setLoadAllPicture.Checked = true;
            this.setLoadAllPicture.CheckState = System.Windows.Forms.CheckState.Checked;
            this.setLoadAllPicture.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.setLoadAllPicture.Location = new System.Drawing.Point(32, 250);
            this.setLoadAllPicture.Margin = new System.Windows.Forms.Padding(2);
            this.setLoadAllPicture.Name = "setLoadAllPicture";
            this.setLoadAllPicture.Size = new System.Drawing.Size(214, 22);
            this.setLoadAllPicture.TabIndex = 4;
            this.setLoadAllPicture.Text = "展開頁時載入全部縮圖";
            this.setLoadAllPicture.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(32, 21);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "儲存設定(&S)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // setLocalPath
            // 
            this.setLocalPath.Location = new System.Drawing.Point(115, 184);
            this.setLocalPath.Name = "setLocalPath";
            this.setLocalPath.Size = new System.Drawing.Size(819, 29);
            this.setLocalPath.TabIndex = 3;
            // 
            // setWinRARPath
            // 
            this.setWinRARPath.Location = new System.Drawing.Point(115, 132);
            this.setWinRARPath.Name = "setWinRARPath";
            this.setWinRARPath.Size = new System.Drawing.Size(819, 29);
            this.setWinRARPath.TabIndex = 2;
            // 
            // setPhotoProgramPath
            // 
            this.setPhotoProgramPath.Location = new System.Drawing.Point(115, 80);
            this.setPhotoProgramPath.Name = "setPhotoProgramPath";
            this.setPhotoProgramPath.Size = new System.Drawing.Size(819, 29);
            this.setPhotoProgramPath.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusMsg,
            this.statusMsg2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 676);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1184, 28);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusMsg
            // 
            this.statusMsg.ForeColor = System.Drawing.Color.Blue;
            this.statusMsg.Name = "statusMsg";
            this.statusMsg.Size = new System.Drawing.Size(22, 23);
            this.statusMsg.Text = "[]";
            // 
            // statusMsg2
            // 
            this.statusMsg2.Name = "statusMsg2";
            this.statusMsg2.Size = new System.Drawing.Size(22, 23);
            this.statusMsg2.Text = "[]";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 800;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_ProgressChanged);
            // 
            // exceptMenu
            // 
            this.exceptMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.exceptMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddIgnoreComic});
            this.exceptMenu.Name = "exceptMenu";
            this.exceptMenu.Size = new System.Drawing.Size(255, 32);
            // 
            // AddIgnoreComic
            // 
            this.AddIgnoreComic.Name = "AddIgnoreComic";
            this.AddIgnoreComic.Size = new System.Drawing.Size(254, 28);
            this.AddIgnoreComic.Text = "加入例外網頁(不顯示)";
            this.AddIgnoreComic.Click += new System.EventHandler(this.AddIgnoreComic_Click);
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1184, 704);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.Text = "漫畫下載器 2018/02/18 0.0.4.002";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.gbProxy.ResumeLayout(false);
            this.gbProxy.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.exceptMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TreeView tvComicTree;
        private System.Windows.Forms.Button btnDownload;
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
        private System.Windows.Forms.CheckBox chkSaveWebSiteName;
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


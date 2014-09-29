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
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("新動漫(xindm)");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbComicCatcher = new System.Windows.Forms.ComboBox();
            this.chkUsingAlternativeUrl = new System.Windows.Forms.CheckBox();
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
            this.chkSaveWebSiteName = new System.Windows.Forms.CheckBox();
            this.setSaveWebSiteName = new System.Windows.Forms.CheckBox();
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
            this.tabControl1.Size = new System.Drawing.Size(1045, 679);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1037, 653);
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
            this.splitContainer1.Panel1.Controls.Add(this.chkSaveWebSiteName);
            this.splitContainer1.Panel1.Controls.Add(this.cbComicCatcher);
            this.splitContainer1.Panel1.Controls.Add(this.chkUsingAlternativeUrl);
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
            this.splitContainer1.Panel2.Controls.Add(this.btnOpenDirectory);
            this.splitContainer1.Panel2.Controls.Add(this.btnDelete);
            this.splitContainer1.Panel2.Controls.Add(this.btnArchive);
            this.splitContainer1.Panel2.Controls.Add(this.lblCbMessage);
            this.splitContainer1.Panel2.Controls.Add(this.cbRelateFolders);
            this.splitContainer1.Panel2.Controls.Add(this.btnRefresh);
            this.splitContainer1.Panel2.Controls.Add(this.txtRootPath);
            this.splitContainer1.Panel2.Controls.Add(this.tvFolder);
            this.splitContainer1.Size = new System.Drawing.Size(1031, 647);
            this.splitContainer1.SplitterDistance = 600;
            this.splitContainer1.TabIndex = 3;
            this.splitContainer1.TabStop = false;
            // 
            // cbComicCatcher
            // 
            this.cbComicCatcher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbComicCatcher.FormattingEnabled = true;
            this.cbComicCatcher.Items.AddRange(new object[] {
            "xindm",
            "dm5"});
            this.cbComicCatcher.Location = new System.Drawing.Point(3, 10);
            this.cbComicCatcher.Name = "cbComicCatcher";
            this.cbComicCatcher.Size = new System.Drawing.Size(106, 20);
            this.cbComicCatcher.TabIndex = 3;
            this.cbComicCatcher.SelectedIndexChanged += new System.EventHandler(this.cbComicCatcher_SelectedIndexChanged);
            // 
            // chkUsingAlternativeUrl
            // 
            this.chkUsingAlternativeUrl.AutoSize = true;
            this.chkUsingAlternativeUrl.Enabled = false;
            this.chkUsingAlternativeUrl.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.chkUsingAlternativeUrl.Location = new System.Drawing.Point(352, 149);
            this.chkUsingAlternativeUrl.Margin = new System.Windows.Forms.Padding(2);
            this.chkUsingAlternativeUrl.Name = "chkUsingAlternativeUrl";
            this.chkUsingAlternativeUrl.Size = new System.Drawing.Size(168, 16);
            this.chkUsingAlternativeUrl.TabIndex = 10;
            this.chkUsingAlternativeUrl.Text = "使用替代網址(需關閉Proxy)";
            this.chkUsingAlternativeUrl.UseVisualStyleBackColor = true;
            this.chkUsingAlternativeUrl.CheckedChanged += new System.EventHandler(this.chkUsingAlternativeUrl_CheckedChanged);
            // 
            // btnCollapse
            // 
            this.btnCollapse.Location = new System.Drawing.Point(409, 117);
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
            this.chkLoadPhoto.Location = new System.Drawing.Point(352, 101);
            this.chkLoadPhoto.Margin = new System.Windows.Forms.Padding(2);
            this.chkLoadPhoto.Name = "chkLoadPhoto";
            this.chkLoadPhoto.Size = new System.Drawing.Size(144, 16);
            this.chkLoadPhoto.TabIndex = 7;
            this.chkLoadPhoto.Text = "展開頁時載入全部縮圖";
            this.chkLoadPhoto.UseVisualStyleBackColor = true;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(491, 39);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(48, 23);
            this.btnFind.TabIndex = 6;
            this.btnFind.Text = "尋找";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(352, 39);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(133, 22);
            this.txtFind.TabIndex = 5;
            this.txtFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFind_KeyPress);
            // 
            // pbIcon
            // 
            this.pbIcon.BackColor = System.Drawing.Color.SaddleBrown;
            this.pbIcon.Location = new System.Drawing.Point(352, 226);
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
            this.lblUpdateChapter.Location = new System.Drawing.Point(436, 205);
            this.lblUpdateChapter.Name = "lblUpdateChapter";
            this.lblUpdateChapter.Size = new System.Drawing.Size(118, 16);
            this.lblUpdateChapter.TabIndex = 14;
            this.lblUpdateChapter.Text = "lblUpdateChapter";
            // 
            // lblUpdateDate
            // 
            this.lblUpdateDate.AutoSize = true;
            this.lblUpdateDate.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblUpdateDate.ForeColor = System.Drawing.Color.Red;
            this.lblUpdateDate.Location = new System.Drawing.Point(436, 178);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(98, 16);
            this.lblUpdateDate.TabIndex = 12;
            this.lblUpdateDate.Text = "lblUpdateDate";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(352, 205);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "更新回數：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(352, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "更新日期：";
            // 
            // chkIsUseProxy
            // 
            this.chkIsUseProxy.AutoSize = true;
            this.chkIsUseProxy.Checked = true;
            this.chkIsUseProxy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsUseProxy.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.chkIsUseProxy.Location = new System.Drawing.Point(352, 121);
            this.chkIsUseProxy.Margin = new System.Windows.Forms.Padding(2);
            this.chkIsUseProxy.Name = "chkIsUseProxy";
            this.chkIsUseProxy.Size = new System.Drawing.Size(52, 16);
            this.chkIsUseProxy.TabIndex = 8;
            this.chkIsUseProxy.Text = "Proxy";
            this.chkIsUseProxy.UseVisualStyleBackColor = true;
            this.chkIsUseProxy.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(115, 8);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(440, 22);
            this.txtUrl.TabIndex = 1;
            // 
            // tvComicTree
            // 
            this.tvComicTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvComicTree.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.tvComicTree.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tvComicTree.HideSelection = false;
            this.tvComicTree.Location = new System.Drawing.Point(4, 36);
            this.tvComicTree.Name = "tvComicTree";
            treeNode3.Name = "http://www.xindm.cn";
            treeNode3.Text = "新動漫(xindm)";
            this.tvComicTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3});
            this.tvComicTree.Size = new System.Drawing.Size(338, 607);
            this.tvComicTree.TabIndex = 4;
            this.tvComicTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvComicTree_AfterSelect);
            this.tvComicTree.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tvComicTree_KeyPress);
            this.tvComicTree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvComicTree_KeyUp);
            this.tvComicTree.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tvComicTree_MouseDoubleClick);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(355, 535);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.TabStop = false;
            this.btnDownload.Text = "下載";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Visible = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnOpenDirectory
            // 
            this.btnOpenDirectory.Font = new System.Drawing.Font("新細明體", 14F);
            this.btnOpenDirectory.Location = new System.Drawing.Point(347, 206);
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
            this.btnDelete.Location = new System.Drawing.Point(347, 101);
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
            // 
            // btnArchive
            // 
            this.btnArchive.Location = new System.Drawing.Point(281, 72);
            this.btnArchive.Name = "btnArchive";
            this.btnArchive.Size = new System.Drawing.Size(75, 23);
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
            this.lblCbMessage.Location = new System.Drawing.Point(3, 74);
            this.lblCbMessage.Name = "lblCbMessage";
            this.lblCbMessage.Size = new System.Drawing.Size(85, 14);
            this.lblCbMessage.TabIndex = 16;
            this.lblCbMessage.Text = "lblCbMessage";
            // 
            // cbRelateFolders
            // 
            this.cbRelateFolders.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbRelateFolders.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbRelateFolders.BackColor = System.Drawing.Color.Lavender;
            this.cbRelateFolders.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cbRelateFolders.FormattingEnabled = true;
            this.cbRelateFolders.Location = new System.Drawing.Point(4, 34);
            this.cbRelateFolders.Name = "cbRelateFolders";
            this.cbRelateFolders.Size = new System.Drawing.Size(271, 24);
            this.cbRelateFolders.TabIndex = 2;
            this.cbRelateFolders.SelectedIndexChanged += new System.EventHandler(this.cbFolders_SelectedIndexChanged);
            this.cbRelateFolders.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cbRelateFolders_KeyUp);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRefresh.Location = new System.Drawing.Point(281, 32);
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
            this.txtRootPath.Size = new System.Drawing.Size(427, 27);
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
            this.tvFolder.Location = new System.Drawing.Point(0, 101);
            this.tvFolder.Name = "tvFolder";
            this.tvFolder.SelectedImageIndex = 0;
            this.tvFolder.Size = new System.Drawing.Size(341, 546);
            this.tvFolder.TabIndex = 5;
            this.tvFolder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tvFolder_KeyPress);
            this.tvFolder.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tvFolder_MouseDoubleClick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtInfo);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1037, 653);
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
            this.txtInfo.Size = new System.Drawing.Size(1031, 647);
            this.txtInfo.TabIndex = 7;
            this.txtInfo.Text = resources.GetString("txtInfo.Text");
            this.txtInfo.TextChanged += new System.EventHandler(this.txtInfo_TextChanged);
            // 
            // tabPage3
            // 
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
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1037, 653);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "設定資料";
            this.tabPage3.UseVisualStyleBackColor = true;
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
            this.setBackGroundLoad.Size = new System.Drawing.Size(96, 16);
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
            this.label7.Size = new System.Drawing.Size(52, 12);
            this.label7.TabIndex = 34;
            this.label7.Text = "ProxyPort";
            // 
            // setProxyPort
            // 
            this.setProxyPort.Location = new System.Drawing.Point(76, 77);
            this.setProxyPort.Name = "setProxyPort";
            this.setProxyPort.Size = new System.Drawing.Size(69, 22);
            this.setProxyPort.TabIndex = 32;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 12);
            this.label6.TabIndex = 33;
            this.label6.Text = "ProxyURL";
            // 
            // setProxyUrl
            // 
            this.setProxyUrl.Location = new System.Drawing.Point(76, 49);
            this.setProxyUrl.Name = "setProxyUrl";
            this.setProxyUrl.Size = new System.Drawing.Size(155, 22);
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
            this.setUsingProxy.Size = new System.Drawing.Size(76, 16);
            this.setUsingProxy.TabIndex = 30;
            this.setUsingProxy.Text = "使用Proxy";
            this.setUsingProxy.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 189);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 25;
            this.label5.Text = "本地圖片路徑";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 12);
            this.label4.TabIndex = 24;
            this.label4.Text = "WinRAR路徑";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
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
            this.setLoadAllPicture.Size = new System.Drawing.Size(144, 16);
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
            this.setLocalPath.Size = new System.Drawing.Size(819, 22);
            this.setLocalPath.TabIndex = 3;
            // 
            // setWinRARPath
            // 
            this.setWinRARPath.Location = new System.Drawing.Point(115, 132);
            this.setWinRARPath.Name = "setWinRARPath";
            this.setWinRARPath.Size = new System.Drawing.Size(819, 22);
            this.setWinRARPath.TabIndex = 2;
            // 
            // setPhotoProgramPath
            // 
            this.setPhotoProgramPath.Location = new System.Drawing.Point(115, 80);
            this.setPhotoProgramPath.Name = "setPhotoProgramPath";
            this.setPhotoProgramPath.Size = new System.Drawing.Size(819, 22);
            this.setPhotoProgramPath.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusMsg,
            this.statusMsg2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 680);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1045, 24);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusMsg
            // 
            this.statusMsg.ForeColor = System.Drawing.Color.Blue;
            this.statusMsg.Name = "statusMsg";
            this.statusMsg.Size = new System.Drawing.Size(19, 19);
            this.statusMsg.Text = "[]";
            // 
            // statusMsg2
            // 
            this.statusMsg2.Name = "statusMsg2";
            this.statusMsg2.Size = new System.Drawing.Size(19, 19);
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
            // chkSaveWebSiteName
            // 
            this.chkSaveWebSiteName.AutoSize = true;
            this.chkSaveWebSiteName.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.chkSaveWebSiteName.Location = new System.Drawing.Point(352, 76);
            this.chkSaveWebSiteName.Margin = new System.Windows.Forms.Padding(2);
            this.chkSaveWebSiteName.Name = "chkSaveWebSiteName";
            this.chkSaveWebSiteName.Size = new System.Drawing.Size(120, 16);
            this.chkSaveWebSiteName.TabIndex = 18;
            this.chkSaveWebSiteName.Text = "存檔目錄加上前綴";
            this.chkSaveWebSiteName.UseVisualStyleBackColor = true;
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
            this.setSaveWebSiteName.Size = new System.Drawing.Size(120, 16);
            this.setSaveWebSiteName.TabIndex = 32;
            this.setSaveWebSiteName.Text = "存檔目錄加上前綴";
            this.setSaveWebSiteName.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 704);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.Text = "漫畫下載器 2014/09/27 0.0.2.001";
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
        private System.Windows.Forms.CheckBox chkUsingAlternativeUrl;
        private System.Windows.Forms.Button btnOpenDirectory;
        private System.Windows.Forms.CheckBox setBackGroundLoad;
        private System.Windows.Forms.ComboBox cbComicCatcher;
        private System.Windows.Forms.CheckBox chkSaveWebSiteName;
        private System.Windows.Forms.CheckBox setSaveWebSiteName;

    }
}


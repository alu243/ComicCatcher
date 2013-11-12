namespace ComicArchiver
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtPath = new System.Windows.Forms.TextBox();
            this.tvFolder = new System.Windows.Forms.TreeView();
            this.ilFolder = new System.Windows.Forms.ImageList(this.components);
            this.btnBrowser = new System.Windows.Forms.Button();
            this.btnFileNext = new System.Windows.Forms.Button();
            this.btnFilePrev = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.BackColor = System.Drawing.SystemColors.Info;
            this.txtPath.Location = new System.Drawing.Point(202, 12);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(448, 22);
            this.txtPath.TabIndex = 0;
            this.txtPath.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtPath_MouseClick);
            this.txtPath.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            this.txtPath.Enter += new System.EventHandler(this.txtPath_Enter);
            this.txtPath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPath_KeyUp);
            // 
            // tvFolder
            // 
            this.tvFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvFolder.BackColor = System.Drawing.SystemColors.Info;
            this.tvFolder.ImageIndex = 0;
            this.tvFolder.ImageList = this.ilFolder;
            this.tvFolder.Location = new System.Drawing.Point(13, 42);
            this.tvFolder.Name = "tvFolder";
            this.tvFolder.SelectedImageIndex = 0;
            this.tvFolder.Size = new System.Drawing.Size(717, 545);
            this.tvFolder.TabIndex = 1;
            this.tvFolder.DoubleClick += new System.EventHandler(this.tvFolder_DoubleClick);
            this.tvFolder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tvFolder_KeyPress);
            // 
            // ilFolder
            // 
            this.ilFolder.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilFolder.ImageStream")));
            this.ilFolder.TransparentColor = System.Drawing.Color.Transparent;
            this.ilFolder.Images.SetKeyName(0, "my_computer2.png");
            this.ilFolder.Images.SetKeyName(1, "未命名.png");
            this.ilFolder.Images.SetKeyName(2, "winrar-icon-small.png");
            // 
            // btnBrowser
            // 
            this.btnBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowser.Location = new System.Drawing.Point(655, 10);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(75, 23);
            this.btnBrowser.TabIndex = 2;
            this.btnBrowser.Text = "瀏覽";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // btnFileNext
            // 
            this.btnFileNext.Location = new System.Drawing.Point(12, 10);
            this.btnFileNext.Name = "btnFileNext";
            this.btnFileNext.Size = new System.Drawing.Size(89, 23);
            this.btnFileNext.TabIndex = 3;
            this.btnFileNext.Text = "找下一個 (&N)";
            this.btnFileNext.UseVisualStyleBackColor = true;
            this.btnFileNext.Click += new System.EventHandler(this.btnFileNext_Click);
            // 
            // btnFilePrev
            // 
            this.btnFilePrev.Location = new System.Drawing.Point(107, 10);
            this.btnFilePrev.Name = "btnFilePrev";
            this.btnFilePrev.Size = new System.Drawing.Size(89, 23);
            this.btnFilePrev.TabIndex = 4;
            this.btnFilePrev.Text = "找上一個 (&P)";
            this.btnFilePrev.UseVisualStyleBackColor = true;
            this.btnFilePrev.Click += new System.EventHandler(this.btnFilePrev_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 599);
            this.Controls.Add(this.btnFilePrev);
            this.Controls.Add(this.btnFileNext);
            this.Controls.Add(this.btnBrowser);
            this.Controls.Add(this.tvFolder);
            this.Controls.Add(this.txtPath);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "漫畫壓縮程式";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TreeView tvFolder;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.ImageList ilFolder;
        private System.Windows.Forms.Button btnFileNext;
        private System.Windows.Forms.Button btnFilePrev;
    }
}


﻿namespace ComicCatcher
{
    partial class frmView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            webview = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)webview).BeginInit();
            SuspendLayout();
            // 
            // webview
            // 
            webview.AllowExternalDrop = true;
            webview.CreationProperties = null;
            webview.DefaultBackgroundColor = System.Drawing.Color.White;
            webview.Dock = System.Windows.Forms.DockStyle.Fill;
            webview.Location = new System.Drawing.Point(0, 0);
            webview.Name = "webview";
            webview.Size = new System.Drawing.Size(800, 450);
            webview.TabIndex = 0;
            webview.ZoomFactor = 1D;
            // 
            // frmView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(webview);
            Name = "frmView";
            Text = "frmView";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += frmView_Load;
            ((System.ComponentModel.ISupportInitialize)webview).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webview;
    }
}
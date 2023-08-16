using System;
using System.Windows.Forms;

namespace ComicCatcher
{
    public partial class frmView : Form
    {
        public frmView()
        {
            InitializeComponent();
            webview.NavigationCompleted += WebView2_NavigateComplete;
        }
        private string _path;
        public void SetPath(string path)
        {
            this._path = path;
            //webview.CoreWebView2?.ExecuteScriptAsync("window.scroll(0,100000)");
            //var webview = new Microsoft.Web.WebView2.WinForms.WebView2();
        }

        private void WebView2_NavigateComplete(object sender, EventArgs e)
        {
            webview.ExecuteScriptAsync("window.scroll(0,0);");
        }

        private async void frmView_Load(object sender, EventArgs e)
        {
            await webview.EnsureCoreWebView2Async();
            webview.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webview.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webview.CoreWebView2.Navigate(_path);
        }
    }
}

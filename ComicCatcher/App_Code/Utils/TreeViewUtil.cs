using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Utils
{
    public static class TreeViewUtil
    {
        private static Font boldFont = new Font("新細明體", 10, FontStyle.Bold);
        public static void SetFontRegular(TreeNode tn)
        {
            if (tn.TreeView == null) return;
            if (tn.TreeView.InvokeRequired)
                tn.TreeView.Invoke(new MethodInvoker(() =>
                {
                    tn.NodeFont = boldFont;
                    if (tn.Level == 2)
                    {
                        tn.ImageIndex = 99;
                        tn.SelectedImageIndex = 99;
                    }
                    else if (tn.Level == 3)
                    {
                        tn.ImageIndex = 99;
                        tn.SelectedImageIndex = 99;
                    }
                }));
            else
                lock (tn)
                {
                    if (null != tn.NodeFont && true == tn.NodeFont.Bold)
                    {
                        tn.NodeFont = new Font("新細明體", 10, FontStyle.Regular);
                        if (tn.Level == 2)
                        {
                            tn.ImageIndex = 99;
                            tn.SelectedImageIndex = 99;
                        }
                        else if (tn.Level == 3)
                        {
                            tn.ImageIndex = 99;
                            tn.SelectedImageIndex = 99;
                        }
                        tn.Text = tn.Text;
                    }
                }
        }

        public static void SetFontBold(TreeNode tn)
        {
            if (tn.TreeView == null) return;
            if (tn.TreeView.InvokeRequired)
                tn.TreeView.Invoke(new MethodInvoker(() =>
                {
                    tn.NodeFont = boldFont;
                    if (tn.Level == 2)
                    {
                        tn.ImageIndex = 4;
                        tn.SelectedImageIndex = 4;
                    }
                    else if (tn.Level == 3)
                    {
                        tn.ImageIndex = 5;
                        tn.SelectedImageIndex = 5;
                    }
                }));
            else
                lock (tn)
                {
                    if (null == tn.NodeFont || false == tn.NodeFont.Bold)
                    {
                        tn.NodeFont = boldFont;
                        if (tn.Level == 2)
                        {
                            tn.ImageIndex = 4;
                            tn.SelectedImageIndex = 4;
                        }
                        else if (tn.Level == 3)
                        {
                            tn.ImageIndex = 5;
                            tn.SelectedImageIndex = 5;
                        }
                        tn.Text = tn.Text;
                    }
                }
        }

        public static void SetFontBold(TreeNode tn, Color color)
        {
            if (tn.TreeView == null) return;
            if (tn.TreeView.InvokeRequired)
                tn.TreeView.Invoke(new MethodInvoker(() =>
                {
                    tn.NodeFont = boldFont;
                    tn.ForeColor = color;
                    if (tn.Level == 2)
                    {
                        tn.ImageIndex = 4;
                        tn.SelectedImageIndex = 4;
                    }
                    else if (tn.Level == 3)
                    {
                        tn.ImageIndex = 5;
                        tn.SelectedImageIndex = 5;
                    }
                }));
            else
            {
                lock (tn)
                {
                    tn.NodeFont = boldFont;
                    tn.ForeColor = color;
                    if (tn.Level == 2)
                    {
                        tn.ImageIndex = 4;
                        tn.SelectedImageIndex = 4;
                    }
                    else if (tn.Level == 3)
                    {
                        tn.ImageIndex = 5;
                        tn.SelectedImageIndex = 5;
                    }
                }
            }
        }

        public static void ClearNodes(TreeView tv)
        {
            if (tv == null) return;
            if (tv.InvokeRequired)
                tv.Invoke(new MethodInvoker(() => tv.Nodes.Clear()));
            else
                lock (tv)
                {
                    tv.Nodes.Clear();
                }
        }

        public static void ClearTreeNode(TreeNode currNode)
        {
            if (currNode.TreeView == null) return;
            if (currNode.TreeView.InvokeRequired)
                currNode.TreeView.Invoke(new MethodInvoker(() => currNode.Nodes.Clear()));
            else
                lock (currNode)
                {
                    currNode.Nodes.Clear();
                }
        }

        public static List<TreeNode> AddTreeNodes(TreeNode parentNode, List<TreeNode> childNodes)
        {
            if (parentNode.TreeView == null) return null;
            if (parentNode.TreeView.InvokeRequired)
                parentNode.TreeView.Invoke(new MethodInvoker(() => parentNode.Nodes.AddRange(childNodes.ToArray())));
            else
                lock (parentNode)
                {
                    parentNode.TreeView.BeginUpdate();
                    parentNode.Nodes.AddRange(childNodes.ToArray());
                    parentNode.TreeView.EndUpdate();
                }
            return childNodes;
        }

        //public static TreeNode AddTreeNode(TreeNode parentNode, string key, string text)
        //{
        //    TreeNode tn = new TreeNode();
        //    if (parentNode.TreeView == null) return null;
        //    if (parentNode.TreeView.InvokeRequired)
        //        parentNode.TreeView.Invoke(new MethodInvoker(() => tn = parentNode.Nodes.Add(key, text)));
        //    else
        //        lock (parentNode)
        //        {
        //            tn = parentNode.Nodes.Add(key, text);
        //        }
        //    return tn;
        //}

        public static TreeNode BuildNode(ComicModels.ComicNameInWebPage comic, string localComicPath, string groupName)
        {
            TreeNode nameNode = new TreeNode();
            nameNode.Name = comic.Url;
            nameNode.Text = comic.Caption;
            nameNode.ImageKey = comic.IconUrl;
            nameNode.Tag = comic;

            // 如果本地端已有此漫畫的資料夾，改為粗體顯示
            if (Directory.Exists(Path.Combine(localComicPath, nameNode.Text)))
            {
                nameNode.NodeFont = boldFont;
                nameNode.ImageIndex = 4;
                nameNode.SelectedImageIndex = 4;
            }
            else if (Directory.Exists(Path.Combine(localComicPath, groupName)))
            {
                nameNode.ForeColor = Color.DarkBlue;
                nameNode.NodeFont = boldFont;
                nameNode.ImageIndex = 4;
                nameNode.SelectedImageIndex = 4;
            }
            else
            {
                nameNode.ImageIndex = 99;
                nameNode.SelectedImageIndex = 99;
            }

            return nameNode;
        }

        public static TreeNode BuildNode(ComicModels.ComicChapterInName chapter, string webSiteName, string comicName)
        {
            TreeNode chapterNode = new TreeNode();
            chapterNode.Name = chapter.Url;
            chapterNode.Text = chapter.Caption;
            chapterNode.Tag = chapter;

            // 如果是已下載過的點，變粗體，改顏色
            if (Models.DownloadedList.HasDownloaded(webSiteName, comicName, chapterNode.Text))
            {
                chapterNode.NodeFont = boldFont;
                chapterNode.ForeColor = Color.Blue;
                chapterNode.ImageIndex = 5;
                chapterNode.SelectedImageIndex = 5;
            }
            else
            {
                chapterNode.ImageIndex = 99;
                chapterNode.SelectedImageIndex = 99;
            }

            return chapterNode;
        }
    }
}


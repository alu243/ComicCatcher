using ComicCatcherLib.ComicModels;
using ComicCatcherLib.DbModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ComicCatcher.Utils
{
    public static class TreeViewUtil
    {
        private static Font boldFont = new Font("新細明體", 10, FontStyle.Bold);
        public static void SetFontRegular(TreeNode tn)
        {
            if (tn.TreeView == null) return;
            if (tn.TreeView.InvokeRequired)
            {
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
            }
            else
            {
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
            {
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
            }
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

        public static void ClearTree(TreeView tv)
        {
            if (tv == null) return;
            if (tv.InvokeRequired)
            {
                tv.Invoke(new MethodInvoker(() => tv.Nodes.Clear()));
            }
            else
            {
                lock (tv) tv.Nodes.Clear();
            }
        }

        public static void ClearSubNode(TreeNode currNode)
        {
            if (currNode.TreeView == null) return;
            if (currNode.TreeView.InvokeRequired)
            {
                currNode.TreeView.Invoke(new MethodInvoker(() => currNode.Nodes.Clear()));
            }
            else
            {
                lock (currNode) currNode.Nodes.Clear();
            }
        }

        public static List<TreeNode> AddTreeNodes(TreeNode parentNode, List<TreeNode> childNodes)
        {
            if (parentNode.TreeView == null) return null;
            if (parentNode.TreeView.InvokeRequired)
            {
                parentNode.TreeView.Invoke(new MethodInvoker(() => parentNode.Nodes.AddRange(childNodes.ToArray())));
            }
            else
            {
                lock (parentNode)
                {
                    parentNode.TreeView.BeginUpdate();
                    parentNode.Nodes.AddRange(childNodes.ToArray());
                    parentNode.TreeView.EndUpdate();
                }
            }
            return childNodes;
        }

        public static TreeNode BuildNode(ComicPagination pagination)
        {
            TreeNode paginationNode = new TreeNode(pagination.Caption);
            paginationNode.Name = pagination.Url;
            paginationNode.Tag = pagination;
            paginationNode.ImageIndex = 3;
            paginationNode.SelectedImageIndex = 3;
            return paginationNode;
        }

        public static TreeNode BuildNode(ComicRoot root)
        {
            TreeNode rootNode = new TreeNode(root.Caption);
            rootNode.Tag = root;
            return rootNode;
        }

        public static TreeNode BuildNode(ComicEntity comic, string localComicPath, string groupName)
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

        public static TreeNode BuildNode(ComicChapter chapter, string webSiteName, string comicName)
        {
            TreeNode chapterNode = new TreeNode();
            chapterNode.Name = chapter.Url;
            chapterNode.Text = chapter.Caption;
            chapterNode.Tag = chapter;

            // 如果是已下載過的點，變粗體，改顏色
            if (DownloadedList.HasDownloaded(webSiteName, comicName, chapterNode.Text).Result)
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


        /// <summary>
        /// 是否為顯示回數(集數)的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        public static bool IsChapterNode(TreeNode tn)
        {
            return null != (tn?.Tag as ComicChapter);
        }

        /// <summary>
        /// 是否為顯示漫畫名稱的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        public static bool IsComicNameNode(TreeNode tn)
        {
            return null != (tn?.Tag as ComicEntity);
        }

        /// <summary>
        /// 是否為顯示漫畫清單的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        public static bool IsPaginationNode(TreeNode tn)
        {
            return null != (tn?.Tag as ComicPagination);
        }
    }
}


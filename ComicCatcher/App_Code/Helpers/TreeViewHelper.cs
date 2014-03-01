using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Helpers
{
    public static class TreeViewHelper
    {
        private static Font font = new Font("新細明體", 10, FontStyle.Bold);
        public static void SetFontBold(TreeNode tn)
        {
            if (tn.TreeView != null)
            {
                if (tn.TreeView.InvokeRequired)
                    tn.TreeView.Invoke(new MethodInvoker(() => tn.NodeFont = font));
                else
                    tn.NodeFont = font;
            }
        }

        public static void SetFontBold(TreeNode tn, Color color)
        {
            if (tn.TreeView != null)
            {
                if (tn.TreeView.InvokeRequired)
                    tn.TreeView.Invoke(new MethodInvoker(() => { tn.NodeFont = font; tn.ForeColor = color; }));
                else
                {
                    tn.NodeFont = font;
                    tn.ForeColor = color;
                }
            }
        }

        public static void ClearTreeNode(TreeNode currNode)
        {
            if (currNode.TreeView.InvokeRequired)
                currNode.TreeView.Invoke(new MethodInvoker(() => currNode.Nodes.Clear()));
            else
                currNode.Nodes.Clear();
        }

        public static TreeNode AddTreeNode(TreeNode parentNode, TreeNode childNode)
        {
            if (parentNode.TreeView.InvokeRequired)
                parentNode.TreeView.Invoke(new MethodInvoker(() => parentNode.Nodes.Add(childNode)));
            else
                parentNode.Nodes.Add(childNode);
            return childNode;
        }

        public static TreeNode AddTreeNode(TreeNode parentNode, string key, string text)
        {
            TreeNode tn = new TreeNode();
            if (parentNode.TreeView.InvokeRequired)
                parentNode.TreeView.Invoke(new MethodInvoker(() => tn = parentNode.Nodes.Add(key, text)));
            else
                tn = parentNode.Nodes.Add(key, text);
            return tn;
        }
    }
}


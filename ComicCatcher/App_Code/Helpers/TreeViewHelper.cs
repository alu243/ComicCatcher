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
        public static void SetFontBold(TreeView tv, TreeNode tn)
        {
            if (tv.InvokeRequired)
                tv.Invoke(new MethodInvoker(() => tn.NodeFont = new Font(tv.Font, FontStyle.Bold)));
            else
                tn.NodeFont = new Font(tv.Font, FontStyle.Bold);
        }

        public static void SetFontBold(TreeView tv, TreeNode tn, Color color)
        {
            if (tv.InvokeRequired)
                tv.Invoke(new MethodInvoker(() => { tn.NodeFont = new Font(tv.Font, FontStyle.Bold); tn.ForeColor = color; }));
            else
            {
                tn.NodeFont = new Font(tv.Font, FontStyle.Bold);
                tn.ForeColor = color;
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


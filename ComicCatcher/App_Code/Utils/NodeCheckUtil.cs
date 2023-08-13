using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComicCatcher.ComicModels;

namespace Utils
{
    /// <summary>
    /// 判斷節點類別 Web -> List(page) -> ComicEntity -> Chapter
    /// </summary>
    class NodeCheckUtil
    {
        /// <summary>
        /// 是否為顯示回數(集數)的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        public static bool IsChapterNode(TreeNode tn)
        {
            return null != (tn.Tag as ComicChapter);
        }

        /// <summary>
        /// 是否為顯示漫畫名稱的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        public static bool IsComicNameNode(TreeNode tn)
        {
            return tn.Level == 2;
            //if (null == tn) return false;
            //if (false == tn.Name.Contains(XindmWebSite.WebUrl)) return false; // 節點本身必需要是包含此內容
            //// 如果父節點是清單連結，表示此連結是漫畫名稱連結
            //return IsListNode(tn.Parent);
        }

        /// <summary>
        /// 是否為顯示漫畫清單的節點
        /// </summary>
        /// <param name="tn"></param>
        /// <returns></returns>
        public static bool IsListNode(TreeNode tn)
        {
            return tn.Level == 1;
            //if (null == tn) return false;
            //return tn.Name.Contains(XindmWebSite.ListUrl);
        }

    }
}

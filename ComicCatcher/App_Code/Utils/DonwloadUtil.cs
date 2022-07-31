using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Utils;
using ComicModels;

namespace Utils
{
    public class DonwloadUtil
    {
        /// <summary>
        /// 取資料後存檔
        /// </summary>
        /// <param name="path"></param>
        public static void donwload(string pictureUrl, string reffer, string fullFileName)
        {
            try
            {
                if (false == Directory.Exists(Path.GetDirectoryName(fullFileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));
                }

                using (MemoryStream ms = ComicUtil.GetPicture(pictureUrl, reffer, Path.GetFileName(fullFileName)))
                {
                    using (FileStream fs = new FileStream(fullFileName, FileMode.Create))
                    {
                        ms.CopyTo(fs);
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("存檔時發生錯誤，原因：" + ex.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Utils;
namespace Utils
{
    public class DonwloadUtil
    {
        /// <summary>
        /// 取資料後存檔
        /// </summary>
        /// <param name="path"></param>
        public static void donwload(string pictureUrl, string fullFileName)
        {
            try
            {
                if (false == Directory.Exists(Path.GetDirectoryName(fullFileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));
                }

                // 如果下載時發生錯誤，重試 10 次
                using (MemoryStream ms = HttpUtil.getPictureResponse(pictureUrl, 10))
                {
                    using (FileStream fs = File.OpenWrite(fullFileName))
                    {
                        if (ms.Length > 0)
                        {
                            ms.Position = 0;
                            fs.Write(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
                        }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;
using System.Threading;

namespace Utils
{
    public static class FileUtil
    {
        private static string CalcMD5(string localTmpFile)
        {
            StringBuilder sb = new StringBuilder();
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            using (FileStream file = new FileStream(localTmpFile, FileMode.Open))
            {
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 比較兩個檔案的 md5 計算值是否相同
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        private static bool CompareMD5(string file1, string file2)
        {
            string file1MD5 = FileUtil.CalcMD5(file1);
            if (String.IsNullOrEmpty(file1MD5)) return false;
            string file2MD5 = FileUtil.CalcMD5(file2);
            if (String.IsNullOrEmpty(file2MD5)) return false;

            return file1MD5.Equals(file2MD5);
        }


        public static bool CompareFileByMD5(string tmpFile , string cmpFile)
        {
            if (true == CompareMD5(tmpFile, cmpFile))
            {
                if (File.Exists(cmpFile)) File.Delete(cmpFile);
                return true;
            }
            else
            {
                FileUtil.MoveFile(cmpFile, tmpFile);
                return false;
            }
        }

        public static void MoveFile(string sourceFile, string destFile)
        {
            if (File.Exists(destFile)) File.Delete(destFile);
            File.Move(sourceFile, destFile);
        }
    }
}

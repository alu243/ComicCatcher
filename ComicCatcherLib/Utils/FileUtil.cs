using System.Text;

namespace ComicCatcherLib.Utils;

public static class FileUtil
{
    public static bool CompareFileByMD5(string tmpFile, string cmpFile)
    {
        if (true == CompareMD5(tmpFile, cmpFile))
        {
            if (File.Exists(cmpFile)) File.Delete(cmpFile);
            return true;
        }
        else
        {
            MoveFile(cmpFile, tmpFile);
            return false;
        }
    }

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
        string file1MD5 = CalcMD5(file1);
        if (string.IsNullOrEmpty(file1MD5)) return false;
        string file2MD5 = CalcMD5(file2);
        if (string.IsNullOrEmpty(file2MD5)) return false;

        return file1MD5.Equals(file2MD5);
    }

    private static void MoveFile(string sourceFile, string destFile)
    {
        if (File.Exists(destFile)) File.Delete(destFile);
        File.Move(sourceFile, destFile);
    }
}
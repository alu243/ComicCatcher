using System;
using ComicCatcher.Helpers;

namespace ComicCatcher.DbModel
{
    public class DownloadedList
    {
        public static void Load()
        {
            DownloadedDao.CreateTableOnFly();
        }
        
        public static bool HasDownloaded(string comicWeb, string comicName, string comicVolumn)
        {
            try
            {
                comicName = comicName.Replace("'", "''");
                comicVolumn = comicVolumn.Replace("'", "''");
                return DownloadedDao.InDownloaded(comicWeb, comicName, comicVolumn);
            }
            catch (Exception ex)
            {
                NLogger.Error($"查詢資料庫時發生錯誤，{comicName}:{comicVolumn}:{ex.Message}");
                return false;
            }
        }

        public static void AddDownloaded(string comicWeb, string comicName, string comicVolumn)
        {
            try
            {
                comicName = comicName.Replace("'", "''");
                comicVolumn = comicVolumn.Replace("'", "''");
                DownloadedDao.AddDownloaded(comicWeb, comicName, comicVolumn);
            }
            catch
            {
                NLogger.Error("資料已存在資料庫中，" + comicName + comicVolumn);
            }
        }
    }
}

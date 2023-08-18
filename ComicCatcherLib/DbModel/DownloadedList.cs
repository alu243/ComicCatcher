namespace ComicCatcherLib.DbModel;

public class DownloadedList
{
    public static void Load()
    {
        DownloadedDao.CreateTableOnFly().Wait();
    }

    public static async Task<bool> HasDownloaded(string comicWeb, string comicName, string comicVolumn)
    {
        try
        {
            comicName = comicName.Replace("'", "''");
            comicVolumn = comicVolumn.Replace("'", "''");
            return await DownloadedDao.InDownloaded(comicWeb, comicName, comicVolumn);
        }
        catch (Exception ex)
        {
            //NLogger.Error($"查詢資料庫時發生錯誤，{comicName}:{comicVolumn}:{ex.Message}");
            return false;
        }
    }

    public static async Task AddDownloaded(string comicWeb, string comicName, string comicVolumn)
    {
        try
        {
            comicName = comicName.Replace("'", "''");
            comicVolumn = comicVolumn.Replace("'", "''");
            await DownloadedDao.AddDownloaded(comicWeb, comicName, comicVolumn);
        }
        catch
        {
            //NLogger.Error("資料已存在資料庫中，" + comicName + comicVolumn);
        }
    }
}
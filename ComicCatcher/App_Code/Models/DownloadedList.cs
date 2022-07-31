using System;
using System.IO;
//using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

using Helpers;
using Utils;
using Models;
using ComicModels;
namespace Models
{
    [Serializable]
    //public class DownloadedList : IXmlSerializable
    public class DownloadedList
    {
        public static void LoadDB()
        {
            SQLiteHelper.CreateDownladedListTableOnFly();
        }

        private Dictionary<string, Dictionary<string, bool>> myList { get; set; }

        public static bool HasDownloaded(string comicWeb, string comicName, string comicVolumn)
        {
            try
            {
                comicName = comicName.Replace("'", "''");
                comicVolumn = comicVolumn.Replace("'", "''");
                return SQLiteHelper.IsInDownloadedList(comicWeb, comicName, comicVolumn);
            }
            catch (Exception ex)
            {
                NLogger.Error($"查詢資料庫時發生錯誤，{comicName}:{comicVolumn}:{ex.Message}");
                return false;
            }
            //if (null == myList) return false;
            //if (myList.ContainsKey(comicName))
            //{
            //    return myList[comicName].ContainsKey(volumnName);
            //}
            //return false;
        }

        public static void AddDownloaded(string comicWeb, string comicName, string comicVolumn)
        {
            try
            {
                comicName = comicName.Replace("'", "''");
                comicVolumn = comicVolumn.Replace("'", "''");
                SQLiteHelper.InsertComicVolumn(comicWeb, comicName, comicVolumn);
            }
            catch
            {
                NLogger.Error("資料已存在資料庫中，" + comicName + comicVolumn);
            }
        }
    }
}

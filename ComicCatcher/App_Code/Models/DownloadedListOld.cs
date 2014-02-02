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
    public class DownloadedListOld
    {
        //////#region IXmlSerializable Members
        //////public System.Xml.Schema.XmlSchema GetSchema()
        //////{
        //////    return null;
        //////}

        //////public void ReadXml(System.Xml.XmlReader reader)
        //////{
        //////    XmlSerializer key1Serializer = new XmlSerializer(typeof(string));
        //////    XmlSerializer key2Serializer = new XmlSerializer(typeof(string));

        //////    bool wasEmpty = reader.IsEmptyElement;
        //////    reader.Read();

        //////    if (wasEmpty)
        //////        return;

        //////    if (null == myList) myList = new Dictionary<string, Dictionary<string, bool>> ();

        //////    while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
        //////    {
        //////        writer.ReadStartElement("item");

        //////        reader.ReadStartElement("comicKey");
        //////        try { string comicKey = (string)key1Serializer.Deserialize(reader); }
        //////        finally { reader.ReadEndElement(); }
        //////        this.Add(key, value);


        //////        reader.ReadStartElement("volumnKey");
        //////        try { string comicKey = (string)key1Serializer.Deserialize(reader); }
        //////        finally { reader.ReadEndElement(); }

        //////        string value = (string)key2Serializer.Deserialize(reader);
        //////        reader.ReadEndElement();


        //////        reader.ReadEndElement();
        //////        reader.MoveToContent();
        //////    }
        //////    reader.ReadEndElement();
        //////}

        //////public void WriteXml(System.Xml.XmlWriter writer)
        //////{
        //////    XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
        //////    XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

        //////    foreach (TKey key in this.Keys)
        //////    {
        //////        writer.WriteStartElement("item");

        //////        writer.WriteStartElement("key");
        //////        keySerializer.Serialize(writer, key);
        //////        writer.WriteEndElement();

        //////        writer.WriteStartElement("value");
        //////        TValue value = this[key];
        //////        valueSerializer.Serialize(writer, value);
        //////        writer.WriteEndElement();

        //////        writer.WriteEndElement();
        //////    }
        //////}

        [Obsolete("序列化的設定已不再使用")]
        private static string filename { get { return @"donwloadedlist.bin"; } }

        //[Obsolete("序列化的設定已不再使用")]
        //public void savexml()
        //{
        //    using (FileStream fs = new FileStream(filename, FileMode.Create))
        //    {
        //        //Console.WriteLine("準備序列化物件");
        //        BinaryFormatter xmlSerializer = new BinaryFormatter();
        //        xmlSerializer.Serialize(fs, this);
        //        fs.Flush();
        //        fs.Close();
        //    }
        //}

        [Obsolete("序列化的設定已不再使用")]
        public static DownloadedListOld loadxml()
        {
            using (FileStream oFileStream = new FileStream(filename, FileMode.Open))
            {
                DownloadedListOld o = null;
                //Console.WriteLine("準備還原序列化物件");
                BinaryFormatter xmlSerializer = new BinaryFormatter();
                o = (DownloadedListOld)xmlSerializer.Deserialize(oFileStream);
                oFileStream.Close();
                //Console.WriteLine("還原完成");
                //this = o;
                return o;
            }
        }

        public static void ImportToDB()
        {
            string filePath = @".\donwloadedlist.bin";
            if (File.Exists(filePath))
            {
                DownloadedListOld.LoadDB();
                DownloadedListOld dwnedList = DownloadedListOld.loadxml();
                dwnedList.Import();
                File.Delete(filePath);
            }
        }

        public void Import()
        {
            myList.Keys.ToList().ForEach(k =>
            {
                myList[k].Keys.ToList().ForEach(k2 =>
                {
                    AddDownloaded(XindmWebSite.WebSiteName, k, k2);
                });
            });
        }


        public static void LoadDB()
        {
            SQLiteHelper.CreateDownladedListTableOnFly();
        }

        private Dictionary<string, Dictionary<string, bool>> myList { get; set; }

        public static void AddDownloaded(string comicWeb, string comicName, string comicVolumn)
        {
            try
            {
                comicName = comicName.Replace("'", "''");
                comicVolumn = comicVolumn.Replace("'", "''");
                SQLiteHelper.InsertComicVolumn(comicWeb, comicName, comicVolumn);
            }
            catch (Exception ex)
            {
                NLogger.Error("資料已存在資料庫中，" + comicName + comicVolumn);
            }
            //return;
            //if (null == myList) myList = new Dictionary<string, Dictionary<string, bool>>();
            //if (false == myList.ContainsKey(comicName))
            //{
            //    myList.Add(comicName, new Dictionary<string, bool>());
            //}
            //if (false == myList[comicName].ContainsKey(volumnName))
            //{
            //    myList[comicName].Add(volumnName, true);
            //}
        }
    }
}

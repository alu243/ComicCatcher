﻿using System;
using System.IO;
//using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
namespace ComicCatcher
{
    [Serializable]
    //public class DownloadedList : IXmlSerializable
    public class DownloadedList
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

        private static string filename { get { return @"donwloadedlist.bin"; } }
        public void save()
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                //Console.WriteLine("準備序列化物件");
                BinaryFormatter xmlSerializer = new BinaryFormatter();
                xmlSerializer.Serialize(fs, this);
                fs.Flush();
                fs.Close();
            }
        }

        public static DownloadedList load()
        {
            using (FileStream oFileStream = new FileStream(filename, FileMode.Open))
            {
                DownloadedList o = null;
                //Console.WriteLine("準備還原序列化物件");
                BinaryFormatter xmlSerializer = new BinaryFormatter();
                o = (DownloadedList)xmlSerializer.Deserialize(oFileStream);
                oFileStream.Close();
                //Console.WriteLine("還原完成");
                //this = o;
                return o;
            }
        }

        public Dictionary<string, Dictionary<string, bool>> myList { get; set; }

        public bool HasDownloaded(string comicName, string volumnName)
        {
            if (null == myList) return false;
            if (myList.ContainsKey(comicName))
            {
                return myList[comicName].ContainsKey(volumnName);
            }
            return false;
        }

        public void AddDownloaded(string comicName, string volumnName)
        {
            if (null == myList) myList = new Dictionary<string, Dictionary<string, bool>>();
            if (false == myList.ContainsKey(comicName))
            {
                myList.Add(comicName, new Dictionary<string, bool>());
            }
            if (false == myList[comicName].ContainsKey(volumnName))
            {
                myList[comicName].Add(volumnName, true);
            }
        }
    }
}

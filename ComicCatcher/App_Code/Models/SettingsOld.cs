﻿using System;
using System.IO;
using System.Xml.Serialization;

namespace ComicCatcher.Models
{
    [Serializable]
    public class SettingsOld
    {
        private static string filename { get { return @"settings.xml"; } }
        public void save()
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                //Console.WriteLine("準備序列化物件");
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsOld));
                xmlSerializer.Serialize(fs, this);
                fs.Close();
            }
        }

        public static SettingsOld load()
        {
            using (FileStream oFileStream = new FileStream(filename, FileMode.Open))
            {
                SettingsOld o = null;
                //Console.WriteLine("準備還原序列化物件");
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsOld));
                o = (SettingsOld)xmlSerializer.Deserialize(oFileStream);
                oFileStream.Close();
                //Console.WriteLine("還原完成");
                //this = o;
                return o;
            }
        }

        private string _photoProgramPath;
        public string PhotoProgramPath
        {
            get { return String.IsNullOrEmpty(this._photoProgramPath) ? @"C:\Program Files\Honeyview\Honeyview.exe" : this._photoProgramPath; }
            set { this._photoProgramPath = value; }
        }
        private string _winRARPath;
        public string WinRARPath
        {
            get { return String.IsNullOrEmpty(this._winRARPath) ? @"C:\Program Files\WinRAR\WinRAR.exe" : this._winRARPath; }
            set { this._winRARPath = value; }
        }

        private string _localPath;
        public string LocalPath
        {
            get { return String.IsNullOrEmpty(this._localPath) ? @"Q:\Comic\ComicShelf" : this._localPath; }
            set { this._localPath = value; }
        }

        private bool? _usingProxy;
        public bool UsingProxy
        {
            get { return this._usingProxy ?? true; }
            set { this._usingProxy = value; }
        }

        private string _proxyUrl;
        public string ProxyUrl
        {
            get { return String.IsNullOrEmpty(this._proxyUrl) ? @"proxy.hinet.net" : this._proxyUrl; }
            set { this._proxyUrl = value; }
        }

        private int? _proxyPort;
        public int ProxyPort
        {
            get { return this._proxyPort ?? 80; }
            set { this._proxyPort = value; }
        }

        private bool? _loadAllPicture;
        public bool LoadAllPicture
        {
            get { return this._loadAllPicture ?? true; }
            set { this._loadAllPicture = value; }
        }

        private bool? _backGroundLoadNode;
        public bool BackGroundLoadNode
        {
            get { return this._backGroundLoadNode ?? false; }
            set { this._backGroundLoadNode = value; }
        }

        private bool? _saveWebSiteName;
        public bool SaveWebSiteName
        {
            get { return this._saveWebSiteName ?? false; }
            set { this._saveWebSiteName = value; }
        }

        private bool? _srchiveDownloadedFile;
        public bool ArchiveDownloadedFile
        {
            get { return this._srchiveDownloadedFile ?? false; }
            set { this._srchiveDownloadedFile = value; }
        }
    }
}

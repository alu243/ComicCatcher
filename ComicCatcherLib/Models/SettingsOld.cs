using System.Xml.Serialization;

namespace ComicCatcherLib.Models;

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
        get { return string.IsNullOrEmpty(_photoProgramPath) ? @"C:\Program Files\Honeyview\Honeyview.exe" : _photoProgramPath; }
        set { _photoProgramPath = value; }
    }
    private string _winRARPath;
    public string WinRARPath
    {
        get { return string.IsNullOrEmpty(_winRARPath) ? @"C:\Program Files\WinRAR\WinRAR.exe" : _winRARPath; }
        set { _winRARPath = value; }
    }

    private string _localPath;
    public string LocalPath
    {
        get { return string.IsNullOrEmpty(_localPath) ? @"Q:\Comic\ComicShelf" : _localPath; }
        set { _localPath = value; }
    }

    private bool? _usingProxy;
    public bool UsingProxy
    {
        get { return _usingProxy ?? true; }
        set { _usingProxy = value; }
    }

    private string _proxyUrl;
    public string ProxyUrl
    {
        get { return string.IsNullOrEmpty(_proxyUrl) ? @"proxy.hinet.net" : _proxyUrl; }
        set { _proxyUrl = value; }
    }

    private int? _proxyPort;
    public int ProxyPort
    {
        get { return _proxyPort ?? 80; }
        set { _proxyPort = value; }
    }

    private bool? _loadAllPicture;
    public bool LoadAllPicture
    {
        get { return _loadAllPicture ?? true; }
        set { _loadAllPicture = value; }
    }

    private bool? _backGroundLoadNode;
    public bool BackGroundLoadNode
    {
        get { return _backGroundLoadNode ?? false; }
        set { _backGroundLoadNode = value; }
    }

    private bool? _saveWebSiteName;
    public bool SaveWebSiteName
    {
        get { return _saveWebSiteName ?? false; }
        set { _saveWebSiteName = value; }
    }

    private bool? _srchiveDownloadedFile;
    public bool ArchiveDownloadedFile
    {
        get { return _srchiveDownloadedFile ?? false; }
        set { _srchiveDownloadedFile = value; }
    }
}
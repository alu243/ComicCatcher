using System.Text.Json;

namespace ComicCatcherLib.DbModel;

public class Settings
{
    public async Task<bool> Save()
    {
        var settingString = JsonSerializer.Serialize(this);
        return await SettingsDao.SaveSettings(settingString);
    }

    public static async Task<Settings> Load()
    {
        SettingsDao.CreateSettingsTableOnFly();
        var settingString = await SettingsDao.GetSettings();
        if (string.IsNullOrEmpty(settingString)) return new Settings();
        return JsonSerializer.Deserialize<Settings>(settingString)!;
    }

    public string PhotoProgramPath { get; set; } = @"C:\Program Files\Honeyview\Honeyview.exe";
    public string WinRARPath { get; set; } = @"C:\Program Files\WinRAR\WinRAR.exe";
    public string LocalPath { get; set; } = @"Q:\Comic\ComicShelf";
    public bool UsingProxy { get; set; } = false;
    public string ProxyUrl { get; set; } = @"proxy.hinet.net";
    public int ProxyPort { get; set; } = 80;
    public bool LoadAllPicture { get; set; } = true;
    public bool BackGroundLoadNode { get; set; } = false;
    public bool SaveWebSiteName { get; set; } = false;
    public bool ArchiveDownloadedFile { get; set; } = false;
}
namespace XIVMultiLauncher.Models;

public class AppConfig
{
    public string XIVLauncherPath { get; set; } = GetDefaultXIVLauncherPath();
    public int SleepTimeSeconds { get; set; } = 10;
    public bool AutoKillMutants { get; set; } = true;
    public bool AutoCreateProfiles { get; set; } = true;
    public string? TemplateProfilePath { get; set; }
    public string ProfilesBasePath { get; set; } = GetDefaultProfilesPath();
    public List<Profile> Profiles { get; set; } = new();

    private static string GetDefaultXIVLauncherPath()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "XIVLauncher", "XIVLauncher.exe");
    }

    private static string GetDefaultProfilesPath()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "XIVMultiLauncher", "Profiles");
    }
}

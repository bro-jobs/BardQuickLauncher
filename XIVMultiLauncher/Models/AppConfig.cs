namespace XIVMultiLauncher.Models;

public class AppConfig
{
    public string XIVLauncherPath { get; set; } = GetDefaultXIVLauncherPath();
    public int SleepTimeSeconds { get; set; } = 10;
    public List<Profile> Profiles { get; set; } = new();

    private static string GetDefaultXIVLauncherPath()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "XIVLauncher", "XIVLauncher.exe");
    }
}

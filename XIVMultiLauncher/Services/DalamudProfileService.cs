using System.Text.Json;
using System.Text.Json.Nodes;

namespace XIVMultiLauncher.Services;

/// <summary>
/// Manages Dalamud profile folders and plugin configurations.
/// </summary>
public class DalamudProfileService
{
    private readonly string _baseProfilePath;

    public DalamudProfileService()
    {
        // Default base path for profiles - user can customize
        _baseProfilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "XIVMultiLauncher",
            "Profiles");
    }

    public string BaseProfilePath => _baseProfilePath;

    /// <summary>
    /// Gets the default profile path for an account.
    /// </summary>
    public string GetProfilePath(string accountName)
    {
        return Path.Combine(_baseProfilePath, SanitizeFolderName(accountName));
    }

    /// <summary>
    /// Creates a new Dalamud profile folder for an account.
    /// </summary>
    public void CreateProfile(string accountName, string? templatePath = null)
    {
        var profilePath = GetProfilePath(accountName);

        // Create the profile directory structure
        Directory.CreateDirectory(profilePath);
        Directory.CreateDirectory(Path.Combine(profilePath, "pluginConfigs"));

        // Copy from template if provided
        if (!string.IsNullOrEmpty(templatePath) && Directory.Exists(templatePath))
        {
            CopyProfileContents(templatePath, profilePath, excludeAutoLogin: true);
        }
    }

    /// <summary>
    /// Copies profile contents from source to destination, optionally excluding AutoLogin config.
    /// </summary>
    public void CopyProfileContents(string sourcePath, string destPath, bool excludeAutoLogin = true)
    {
        // Top-level files to copy
        string[] topLevelFiles = {
            "launcherConfigV3.json",  // XIVLauncher config (required to skip first-time setup)
            "dalamudConfig.json",      // Dalamud settings
            "dalamudUI.ini",           // Dalamud window positions
            "dalamudVfs.db"            // Plugin collections and other info
        };

        foreach (var fileName in topLevelFiles)
        {
            var sourceFile = Path.Combine(sourcePath, fileName);
            if (File.Exists(sourceFile))
            {
                File.Copy(sourceFile, Path.Combine(destPath, fileName), overwrite: true);
            }
        }

        // Folders to copy recursively
        string[] foldersToRecursivelyCopy = {
            "installedPlugins",  // Installed plugin DLLs
            "addon"              // Dalamud hooks and other addons
        };

        foreach (var folderName in foldersToRecursivelyCopy)
        {
            var sourceFolder = Path.Combine(sourcePath, folderName);
            var destFolder = Path.Combine(destPath, folderName);

            if (Directory.Exists(sourceFolder))
            {
                CopyDirectoryRecursive(sourceFolder, destFolder);
            }
        }

        // Copy pluginConfigs folder (with AutoLogin exclusion option)
        var sourcePluginConfigs = Path.Combine(sourcePath, "pluginConfigs");
        var destPluginConfigs = Path.Combine(destPath, "pluginConfigs");

        if (Directory.Exists(sourcePluginConfigs))
        {
            CopyDirectoryRecursive(sourcePluginConfigs, destPluginConfigs, excludeFile: filePath =>
            {
                if (!excludeAutoLogin) return false;
                var fileName = Path.GetFileName(filePath);
                return fileName.Equals("AutoLogin.json", StringComparison.OrdinalIgnoreCase);
            });
        }
    }

    /// <summary>
    /// Recursively copies a directory and all its contents.
    /// </summary>
    private void CopyDirectoryRecursive(string sourceDir, string destDir, Func<string, bool>? excludeFile = null)
    {
        Directory.CreateDirectory(destDir);

        // Copy all files
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            if (excludeFile?.Invoke(file) == true)
                continue;

            var fileName = Path.GetFileName(file);
            var destFile = Path.Combine(destDir, fileName);
            File.Copy(file, destFile, overwrite: true);
        }

        // Recursively copy subdirectories
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(subDir);
            var destSubDir = Path.Combine(destDir, dirName);
            CopyDirectoryRecursive(subDir, destSubDir, excludeFile);
        }
    }

    /// <summary>
    /// Generates and saves an AutoLogin plugin configuration.
    /// </summary>
    public void SaveAutoLoginConfig(string profilePath, uint dataCenterId, uint worldId, uint characterSlot)
    {
        var pluginConfigsPath = Path.Combine(profilePath, "pluginConfigs");
        Directory.CreateDirectory(pluginConfigsPath);

        var config = new JsonObject
        {
            ["$type"] = "AutoLogin.Configuration, AutoLogin",
            ["Version"] = 1,
            ["DataCenter"] = dataCenterId,
            ["World"] = worldId,
            ["CharacterSlot"] = characterSlot,
            ["RelogAfterDisconnect"] = true,
            ["SeenReconnectionExplanation"] = true,
            ["SendNotif"] = false,
            ["WebhookURL"] = "",
            ["WebhookMessage"] = "[AutoLogin] Your game has lost connection!",
            ["DebugMode"] = false,
            ["LastErrorCode"] = 0,
            ["CurrentError"] = "none"
        };

        var configPath = Path.Combine(pluginConfigsPath, "AutoLogin.json");
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(configPath, config.ToJsonString(options));
    }

    /// <summary>
    /// Checks if a profile folder exists and has the expected structure.
    /// </summary>
    public bool ProfileExists(string profilePath)
    {
        if (string.IsNullOrEmpty(profilePath))
            return false;

        return Directory.Exists(profilePath) &&
               Directory.Exists(Path.Combine(profilePath, "pluginConfigs"));
    }

    /// <summary>
    /// Checks if XIVLauncher config exists (required to skip first-time setup).
    /// </summary>
    public bool HasLauncherConfig(string profilePath)
    {
        var configPath = Path.Combine(profilePath, "launcherConfigV3.json");
        return File.Exists(configPath);
    }

    /// <summary>
    /// Checks if AutoLogin is configured for this profile.
    /// </summary>
    public bool HasAutoLoginConfig(string profilePath)
    {
        var configPath = Path.Combine(profilePath, "pluginConfigs", "AutoLogin.json");
        return File.Exists(configPath);
    }

    private static string SanitizeFolderName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", name.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
    }
}

using System.Diagnostics;
using System.Net.Http;
using XIVMultiLauncher.Models;

namespace XIVMultiLauncher.Services;

public class LauncherService
{
    private readonly HttpClient _httpClient = new();

    public void LaunchProfile(string xivLauncherPath, Profile profile)
    {
        var args = new List<string>
        {
            $"--account={profile.AccountId}"
        };

        if (!string.IsNullOrEmpty(profile.RoamingPath))
        {
            args.Add($"--roamingPath={profile.RoamingPath}");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = xivLauncherPath,
            Arguments = string.Join(" ", args),
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }

    public async Task SendOtpAsync(string otp)
    {
        if (string.IsNullOrEmpty(otp) || otp.Length != 6)
            throw new ArgumentException("OTP must be 6 digits");

        try
        {
            await _httpClient.GetAsync($"http://127.0.0.1:4646/ffxivlauncher/{otp}");
        }
        catch (HttpRequestException)
        {
            throw new Exception(
                "Failed to connect to XIVLauncher OTP listener.\n" +
                "Make sure XIVLauncher has 'Enable XL Authenticator app/OTP macro support' enabled.");
        }
    }

    public string? GetOtpFrom1Password(string profileName)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "op",
                Arguments = $"item get {profileName} --otp",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null) return null;

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return process.ExitCode == 0 ? output.Trim() : null;
        }
        catch
        {
            return null;
        }
    }
}

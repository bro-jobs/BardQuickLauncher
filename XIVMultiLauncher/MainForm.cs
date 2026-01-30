using XIVMultiLauncher.Data;
using XIVMultiLauncher.Models;
using XIVMultiLauncher.Services;

namespace XIVMultiLauncher;

public partial class MainForm : Form
{
    private readonly ConfigService _configService = new();
    private readonly LauncherService _launcherService = new();
    private readonly MutantKillerService _mutantKillerService = new();
    private readonly DalamudProfileService _profileService = new();
    private AppConfig _config = new();

    public MainForm()
    {
        InitializeComponent();
        LoadConfig();
    }

    private void LoadConfig()
    {
        _config = _configService.Load();
        txtXIVLauncherPath.Text = _config.XIVLauncherPath;
        txtTemplatePath.Text = _config.TemplateProfilePath ?? "";
        numSleepTime.Value = _config.SleepTimeSeconds;
        chkKillMutants.Checked = _config.AutoKillMutants;
        RefreshProfileList();
    }

    private void SaveConfig()
    {
        _config.XIVLauncherPath = txtXIVLauncherPath.Text;
        _config.TemplateProfilePath = string.IsNullOrWhiteSpace(txtTemplatePath.Text)
            ? null : txtTemplatePath.Text;
        _config.SleepTimeSeconds = (int)numSleepTime.Value;
        _config.AutoKillMutants = chkKillMutants.Checked;
        _configService.Save(_config);
    }

    private void RefreshProfileList()
    {
        lstProfiles.Items.Clear();
        var hasWarnings = false;

        foreach (var profile in _config.Profiles)
        {
            var item = new ListViewItem(profile.DisplayName);
            item.SubItems.Add(profile.AccountName);
            item.SubItems.Add(profile.IsMainAccount ? "Yes" : "");
            item.SubItems.Add(profile.IsSteam ? "Yes" : "");
            item.SubItems.Add(profile.UseOtp ? "Yes" : "");

            // Check profile status
            var status = GetProfileStatus(profile);
            item.SubItems.Add(status);

            // Highlight warnings
            if (status != "OK")
            {
                item.BackColor = Color.LightYellow;
                hasWarnings = true;
            }

            item.Tag = profile;
            lstProfiles.Items.Add(item);
        }

        // Show warning if any profiles have issues
        if (hasWarnings)
        {
            lblStatus.Text = "Warning: Some profiles have missing folders or configs";
            lblStatus.ForeColor = Color.DarkOrange;
        }
        else
        {
            lblStatus.Text = "Ready";
            lblStatus.ForeColor = SystemColors.ControlText;
        }
    }

    private string GetProfileStatus(Profile profile)
    {
        if (string.IsNullOrEmpty(profile.RoamingPath))
            return "No path";

        if (!Directory.Exists(profile.RoamingPath))
            return "Missing folder";

        if (!_profileService.HasLauncherConfig(profile.RoamingPath))
            return "No launcher cfg";

        if (!_profileService.HasAutoLoginConfig(profile.RoamingPath))
        {
            if (profile.DataCenterId > 0 && profile.WorldId > 0)
                return "No AutoLogin";
        }

        return "OK";
    }

    private void btnBrowse_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Executable files (*.exe)|*.exe",
            Title = "Select XIVLauncher.exe"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtXIVLauncherPath.Text = dialog.FileName;
            SaveConfig();
        }
    }

    private void btnBrowseTemplate_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select your main account's XIVLauncher folder (contains launcherConfigV3.json)",
            UseDescriptionForTitle = true
        };

        // Default to standard XIVLauncher path
        var defaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "XIVLauncher");
        if (Directory.Exists(defaultPath))
            dialog.SelectedPath = defaultPath;

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            // Validate it looks like an XIVLauncher folder
            var launcherConfig = Path.Combine(dialog.SelectedPath, "launcherConfigV3.json");
            if (!File.Exists(launcherConfig))
            {
                var result = MessageBox.Show(
                    "This folder doesn't contain launcherConfigV3.json.\n" +
                    "New profiles won't skip the first-time setup.\n\n" +
                    "Use this folder anyway?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;
            }

            txtTemplatePath.Text = dialog.SelectedPath;
            SaveConfig();
        }
    }

    private void btnAddProfile_Click(object? sender, EventArgs e)
    {
        using var dialog = new ProfileDialog(_config);
        if (dialog.ShowDialog() == DialogResult.OK && dialog.Profile != null)
        {
            _config.Profiles.Add(dialog.Profile);
            SaveConfig();
            RefreshProfileList();
        }
    }

    private void btnEditProfile_Click(object? sender, EventArgs e)
    {
        if (lstProfiles.SelectedItems.Count == 0) return;

        var profile = (Profile)lstProfiles.SelectedItems[0].Tag!;
        using var dialog = new ProfileDialog(_config, profile);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            SaveConfig();
            RefreshProfileList();
        }
    }

    private void btnRemoveProfile_Click(object? sender, EventArgs e)
    {
        if (lstProfiles.SelectedItems.Count == 0) return;

        var profile = (Profile)lstProfiles.SelectedItems[0].Tag!;
        var result = MessageBox.Show(
            $"Remove profile '{profile.DisplayName}'?",
            "Confirm",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _config.Profiles.Remove(profile);
            SaveConfig();
            RefreshProfileList();
        }
    }

    private void btnMoveUp_Click(object? sender, EventArgs e)
    {
        if (lstProfiles.SelectedItems.Count == 0) return;
        var index = lstProfiles.SelectedIndices[0];
        if (index == 0) return;

        (_config.Profiles[index], _config.Profiles[index - 1]) =
            (_config.Profiles[index - 1], _config.Profiles[index]);
        SaveConfig();
        RefreshProfileList();
        lstProfiles.Items[index - 1].Selected = true;
    }

    private void btnMoveDown_Click(object? sender, EventArgs e)
    {
        if (lstProfiles.SelectedItems.Count == 0) return;
        var index = lstProfiles.SelectedIndices[0];
        if (index >= _config.Profiles.Count - 1) return;

        (_config.Profiles[index], _config.Profiles[index + 1]) =
            (_config.Profiles[index + 1], _config.Profiles[index]);
        SaveConfig();
        RefreshProfileList();
        lstProfiles.Items[index + 1].Selected = true;
    }

    private async void btnLaunchAll_Click(object? sender, EventArgs e)
    {
        if (_config.Profiles.Count == 0)
        {
            MessageBox.Show("No profiles configured.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!File.Exists(_config.XIVLauncherPath))
        {
            MessageBox.Show("XIVLauncher.exe not found. Please check the path.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        SetUIEnabled(false);
        progressBar.Value = 0;
        progressBar.Maximum = _config.Profiles.Count;

        try
        {
            for (var i = 0; i < _config.Profiles.Count; i++)
            {
                var profile = _config.Profiles[i];
                lblStatus.Text = $"Launching {profile.DisplayName}...";

                // Handle OTP if configured
                if (profile.UseOtp && !string.IsNullOrEmpty(profile.OtpProvider))
                {
                    await HandleOtpAsync(profile);
                }

                _launcherService.LaunchProfile(_config.XIVLauncherPath, profile);
                progressBar.Value = i + 1;

                // Kill mutants after launch to allow more instances
                if (_config.AutoKillMutants && i < _config.Profiles.Count - 1)
                {
                    // Wait a moment for the game to create its mutants
                    await Task.Delay(2000);
                    lblStatus.Text = $"Killing mutants for {profile.DisplayName}...";
                    var killed = _mutantKillerService.KillFfxivMutants();
                    if (killed > 0)
                        lblStatus.Text = $"Killed {killed} mutant(s)";
                }

                // If main account, wait for user confirmation
                if (profile.IsMainAccount && i < _config.Profiles.Count - 1)
                {
                    lblStatus.Text = "Waiting for main account to reach main menu...";
                    var result = MessageBox.Show(
                        $"Main account '{profile.DisplayName}' launched.\n\n" +
                        "Click OK when you're at the main menu to continue launching other accounts.\n" +
                        "(This prevents XIVLauncher from refusing more than 2 instances)",
                        "Main Account",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Cancel)
                    {
                        lblStatus.Text = "Launch cancelled.";
                        return;
                    }
                }
                // Wait between launches (except for last one)
                else if (i < _config.Profiles.Count - 1)
                {
                    for (var s = _config.SleepTimeSeconds; s > 0; s--)
                    {
                        lblStatus.Text = $"Waiting {s}s before next launch...";
                        await Task.Delay(1000);
                    }
                }
            }

            lblStatus.Text = "All profiles launched!";
            MessageBox.Show("All profiles have been launched!", "Done",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Error occurred.";
        }
        finally
        {
            SetUIEnabled(true);
        }
    }

    private async void btnLaunchSelected_Click(object? sender, EventArgs e)
    {
        if (lstProfiles.SelectedItems.Count == 0) return;

        if (!File.Exists(_config.XIVLauncherPath))
        {
            MessageBox.Show("XIVLauncher.exe not found. Please check the path.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var profile = (Profile)lstProfiles.SelectedItems[0].Tag!;
        SetUIEnabled(false);
        lblStatus.Text = $"Launching {profile.DisplayName}...";

        try
        {
            if (profile.UseOtp && !string.IsNullOrEmpty(profile.OtpProvider))
            {
                await HandleOtpAsync(profile);
            }

            _launcherService.LaunchProfile(_config.XIVLauncherPath, profile);
            lblStatus.Text = $"Launched {profile.DisplayName}!";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Error occurred.";
        }
        finally
        {
            SetUIEnabled(true);
        }
    }

    private async Task HandleOtpAsync(Profile profile)
    {
        if (string.IsNullOrEmpty(profile.OtpProvider)) return;

        var parts = profile.OtpProvider.Split(':');
        if (parts.Length < 2) return;

        var provider = parts[0].ToLower();
        var value = string.Join(":", parts.Skip(1));

        string? otp = null;

        switch (provider)
        {
            case "1password":
                lblStatus.Text = $"Getting OTP from 1Password for {profile.DisplayName}...";
                otp = _launcherService.GetOtpFrom1Password(value);
                break;
        }

        if (!string.IsNullOrEmpty(otp))
        {
            lblStatus.Text = $"Sending OTP for {profile.DisplayName}...";
            await Task.Delay(3000); // Wait for XIVLauncher to show OTP dialog
            await _launcherService.SendOtpAsync(otp);
        }
    }

    private void SetUIEnabled(bool enabled)
    {
        btnLaunchAll.Enabled = enabled;
        btnLaunchSelected.Enabled = enabled;
        btnAddProfile.Enabled = enabled;
        btnEditProfile.Enabled = enabled;
        btnRemoveProfile.Enabled = enabled;
        btnMoveUp.Enabled = enabled;
        btnMoveDown.Enabled = enabled;
    }

    private void numSleepTime_ValueChanged(object? sender, EventArgs e)
    {
        SaveConfig();
    }

    private void chkKillMutants_CheckedChanged(object? sender, EventArgs e)
    {
        SaveConfig();
    }

    private void btnKillMutants_Click(object? sender, EventArgs e)
    {
        lblStatus.Text = "Killing FFXIV mutants...";
        try
        {
            var killed = _mutantKillerService.KillFfxivMutants();
            lblStatus.Text = killed > 0
                ? $"Killed {killed} mutant(s). You can now launch more instances."
                : "No FFXIV mutants found (no game running or already cleared).";
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Error: {ex.Message}";
            MessageBox.Show(
                $"Failed to kill mutants: {ex.Message}\n\nYou may need to run as Administrator.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void lstProfiles_DoubleClick(object? sender, EventArgs e)
    {
        btnEditProfile_Click(sender, e);
    }
}

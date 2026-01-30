using XIVMultiLauncher.Models;
using XIVMultiLauncher.Services;

namespace XIVMultiLauncher;

public partial class MainForm : Form
{
    private readonly ConfigService _configService = new();
    private readonly LauncherService _launcherService = new();
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
        numSleepTime.Value = _config.SleepTimeSeconds;
        RefreshProfileList();
    }

    private void SaveConfig()
    {
        _config.XIVLauncherPath = txtXIVLauncherPath.Text;
        _config.SleepTimeSeconds = (int)numSleepTime.Value;
        _configService.Save(_config);
    }

    private void RefreshProfileList()
    {
        lstProfiles.Items.Clear();
        foreach (var profile in _config.Profiles)
        {
            var item = new ListViewItem(profile.DisplayName);
            item.SubItems.Add(profile.AccountName);
            item.SubItems.Add(profile.IsMainAccount ? "Yes" : "");
            item.SubItems.Add(profile.IsSteam ? "Yes" : "");
            item.SubItems.Add(profile.UseOtp ? "Yes" : "");
            item.Tag = profile;
            lstProfiles.Items.Add(item);
        }
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

    private void btnAddProfile_Click(object? sender, EventArgs e)
    {
        using var dialog = new ProfileDialog();
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
        using var dialog = new ProfileDialog(profile);
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

    private void lstProfiles_DoubleClick(object? sender, EventArgs e)
    {
        btnEditProfile_Click(sender, e);
    }
}

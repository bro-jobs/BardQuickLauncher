using XIVMultiLauncher.Data;
using XIVMultiLauncher.Models;
using XIVMultiLauncher.Services;

namespace XIVMultiLauncher;

public partial class ProfileDialog : Form
{
    public Profile? Profile { get; private set; }

    private readonly DalamudProfileService _profileService;
    private readonly AppConfig _config;
    private readonly bool _isNewProfile;

    public ProfileDialog(AppConfig config, Profile? existingProfile = null)
    {
        _config = config;
        _profileService = new DalamudProfileService();
        _isNewProfile = existingProfile == null;

        InitializeComponent();
        PopulateDataCenters();

        if (existingProfile != null)
        {
            Profile = existingProfile;
            txtAccountName.Text = existingProfile.AccountName;
            txtDisplayName.Text = existingProfile.DisplayName;
            chkMainAccount.Checked = existingProfile.IsMainAccount;
            chkSteam.Checked = existingProfile.IsSteam;
            chkUseOtp.Checked = existingProfile.UseOtp;
            txtRoamingPath.Text = existingProfile.RoamingPath ?? "";
            txtOtpProvider.Text = existingProfile.OtpProvider ?? "";

            // Set character selection
            SelectDataCenter(existingProfile.DataCenterId);
            SelectWorld(existingProfile.WorldId);
            cmbCharacterSlot.SelectedIndex = (int)existingProfile.CharacterSlot;

            this.Text = "Edit Profile";
            chkAutoCreateProfile.Visible = false;
        }
        else
        {
            this.Text = "Add Profile";
            cmbCharacterSlot.SelectedIndex = 0;
            chkAutoCreateProfile.Checked = _config.AutoCreateProfiles;
        }

        UpdateOtpFieldsEnabled();
        UpdateAutoCreateEnabled();
    }

    private void PopulateDataCenters()
    {
        cmbDataCenter.Items.Clear();
        cmbDataCenter.Items.Add("(Select Data Center)");

        foreach (var dc in GameData.DataCenters.OrderBy(d => d.Region).ThenBy(d => d.Name))
        {
            cmbDataCenter.Items.Add($"{dc.Name} ({dc.Region})");
        }

        cmbDataCenter.SelectedIndex = 0;

        // Populate character slots
        cmbCharacterSlot.Items.Clear();
        for (int i = 1; i <= 8; i++)
        {
            cmbCharacterSlot.Items.Add($"Slot {i}");
        }
    }

    private void SelectDataCenter(uint id)
    {
        var dc = GameData.GetDataCenter(id);
        if (dc == null)
        {
            cmbDataCenter.SelectedIndex = 0;
            return;
        }

        for (int i = 1; i < cmbDataCenter.Items.Count; i++)
        {
            if (cmbDataCenter.Items[i]?.ToString()?.StartsWith(dc.Name) == true)
            {
                cmbDataCenter.SelectedIndex = i;
                PopulateWorlds(id);
                return;
            }
        }
    }

    private void SelectWorld(uint id)
    {
        var world = GameData.GetWorld(id);
        if (world == null)
        {
            cmbWorld.SelectedIndex = 0;
            return;
        }

        for (int i = 1; i < cmbWorld.Items.Count; i++)
        {
            if (cmbWorld.Items[i]?.ToString() == world.Name)
            {
                cmbWorld.SelectedIndex = i;
                return;
            }
        }
    }

    private void PopulateWorlds(uint dataCenterId)
    {
        cmbWorld.Items.Clear();
        cmbWorld.Items.Add("(Select World)");

        foreach (var world in GameData.GetWorldsForDataCenter(dataCenterId))
        {
            cmbWorld.Items.Add(world.Name);
        }

        cmbWorld.SelectedIndex = 0;
        cmbWorld.Enabled = true;
    }

    private void cmbDataCenter_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbDataCenter.SelectedIndex <= 0)
        {
            cmbWorld.Items.Clear();
            cmbWorld.Items.Add("(Select Data Center first)");
            cmbWorld.SelectedIndex = 0;
            cmbWorld.Enabled = false;
            return;
        }

        // Parse the datacenter name from the combo item
        var selectedText = cmbDataCenter.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedText)) return;

        var dcName = selectedText.Split(' ')[0];
        var dc = GameData.GetDataCenterByName(dcName);
        if (dc != null)
        {
            PopulateWorlds(dc.Id);
        }
    }

    private void btnOK_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtAccountName.Text))
        {
            MessageBox.Show("Account name is required.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtAccountName.Focus();
            return;
        }

        // Get selected DC and World IDs
        uint dataCenterId = 0;
        uint worldId = 0;

        if (cmbDataCenter.SelectedIndex > 0)
        {
            var dcName = cmbDataCenter.SelectedItem?.ToString()?.Split(' ')[0];
            var dc = GameData.GetDataCenterByName(dcName ?? "");
            if (dc != null) dataCenterId = dc.Id;
        }

        if (cmbWorld.SelectedIndex > 0)
        {
            var worldName = cmbWorld.SelectedItem?.ToString();
            var world = GameData.GetWorldByName(worldName ?? "");
            if (world != null) worldId = world.Id;
        }

        Profile ??= new Profile();
        Profile.AccountName = txtAccountName.Text.Trim().ToLower();
        Profile.DisplayName = string.IsNullOrWhiteSpace(txtDisplayName.Text)
            ? Profile.AccountName
            : txtDisplayName.Text.Trim();
        Profile.IsMainAccount = chkMainAccount.Checked;
        Profile.IsSteam = chkSteam.Checked;
        Profile.UseOtp = chkUseOtp.Checked;
        Profile.OtpProvider = string.IsNullOrWhiteSpace(txtOtpProvider.Text)
            ? null
            : txtOtpProvider.Text.Trim();
        Profile.DataCenterId = dataCenterId;
        Profile.WorldId = worldId;
        Profile.CharacterSlot = (uint)cmbCharacterSlot.SelectedIndex;

        // Handle roaming path and profile creation
        if (_isNewProfile && chkAutoCreateProfile.Checked)
        {
            var profilePath = Path.Combine(_config.ProfilesBasePath, Profile.AccountName);
            Profile.RoamingPath = profilePath;

            try
            {
                // Create profile folder
                Directory.CreateDirectory(profilePath);
                Directory.CreateDirectory(Path.Combine(profilePath, "pluginConfigs"));

                // Copy from template if specified
                if (!string.IsNullOrEmpty(_config.TemplateProfilePath) &&
                    Directory.Exists(_config.TemplateProfilePath))
                {
                    _profileService.CopyProfileContents(_config.TemplateProfilePath, profilePath, excludeAutoLogin: true);
                }

                // Generate AutoLogin config if character is selected
                if (dataCenterId > 0 && worldId > 0)
                {
                    _profileService.SaveAutoLoginConfig(profilePath, dataCenterId, worldId, Profile.CharacterSlot);
                }

                MessageBox.Show($"Created Dalamud profile folder:\n{profilePath}",
                    "Profile Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create profile folder:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        else
        {
            Profile.RoamingPath = string.IsNullOrWhiteSpace(txtRoamingPath.Text)
                ? null
                : txtRoamingPath.Text.Trim();

            // Update AutoLogin config if roaming path exists and character is selected
            if (!string.IsNullOrEmpty(Profile.RoamingPath) &&
                Directory.Exists(Profile.RoamingPath) &&
                dataCenterId > 0 && worldId > 0)
            {
                try
                {
                    _profileService.SaveAutoLoginConfig(Profile.RoamingPath, dataCenterId, worldId, Profile.CharacterSlot);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to update AutoLogin config:\n{ex.Message}",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }

    private void chkUseOtp_CheckedChanged(object? sender, EventArgs e)
    {
        UpdateOtpFieldsEnabled();
    }

    private void chkAutoCreateProfile_CheckedChanged(object? sender, EventArgs e)
    {
        UpdateAutoCreateEnabled();
    }

    private void UpdateOtpFieldsEnabled()
    {
        txtOtpProvider.Enabled = chkUseOtp.Checked;
        lblOtpProvider.Enabled = chkUseOtp.Checked;
    }

    private void UpdateAutoCreateEnabled()
    {
        var autoCreate = chkAutoCreateProfile.Checked && _isNewProfile;
        txtRoamingPath.Enabled = !autoCreate;
        btnBrowseRoaming.Enabled = !autoCreate;

        if (autoCreate && !string.IsNullOrEmpty(txtAccountName.Text))
        {
            txtRoamingPath.Text = Path.Combine(_config.ProfilesBasePath, txtAccountName.Text.Trim().ToLower());
        }
    }

    private void txtAccountName_TextChanged(object? sender, EventArgs e)
    {
        if (chkAutoCreateProfile.Checked && _isNewProfile)
        {
            txtRoamingPath.Text = Path.Combine(_config.ProfilesBasePath,
                txtAccountName.Text.Trim().ToLower());
        }
    }

    private void btnBrowseRoaming_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select Roaming Path folder"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtRoamingPath.Text = dialog.SelectedPath;
        }
    }
}

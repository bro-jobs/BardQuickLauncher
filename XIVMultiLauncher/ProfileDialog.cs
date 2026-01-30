using XIVMultiLauncher.Models;

namespace XIVMultiLauncher;

public partial class ProfileDialog : Form
{
    public Profile? Profile { get; private set; }

    public ProfileDialog(Profile? existingProfile = null)
    {
        InitializeComponent();

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
            this.Text = "Edit Profile";
        }
        else
        {
            this.Text = "Add Profile";
        }

        UpdateOtpFieldsEnabled();
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

        Profile ??= new Profile();
        Profile.AccountName = txtAccountName.Text.Trim().ToLower();
        Profile.DisplayName = string.IsNullOrWhiteSpace(txtDisplayName.Text)
            ? Profile.AccountName
            : txtDisplayName.Text.Trim();
        Profile.IsMainAccount = chkMainAccount.Checked;
        Profile.IsSteam = chkSteam.Checked;
        Profile.UseOtp = chkUseOtp.Checked;
        Profile.RoamingPath = string.IsNullOrWhiteSpace(txtRoamingPath.Text)
            ? null
            : txtRoamingPath.Text.Trim();
        Profile.OtpProvider = string.IsNullOrWhiteSpace(txtOtpProvider.Text)
            ? null
            : txtOtpProvider.Text.Trim();

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

    private void UpdateOtpFieldsEnabled()
    {
        txtOtpProvider.Enabled = chkUseOtp.Checked;
        lblOtpProvider.Enabled = chkUseOtp.Checked;
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

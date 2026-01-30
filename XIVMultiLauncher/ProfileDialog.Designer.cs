namespace XIVMultiLauncher;

partial class ProfileDialog
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        // Form settings
        this.Text = "Profile";
        this.Size = new Size(480, 520);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;

        // Account Name
        lblAccountName = new Label();
        lblAccountName.Text = "Account Name:";
        lblAccountName.Location = new Point(12, 18);
        lblAccountName.AutoSize = true;

        txtAccountName = new TextBox();
        txtAccountName.Location = new Point(130, 15);
        txtAccountName.Size = new Size(200, 23);
        txtAccountName.TextChanged += txtAccountName_TextChanged;

        lblAccountHint = new Label();
        lblAccountHint.Text = "(Your Square Enix username, lowercase)";
        lblAccountHint.Location = new Point(130, 40);
        lblAccountHint.AutoSize = true;
        lblAccountHint.ForeColor = Color.Gray;
        lblAccountHint.Font = new Font(this.Font.FontFamily, 8);

        // Display Name
        lblDisplayName = new Label();
        lblDisplayName.Text = "Display Name:";
        lblDisplayName.Location = new Point(12, 65);
        lblDisplayName.AutoSize = true;

        txtDisplayName = new TextBox();
        txtDisplayName.Location = new Point(130, 62);
        txtDisplayName.Size = new Size(200, 23);

        // Checkboxes row 1
        chkMainAccount = new CheckBox();
        chkMainAccount.Text = "Main Account";
        chkMainAccount.Location = new Point(130, 95);
        chkMainAccount.AutoSize = true;

        chkSteam = new CheckBox();
        chkSteam.Text = "Steam";
        chkSteam.Location = new Point(260, 95);
        chkSteam.AutoSize = true;

        chkUseOtp = new CheckBox();
        chkUseOtp.Text = "Use OTP";
        chkUseOtp.Location = new Point(350, 95);
        chkUseOtp.AutoSize = true;
        chkUseOtp.CheckedChanged += chkUseOtp_CheckedChanged;

        // OTP Provider
        lblOtpProvider = new Label();
        lblOtpProvider.Text = "OTP Provider:";
        lblOtpProvider.Location = new Point(12, 125);
        lblOtpProvider.AutoSize = true;

        txtOtpProvider = new TextBox();
        txtOtpProvider.Location = new Point(130, 122);
        txtOtpProvider.Size = new Size(320, 23);
        txtOtpProvider.Enabled = false;
        txtOtpProvider.PlaceholderText = "e.g., 1password:ItemName";

        // Character Selection Group
        grpCharacter = new GroupBox();
        grpCharacter.Text = "Character Auto-Login";
        grpCharacter.Location = new Point(12, 155);
        grpCharacter.Size = new Size(445, 110);

        lblDataCenter = new Label();
        lblDataCenter.Text = "Data Center:";
        lblDataCenter.Location = new Point(10, 25);
        lblDataCenter.AutoSize = true;

        cmbDataCenter = new ComboBox();
        cmbDataCenter.Location = new Point(100, 22);
        cmbDataCenter.Size = new Size(180, 23);
        cmbDataCenter.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDataCenter.SelectedIndexChanged += cmbDataCenter_SelectedIndexChanged;

        lblWorld = new Label();
        lblWorld.Text = "World:";
        lblWorld.Location = new Point(290, 25);
        lblWorld.AutoSize = true;

        cmbWorld = new ComboBox();
        cmbWorld.Location = new Point(340, 22);
        cmbWorld.Size = new Size(95, 23);
        cmbWorld.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbWorld.Enabled = false;

        lblCharacterSlot = new Label();
        lblCharacterSlot.Text = "Character Slot:";
        lblCharacterSlot.Location = new Point(10, 55);
        lblCharacterSlot.AutoSize = true;

        cmbCharacterSlot = new ComboBox();
        cmbCharacterSlot.Location = new Point(100, 52);
        cmbCharacterSlot.Size = new Size(100, 23);
        cmbCharacterSlot.DropDownStyle = ComboBoxStyle.DropDownList;

        lblCharacterHint = new Label();
        lblCharacterHint.Text = "Configure DataCenter/World/Slot to auto-login to a specific character";
        lblCharacterHint.Location = new Point(10, 85);
        lblCharacterHint.AutoSize = true;
        lblCharacterHint.ForeColor = Color.Gray;
        lblCharacterHint.Font = new Font(this.Font.FontFamily, 8);

        grpCharacter.Controls.Add(lblDataCenter);
        grpCharacter.Controls.Add(cmbDataCenter);
        grpCharacter.Controls.Add(lblWorld);
        grpCharacter.Controls.Add(cmbWorld);
        grpCharacter.Controls.Add(lblCharacterSlot);
        grpCharacter.Controls.Add(cmbCharacterSlot);
        grpCharacter.Controls.Add(lblCharacterHint);

        // Profile Path Group
        grpProfile = new GroupBox();
        grpProfile.Text = "Dalamud Profile";
        grpProfile.Location = new Point(12, 275);
        grpProfile.Size = new Size(445, 115);

        chkAutoCreateProfile = new CheckBox();
        chkAutoCreateProfile.Text = "Auto-create profile folder for this account";
        chkAutoCreateProfile.Location = new Point(10, 22);
        chkAutoCreateProfile.AutoSize = true;
        chkAutoCreateProfile.Checked = true;
        chkAutoCreateProfile.CheckedChanged += chkAutoCreateProfile_CheckedChanged;

        lblRoamingPath = new Label();
        lblRoamingPath.Text = "Roaming Path:";
        lblRoamingPath.Location = new Point(10, 52);
        lblRoamingPath.AutoSize = true;

        txtRoamingPath = new TextBox();
        txtRoamingPath.Location = new Point(100, 49);
        txtRoamingPath.Size = new Size(250, 23);

        btnBrowseRoaming = new Button();
        btnBrowseRoaming.Text = "Browse...";
        btnBrowseRoaming.Location = new Point(356, 48);
        btnBrowseRoaming.Size = new Size(75, 25);
        btnBrowseRoaming.Click += btnBrowseRoaming_Click;

        lblProfileHint = new Label();
        lblProfileHint.Text = "Each account needs a separate folder for its own plugin configs";
        lblProfileHint.Location = new Point(10, 80);
        lblProfileHint.AutoSize = true;
        lblProfileHint.ForeColor = Color.Gray;
        lblProfileHint.Font = new Font(this.Font.FontFamily, 8);

        lblProfileHint2 = new Label();
        lblProfileHint2.Text = "Template folder (in Settings) will be copied to new profiles";
        lblProfileHint2.Location = new Point(10, 93);
        lblProfileHint2.AutoSize = true;
        lblProfileHint2.ForeColor = Color.Gray;
        lblProfileHint2.Font = new Font(this.Font.FontFamily, 8);

        grpProfile.Controls.Add(chkAutoCreateProfile);
        grpProfile.Controls.Add(lblRoamingPath);
        grpProfile.Controls.Add(txtRoamingPath);
        grpProfile.Controls.Add(btnBrowseRoaming);
        grpProfile.Controls.Add(lblProfileHint);
        grpProfile.Controls.Add(lblProfileHint2);

        // Buttons
        btnOK = new Button();
        btnOK.Text = "OK";
        btnOK.Location = new Point(290, 445);
        btnOK.Size = new Size(75, 28);
        btnOK.Click += btnOK_Click;

        btnCancel = new Button();
        btnCancel.Text = "Cancel";
        btnCancel.Location = new Point(375, 445);
        btnCancel.Size = new Size(75, 28);
        btnCancel.Click += btnCancel_Click;

        // Add controls to form
        this.Controls.Add(lblAccountName);
        this.Controls.Add(txtAccountName);
        this.Controls.Add(lblAccountHint);
        this.Controls.Add(lblDisplayName);
        this.Controls.Add(txtDisplayName);
        this.Controls.Add(chkMainAccount);
        this.Controls.Add(chkSteam);
        this.Controls.Add(chkUseOtp);
        this.Controls.Add(lblOtpProvider);
        this.Controls.Add(txtOtpProvider);
        this.Controls.Add(grpCharacter);
        this.Controls.Add(grpProfile);
        this.Controls.Add(btnOK);
        this.Controls.Add(btnCancel);

        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }

    #endregion

    private Label lblAccountName;
    private TextBox txtAccountName;
    private Label lblAccountHint;
    private Label lblDisplayName;
    private TextBox txtDisplayName;
    private CheckBox chkMainAccount;
    private CheckBox chkSteam;
    private CheckBox chkUseOtp;
    private Label lblOtpProvider;
    private TextBox txtOtpProvider;

    private GroupBox grpCharacter;
    private Label lblDataCenter;
    private ComboBox cmbDataCenter;
    private Label lblWorld;
    private ComboBox cmbWorld;
    private Label lblCharacterSlot;
    private ComboBox cmbCharacterSlot;
    private Label lblCharacterHint;

    private GroupBox grpProfile;
    private CheckBox chkAutoCreateProfile;
    private Label lblRoamingPath;
    private TextBox txtRoamingPath;
    private Button btnBrowseRoaming;
    private Label lblProfileHint;
    private Label lblProfileHint2;

    private Button btnOK;
    private Button btnCancel;
}

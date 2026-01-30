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
        this.Size = new Size(420, 340);
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
        txtAccountName.Location = new Point(120, 15);
        txtAccountName.Size = new Size(270, 23);

        lblAccountHint = new Label();
        lblAccountHint.Text = "(Your Square Enix username, lowercase)";
        lblAccountHint.Location = new Point(120, 40);
        lblAccountHint.AutoSize = true;
        lblAccountHint.ForeColor = Color.Gray;
        lblAccountHint.Font = new Font(this.Font.FontFamily, 8);

        // Display Name
        lblDisplayName = new Label();
        lblDisplayName.Text = "Display Name:";
        lblDisplayName.Location = new Point(12, 68);
        lblDisplayName.AutoSize = true;

        txtDisplayName = new TextBox();
        txtDisplayName.Location = new Point(120, 65);
        txtDisplayName.Size = new Size(270, 23);

        // Checkboxes
        chkMainAccount = new CheckBox();
        chkMainAccount.Text = "Main Account (pause after launch for others)";
        chkMainAccount.Location = new Point(120, 100);
        chkMainAccount.AutoSize = true;

        chkSteam = new CheckBox();
        chkSteam.Text = "Steam Account";
        chkSteam.Location = new Point(120, 125);
        chkSteam.AutoSize = true;

        chkUseOtp = new CheckBox();
        chkUseOtp.Text = "Use OTP (2FA)";
        chkUseOtp.Location = new Point(120, 150);
        chkUseOtp.AutoSize = true;
        chkUseOtp.CheckedChanged += chkUseOtp_CheckedChanged;

        // OTP Provider
        lblOtpProvider = new Label();
        lblOtpProvider.Text = "OTP Provider:";
        lblOtpProvider.Location = new Point(12, 180);
        lblOtpProvider.AutoSize = true;

        txtOtpProvider = new TextBox();
        txtOtpProvider.Location = new Point(120, 177);
        txtOtpProvider.Size = new Size(270, 23);
        txtOtpProvider.Enabled = false;

        lblOtpHint = new Label();
        lblOtpHint.Text = "(e.g., 1password:ItemName)";
        lblOtpHint.Location = new Point(120, 202);
        lblOtpHint.AutoSize = true;
        lblOtpHint.ForeColor = Color.Gray;
        lblOtpHint.Font = new Font(this.Font.FontFamily, 8);

        // Roaming Path
        lblRoamingPath = new Label();
        lblRoamingPath.Text = "Roaming Path:";
        lblRoamingPath.Location = new Point(12, 230);
        lblRoamingPath.AutoSize = true;

        txtRoamingPath = new TextBox();
        txtRoamingPath.Location = new Point(120, 227);
        txtRoamingPath.Size = new Size(190, 23);

        btnBrowseRoaming = new Button();
        btnBrowseRoaming.Text = "Browse...";
        btnBrowseRoaming.Location = new Point(316, 226);
        btnBrowseRoaming.Size = new Size(75, 25);
        btnBrowseRoaming.Click += btnBrowseRoaming_Click;

        lblRoamingHint = new Label();
        lblRoamingHint.Text = "(Optional: separate config folder for this account)";
        lblRoamingHint.Location = new Point(120, 252);
        lblRoamingHint.AutoSize = true;
        lblRoamingHint.ForeColor = Color.Gray;
        lblRoamingHint.Font = new Font(this.Font.FontFamily, 8);

        // Buttons
        btnOK = new Button();
        btnOK.Text = "OK";
        btnOK.Location = new Point(230, 275);
        btnOK.Size = new Size(75, 25);
        btnOK.Click += btnOK_Click;

        btnCancel = new Button();
        btnCancel.Text = "Cancel";
        btnCancel.Location = new Point(315, 275);
        btnCancel.Size = new Size(75, 25);
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
        this.Controls.Add(lblOtpHint);
        this.Controls.Add(lblRoamingPath);
        this.Controls.Add(txtRoamingPath);
        this.Controls.Add(btnBrowseRoaming);
        this.Controls.Add(lblRoamingHint);
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
    private Label lblOtpHint;
    private Label lblRoamingPath;
    private TextBox txtRoamingPath;
    private Button btnBrowseRoaming;
    private Label lblRoamingHint;
    private Button btnOK;
    private Button btnCancel;
}

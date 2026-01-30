namespace XIVMultiLauncher;

partial class MainForm
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
        this.Text = "XIV Multi Launcher";
        this.Size = new System.Drawing.Size(600, 540);
        this.MinimumSize = new System.Drawing.Size(550, 480);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Settings group
        grpSettings = new GroupBox();
        grpSettings.Text = "Settings";
        grpSettings.Location = new Point(12, 12);
        grpSettings.Size = new Size(560, 130);
        grpSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        lblPath = new Label();
        lblPath.Text = "XIVLauncher Path:";
        lblPath.Location = new Point(10, 25);
        lblPath.AutoSize = true;

        txtXIVLauncherPath = new TextBox();
        txtXIVLauncherPath.Location = new Point(120, 22);
        txtXIVLauncherPath.Size = new Size(350, 23);
        txtXIVLauncherPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtXIVLauncherPath.ReadOnly = true;

        btnBrowse = new Button();
        btnBrowse.Text = "Browse...";
        btnBrowse.Location = new Point(476, 21);
        btnBrowse.Size = new Size(75, 25);
        btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBrowse.Click += btnBrowse_Click;

        lblTemplate = new Label();
        lblTemplate.Text = "Template Folder:";
        lblTemplate.Location = new Point(10, 52);
        lblTemplate.AutoSize = true;

        txtTemplatePath = new TextBox();
        txtTemplatePath.Location = new Point(120, 49);
        txtTemplatePath.Size = new Size(350, 23);
        txtTemplatePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtTemplatePath.ReadOnly = true;
        txtTemplatePath.PlaceholderText = "(Your main account's XIVLauncher folder)";

        btnBrowseTemplate = new Button();
        btnBrowseTemplate.Text = "Browse...";
        btnBrowseTemplate.Location = new Point(476, 48);
        btnBrowseTemplate.Size = new Size(75, 25);
        btnBrowseTemplate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBrowseTemplate.Click += btnBrowseTemplate_Click;

        lblSleep = new Label();
        lblSleep.Text = "Delay between launches:";
        lblSleep.Location = new Point(10, 79);
        lblSleep.AutoSize = true;

        numSleepTime = new NumericUpDown();
        numSleepTime.Location = new Point(160, 77);
        numSleepTime.Size = new Size(60, 23);
        numSleepTime.Minimum = 1;
        numSleepTime.Maximum = 60;
        numSleepTime.Value = 10;
        numSleepTime.ValueChanged += numSleepTime_ValueChanged;

        lblSeconds = new Label();
        lblSeconds.Text = "seconds";
        lblSeconds.Location = new Point(226, 79);
        lblSeconds.AutoSize = true;

        chkKillMutants = new CheckBox();
        chkKillMutants.Text = "Auto-kill mutants (allows >2 instances)";
        chkKillMutants.Location = new Point(10, 103);
        chkKillMutants.AutoSize = true;
        chkKillMutants.Checked = true;
        chkKillMutants.CheckedChanged += chkKillMutants_CheckedChanged;

        btnKillMutants = new Button();
        btnKillMutants.Text = "Kill Mutants Now";
        btnKillMutants.Location = new Point(280, 99);
        btnKillMutants.Size = new Size(120, 25);
        btnKillMutants.Click += btnKillMutants_Click;

        grpSettings.Controls.Add(lblPath);
        grpSettings.Controls.Add(txtXIVLauncherPath);
        grpSettings.Controls.Add(btnBrowse);
        grpSettings.Controls.Add(lblTemplate);
        grpSettings.Controls.Add(txtTemplatePath);
        grpSettings.Controls.Add(btnBrowseTemplate);
        grpSettings.Controls.Add(lblSleep);
        grpSettings.Controls.Add(numSleepTime);
        grpSettings.Controls.Add(lblSeconds);
        grpSettings.Controls.Add(chkKillMutants);
        grpSettings.Controls.Add(btnKillMutants);

        // Profiles group
        grpProfiles = new GroupBox();
        grpProfiles.Text = "Profiles";
        grpProfiles.Location = new Point(12, 148);
        grpProfiles.Size = new Size(560, 260);
        grpProfiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        lstProfiles = new ListView();
        lstProfiles.Location = new Point(10, 22);
        lstProfiles.Size = new Size(450, 190);
        lstProfiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lstProfiles.View = View.Details;
        lstProfiles.FullRowSelect = true;
        lstProfiles.GridLines = true;
        lstProfiles.Columns.Add("Display Name", 120);
        lstProfiles.Columns.Add("Account", 100);
        lstProfiles.Columns.Add("Main", 40);
        lstProfiles.Columns.Add("Steam", 45);
        lstProfiles.Columns.Add("OTP", 40);
        lstProfiles.Columns.Add("Status", 80);
        lstProfiles.DoubleClick += lstProfiles_DoubleClick;

        btnAddProfile = new Button();
        btnAddProfile.Text = "Add";
        btnAddProfile.Location = new Point(466, 22);
        btnAddProfile.Size = new Size(85, 28);
        btnAddProfile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAddProfile.Click += btnAddProfile_Click;

        btnEditProfile = new Button();
        btnEditProfile.Text = "Edit";
        btnEditProfile.Location = new Point(466, 56);
        btnEditProfile.Size = new Size(85, 28);
        btnEditProfile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnEditProfile.Click += btnEditProfile_Click;

        btnRemoveProfile = new Button();
        btnRemoveProfile.Text = "Remove";
        btnRemoveProfile.Location = new Point(466, 90);
        btnRemoveProfile.Size = new Size(85, 28);
        btnRemoveProfile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRemoveProfile.Click += btnRemoveProfile_Click;

        btnMoveUp = new Button();
        btnMoveUp.Text = "Move Up";
        btnMoveUp.Location = new Point(466, 134);
        btnMoveUp.Size = new Size(85, 28);
        btnMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnMoveUp.Click += btnMoveUp_Click;

        btnMoveDown = new Button();
        btnMoveDown.Text = "Move Down";
        btnMoveDown.Location = new Point(466, 168);
        btnMoveDown.Size = new Size(85, 28);
        btnMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnMoveDown.Click += btnMoveDown_Click;

        btnSyncProfile = new Button();
        btnSyncProfile.Text = "Sync";
        btnSyncProfile.Location = new Point(120, 220);
        btnSyncProfile.Size = new Size(100, 30);
        btnSyncProfile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnSyncProfile.Click += btnSyncProfile_Click;

        btnSyncAll = new Button();
        btnSyncAll.Text = "Sync All";
        btnSyncAll.Location = new Point(230, 220);
        btnSyncAll.Size = new Size(100, 30);
        btnSyncAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnSyncAll.Click += btnSyncAll_Click;

        btnLaunchSelected = new Button();
        btnLaunchSelected.Text = "Launch";
        btnLaunchSelected.Location = new Point(10, 220);
        btnLaunchSelected.Size = new Size(100, 30);
        btnLaunchSelected.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnLaunchSelected.Click += btnLaunchSelected_Click;

        btnLaunchAll = new Button();
        btnLaunchAll.Text = "Launch All";
        btnLaunchAll.Location = new Point(360, 220);
        btnLaunchAll.Size = new Size(100, 30);
        btnLaunchAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnLaunchAll.Click += btnLaunchAll_Click;

        grpProfiles.Controls.Add(lstProfiles);
        grpProfiles.Controls.Add(btnAddProfile);
        grpProfiles.Controls.Add(btnEditProfile);
        grpProfiles.Controls.Add(btnRemoveProfile);
        grpProfiles.Controls.Add(btnMoveUp);
        grpProfiles.Controls.Add(btnMoveDown);
        grpProfiles.Controls.Add(btnLaunchSelected);
        grpProfiles.Controls.Add(btnSyncProfile);
        grpProfiles.Controls.Add(btnSyncAll);
        grpProfiles.Controls.Add(btnLaunchAll);

        // Status bar
        progressBar = new ProgressBar();
        progressBar.Location = new Point(12, 420);
        progressBar.Size = new Size(560, 23);
        progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        lblStatus = new Label();
        lblStatus.Text = "Ready";
        lblStatus.Location = new Point(12, 450);
        lblStatus.Size = new Size(560, 20);
        lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        // Add controls to form
        this.Controls.Add(grpSettings);
        this.Controls.Add(grpProfiles);
        this.Controls.Add(progressBar);
        this.Controls.Add(lblStatus);
    }

    #endregion

    private GroupBox grpSettings;
    private Label lblPath;
    private TextBox txtXIVLauncherPath;
    private Button btnBrowse;
    private Label lblTemplate;
    private TextBox txtTemplatePath;
    private Button btnBrowseTemplate;
    private Label lblSleep;
    private NumericUpDown numSleepTime;
    private Label lblSeconds;
    private CheckBox chkKillMutants;
    private Button btnKillMutants;

    private GroupBox grpProfiles;
    private ListView lstProfiles;
    private Button btnAddProfile;
    private Button btnEditProfile;
    private Button btnRemoveProfile;
    private Button btnMoveUp;
    private Button btnMoveDown;
    private Button btnLaunchSelected;
    private Button btnSyncProfile;
    private Button btnSyncAll;
    private Button btnLaunchAll;

    private ProgressBar progressBar;
    private Label lblStatus;
}

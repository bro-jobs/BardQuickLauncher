namespace XIVMultiLauncher.Models;

public class Profile
{
    public string AccountName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsMainAccount { get; set; }
    public bool IsSteam { get; set; }
    public bool UseOtp { get; set; }
    public string? RoamingPath { get; set; }
    public string? OtpProvider { get; set; } // e.g., "1password:ProfileName"

    // AutoLogin character selection
    public uint DataCenterId { get; set; }
    public uint WorldId { get; set; }
    public uint CharacterSlot { get; set; } // 0-7

    /// <summary>
    /// Gets the account ID in XIVLauncher format: {username}-{useOtp}-{isSteam}
    /// </summary>
    public string AccountId => $"{AccountName}-{UseOtp}-{IsSteam}";

    public override string ToString() => string.IsNullOrEmpty(DisplayName) ? AccountName : DisplayName;
}

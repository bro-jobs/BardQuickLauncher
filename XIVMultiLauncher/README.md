# XIV Multi Launcher

A Windows GUI application for launching multiple XIVLauncher accounts sequentially.

## Features

- Configure multiple FFXIV account profiles
- Launch all accounts with a single click
- Configurable delay between launches
- Support for Steam accounts
- OTP (2FA) support via 1Password CLI
- Optional roaming path per profile (for separate plugin configurations)
- Main account detection (pauses to prevent >2 instance issues)

## Requirements

- Windows 10/11 (x64)
- XIVLauncher installed with accounts configured and "Auto Login" enabled for each account

## Installation

1. Download `XIVMultiLauncher.exe` from the releases
2. Place it anywhere on your system
3. Run it - no installation required

## Setup

### Initial XIVLauncher Setup

Before using this tool, you must set up each account in XIVLauncher:

1. Open XIVLauncher
2. Log in with your first account
3. Enable "Auto Login" in settings
4. If using separate configs per account, use `--roamingPath` on first launch to create separate config folders

### Adding Profiles

1. Click "Add" to add a new profile
2. Fill in:
   - **Account Name**: Your Square Enix username (lowercase)
   - **Display Name**: Friendly name shown in the list
   - **Main Account**: Check if this is your main account (will pause before launching others)
   - **Steam Account**: Check if this account uses Steam
   - **Use OTP**: Check if this account has 2FA enabled
   - **OTP Provider**: Format: `1password:ItemName` (requires 1Password CLI)
   - **Roaming Path**: Optional custom config folder path

### Account ID Format

XIVLauncher identifies accounts using the format: `{username}-{useOtp}-{isSteam}`

For example:
- `myaccount-False-False` - Regular account, no OTP
- `myaccount-True-False` - Regular account with OTP
- `myaccount-False-True` - Steam account, no OTP

## Building from Source

```bash
# Build
dotnet build

# Publish single-file exe
dotnet publish -c Release

# Output: bin/Release/net8.0-windows/win-x64/publish/XIVMultiLauncher.exe
```

## Configuration

Settings are stored in `config.json` next to the executable.

## Credits

- Inspired by [BardQuickLauncher](https://github.com/AlexFlipnote/BardQuickLauncher)
- Uses XIVLauncher's `--account` and `--roamingPath` command-line arguments

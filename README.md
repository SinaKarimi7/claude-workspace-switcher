# Claude Workspace Switcher

**Archived.** For multiple Claude Desktop accounts on Windows, we recommend [Piwqust’s Claude Profile Switcher](https://github.com/Piwqust/claude-switcher). The documentation below describes the archived launcher.

![Two overlapping workspace windows with the Claude starburst](assets/workspaces.webp)

A small Windows launcher for two Claude Desktop profiles. Work opens the default profile. Original launches Claude with a separate user-data folder under LocalAppData named Claude-Work. Each button starts the installed Claude Desktop executable. Browser sign-in callbacks follow Windows app registration.

## Build and run

Download `Claude Workspace Switcher.exe` from the [latest GitHub release](https://github.com/SinaKarimi7/claude-workspace-switcher/releases/latest) and open it on Windows with Claude Desktop installed.

On 64-bit Windows with .NET Framework 4.x and the Claude Desktop MSIX package installed, run ` .\build.ps1 ` in PowerShell. Open `bin\Claude Workspace Switcher.exe` and pin it to the taskbar if desired.

## Signing releases

1. In Azure Artifact Signing, complete identity validation and create a **Public Trust** certificate profile. Grant the workflow's Azure application the **Artifact Signing Certificate Profile Signer** role on that profile.
2. Add a federated credential to the Azure application for GitHub Actions. Use issuer `https://token.actions.githubusercontent.com`, audience `api://AzureADTokenExchange`, and subject `repo:SinaKarimi7/claude-workspace-switcher:environment:release`.
3. In the GitHub repository's `release` environment, restrict deployments to `v*` tags and set the variables `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, `SIGNING_ENDPOINT` (the regional endpoint URL), `SIGNING_ACCOUNT`, and `SIGNING_PROFILE`.
4. Push a new version tag. The release job signs the executable, verifies its signature and timestamp, then uploads it to GitHub Releases.

## Manage profiles

Choose **Add profile** to create a named launcher with a separate local Claude data folder. Choose **Edit** to rename a profile, designate it as main, or remove its launcher entry. The main profile appears first and is the default button when the window opens. Removing the main profile promotes the first remaining entry. The last profile cannot be removed. Removing a launcher entry keeps the local Claude files. Profile settings are saved for the current Windows user.

# Claude Workspace Switcher

A small Windows launcher for two Claude Desktop profiles. Work opens the default profile. Original launches Claude with a separate user-data folder under LocalAppData named Claude-Work. Each button starts the installed Claude Desktop executable. Browser sign-in callbacks follow Windows app registration.

## Build and run

On 64-bit Windows with .NET Framework 4.x and the Claude Desktop MSIX package installed, run ` .\build.ps1 ` in PowerShell. Open `bin\Claude Workspace Switcher.exe` and pin it to the taskbar if desired.

## Manage profiles

Choose **Add profile** to create a named launcher with a separate local Claude data folder. Choose **Edit** to rename a profile, designate it as main, or remove its launcher entry. The main profile appears first and is the default button when the window opens. Removing the main profile promotes the first remaining entry. The last profile cannot be removed. Removing a launcher entry keeps the local Claude files. Profile settings are saved for the current Windows user.

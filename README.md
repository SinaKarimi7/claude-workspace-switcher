# Claude Workspace Switcher

A small Windows launcher for two Claude Desktop profiles. Work opens the default profile. Original launches Claude with a separate user-data folder under LocalAppData named Claude-Work. Each button starts the installed Claude Desktop executable. Browser sign-in callbacks follow Windows app registration.

## Build and run

On 64-bit Windows with .NET Framework 4.x and the Claude Desktop MSIX package installed, run ` .\build.ps1 ` in PowerShell. Open `bin\Claude Workspace Switcher.exe` and pin it to the taskbar if desired.

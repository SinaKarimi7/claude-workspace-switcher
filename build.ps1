$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
    New-Item -ItemType Directory -Force bin | Out-Null
    & $compiler /nologo /target:winexe '/out:bin\Claude Workspace Switcher.exe' /win32manifest:Switcher.manifest /win32icon:ClaudeSwitcher.ico Switcher.cs
    if ($LASTEXITCODE) { throw "Build failed ($LASTEXITCODE)" }
} finally {
    Pop-Location
}

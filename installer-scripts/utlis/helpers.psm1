# ========================
# Pwamp Installer Helpers
# ========================

function Ensure-Directory {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Path
    )
    if (-not (Test-Path -Path $Path)) {
        New-Item -ItemType Directory -Path $Path | Out-Null
    }
}

function Get-InstallerConfig {
    $configPath = Join-Path $PSScriptRoot "..\installer-config.json"
    if (-not (Test-Path $configPath)) {
        throw "Config file not found: $configPath"
    }

    $json = Get-Content $configPath -Raw | ConvertFrom-Json
    return $json
}

function Assert-FileHash {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter(Mandatory = $true)]
        [string]$ExpectedHash
    )

    if (-not (Test-Path $Path)) {
        throw "File not found: $Path"
    }

    $actualHash = (Get-FileHash -Path $Path -Algorithm SHA256).Hash.ToLower()
    $expected = $ExpectedHash.ToLower()

    if ($actualHash -ne $expected) {
        throw "Hash mismatch for $Path. Expected $expected but got $actualHash"
    }
}

function Extract-Zip {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Source,

        [Parameter(Mandatory = $true)]
        [string]$Destination
    )

    if (-not (Test-Path $Source)) {
        throw "ZIP file not found: $Source"
    }

    # If nested folder exists inside ZIP, this extracts contents cleanly
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($Source, $Destination)
}

function Apply-Template {
    param (
        [Parameter(Mandatory = $true)]
        [string]$TemplatePath,

        [Parameter(Mandatory = $true)]
        [string]$TargetPath,

        [Parameter(Mandatory = $true)]
        [hashtable]$Replacements
    )

    if (-not (Test-Path $TemplatePath)) {
        throw "Template file not found: $TemplatePath"
    }

    $content = Get-Content $TemplatePath -Raw

    foreach ($key in $Replacements.Keys) {
        $placeholder = "{{${key}}}"
        $content = $content -replace [regex]::Escape($placeholder), [regex]::Escape($Replacements[$key])
    }

    $targetDir = Split-Path $TargetPath -Parent
    Ensure-Directory $targetDir
    Set-Content -Path $TargetPath -Value $content -Encoding UTF8
}

function Test-Cancel {
    if ($env:PWAMP_CANCEL_FILE -and (Test-Path $env:PWAMP_CANCEL_FILE)) {
        throw "⚠️ Installation cancelled by user."
    }
}

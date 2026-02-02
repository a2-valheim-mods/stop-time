# Get the folder where the script is located
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Folder containing the .md files
$packageDir = Join-Path $scriptDir "..\Package" | Resolve-Path

# Files to fix
$files = @("CHANGELOG.md", "README.md", "manifest.json") | ForEach-Object { Join-Path $packageDir $_ }

foreach ($file in $files) {
    if (Test-Path $file) {
        $bytes = [System.IO.File]::ReadAllBytes($file)
        # Check for UTF-8 BOM
        if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
            Write-Host "Removing BOM from $file"
            # Write back without BOM
            [System.IO.File]::WriteAllBytes($file, $bytes[3..($bytes.Length-1)])
        }
    }
}

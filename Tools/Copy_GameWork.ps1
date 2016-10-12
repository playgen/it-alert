# Must be run from ./Tools directory

# Copy Gamework.Core Unity GameWork.unity
Push-Location "../GameWork.Unity/Tools"
& "./Copy_GameWorkCore.ps1"
Pop-Location

# Copy GameWork.Unity

function CopyGameworkComponent([string] $sourcePath, [string] $destPath)
{
    If(Test-Path $destPath)
    {
        Write-Output "Removing: $destPath"
        Remove-Item $destPath -Recurse
    }

    New-Item -ItemType directory $destPath | Out-Null

    Get-ChildItem -Path $sourcePath | Copy-Item -Destination $destPath -Recurse -Container
}

CopyGameworkComponent "..\GameWork.Unity\UnityProject\Assets\GameWork\Core" "..\Client\Assets\GameWork\Core"
CopyGameworkComponent "..\GameWork.Unity\UnityProject\Assets\GameWork\Unity" "..\Client\Assets\GameWork\Unity"
$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"

$root = "it-alert"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$splitIndex = $scriptDir.IndexOf($root)
$parentDir = $scriptDir.Substring(0, $splitIndex + $root.Length)

Write-Output $parentDir

$nuget = "$parentDir\Tools\nuget.exe"

function Build
{
    param( [string]$solution, [string]$configuration )

    & $nuget restore $solution
    & $msbuild "$solution" "/p:Configuration=$configuration"
}


Build "$parentDir\SUGAR\PlayGen.SUGAR.sln" "Debug"

Build "$parentDir\Simulation\PlayGen.ITAlert.sln" "UnityDebug"

Build "$parentDir\GameWork.Unity\GameWork.Unity.sln" "Unity Client Debug"

Build "$parentDir\Server\Photon.Loadbalancing.sln" "Unity Debug"
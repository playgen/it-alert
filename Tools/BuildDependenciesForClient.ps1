$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
$nuget = ".\nuget.exe"

function Build
{
    param( [string]$solution, [string]$configuration )

    & $nuget restore ..\$solution
    & $msbuild "..\$solution" "/p:Configuration=$configuration"
}


Build "SUGAR\PlayGen.SUGAR.sln" "Debug"

Build "Simulation\PlayGen.ITAlert.sln" "UnityDebug"

Build "GameWork\GameWork.sln" "Unity Client Debug"

Build "Server\Photon.Loadbalancing.sln" "Unity Debug"
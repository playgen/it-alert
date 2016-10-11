#must be executed from the Tools folder

$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

$nuget = ".\nuget.exe"

function Build
{
    param( [string]$relativeDir, [string]$solution, [string]$configuration )

    dotnet restore "..\$relativeDir"
    & $nuget restore "..\$relativeDir\$solution"
    & $msbuild "..\$relativeDir\$solution" "/p:Configuration=$configuration"
}


Build "SUGAR" "PlayGen.SUGAR.sln" "UnityDebug"

Build "Simulation" "PlayGen.ITAlert.sln" "UnityDebug"

Build "GameWork.Unity" "GameWork.Unity.sln" "Unity Client Debug"

Build "Server" "Photon.Loadbalancing.sln" "Unity Debug"
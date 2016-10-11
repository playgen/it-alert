#must be executed from the Tools folder

$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

$nuget = ".\Tools\nuget.exe"

function Build
{
    param( [string]$solution, [string]$configuration )

    & $nuget restore ".\$solution"
    & $msbuild ".\$solution" "/p:Configuration=$configuration"
}


Build "SUGAR\PlayGen.SUGAR.sln" "Debug"

Build "Simulation\PlayGen.ITAlert.sln" "UnityDebug"

Build "GameWork.Unity\GameWork.Unity.sln" "Unity Client Debug"

Build "Server\Photon.Loadbalancing.sln" "Unity Debug"
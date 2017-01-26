# Must be executed from the Tools folder

$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

$nuget = ".\nuget.exe"

function Restore
{
	param( [string]$relativeDir, [string]$solution )

	# dotnet restore "..\$relativeDir"
	& $nuget restore "..\$relativeDir\$solution"
}

function Build
{
	param( [string]$relativeDir, [string]$solution, [string]$configuration )
	
	Restore $relativeDir $solution

	& $msbuild "..\$relativeDir\$solution" "/p:Configuration=$configuration"
}

Restore "Simulation\Engine" "Ephemeris.ECS-Alpha.sln"
Build "Simulation" "PlayGen.ITAlert.sln" "Debug"

Restore "Server\photon-plugin" "PlayGen.Photon.Plugin.sln"
Build "Server" "PlayGen.ITAlert.Photon.sln" "Debug"

Build "Unity\PlayGen.ITAlert.Unity" "PlayGen.ITAlert.Unity.sln" "Debug UNITY_EDITOR"

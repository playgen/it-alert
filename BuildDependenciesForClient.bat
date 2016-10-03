SET MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"

CALL :BUILD "SUGAR\PlayGen.SUGAR.sln" "Debug"
CALL :BUILD "Simulation\PlayGen.ITAlert.Client.sln" "UnityDebug"
CALL :BUILD "GameWork\GameWork.sln" "Unity Client Debug"
CALL :BUILD "Server\src-server\Loadbalancing\Loadbalancing.sln" "Unity Debug"

PAUSE

:BUILD

%MSBUILD% %1 /p:Configuration=%2

EXIT/B 0



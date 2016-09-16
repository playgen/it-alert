@echo OFF 
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86
echo .
echo Starting Photon Playgen.ITAlert Build
echo .
devenv "%~dp0\photon-server\PlayGen.ITAlert\PlayGen.ItAlert.sln" /rebuild "Debug"
echo .
echo Starting Loadbalancing Build
echo .
devenv "%~dp0\photon-server\src-server\Loadbalancing\LoadBalancing.sln" /rebuild "Unity Debug"
echo .
echo Starting GameWork Build
echo .
devenv "%~dp0\GameWork\GameWork.sln" /rebuild "UnityEngine Debug"
echo .
echo Starting Playgen.ITAlert Build
echo .
devenv "%~dp0\PlayGen.ITAlert\PlayGen.ItAlert.sln" /rebuild "UnityDebug"
echo .
pause
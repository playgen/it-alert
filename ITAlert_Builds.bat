@echo OFF 
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x86
echo .
echo Starting Photon Playgen.ITAlert Build
echo .
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "%~dp0\photon-server\PlayGen.ITAlert\PlayGen.ItAlert.sln" /rebuild "Debug" /out
echo .
echo Starting Loadbalancing Build
echo .
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "%~dp0\photon-server\src-server\Loadbalancing\LoadBalancing.sln" /rebuild "Unity Debug" /out
echo .
echo Starting GameWork Build
echo .
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "%~dp0\GameWork\GameWork.sln" /rebuild "UnityEngine Debug" /out
echo .
echo Starting Playgen.ITAlert Build
echo .
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "%~dp0\PlayGen.ITAlert\PlayGen.ItAlert.sln" /rebuild "UnityDebug" /out
echo .

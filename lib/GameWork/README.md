GameWork.Unity

C# Framework for Unity, extending the C# .Net GameWork.Core framework.

Purpose:

Collection and tools and scripts to aid development in Unity.

Dependencies:

GameWork.Core which is added as a submodule.

Build Tools:
RunAll: Run .\Tools\BuildAll.ps1 which will do all the below steps.

Individual Steps:
1. Build .\GameWork.Core\GameWork.Core.sln (GameWork.Unity is dependant on this project)
2. Run .\Tools\CreateCoreReferrenceJunctions.bat (this will create the symlinks required for GameWork.Unity.sln to resolve any dependencies on GameWork.Core)
3. Build .\GameWork.Unity.sln.
4. Run .\Tools\CreateUnityReferenceJunctions.bat (This will create the symlinks necessary for the UnityProject to build the GameWork.unitypackage)
5. Open the UnityProject in the Unity Editor and use the menu Tools/Build GameWork Package to build the GameWork.unitypackage.
6. Find the GameWork.unitypackage in .\Build


Source:
https://github.com/Game-Work/GameWork.Unity
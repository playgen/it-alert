# Overview 
IT Alert! is a system admin, network protection, cooperative survival game.

It is part of the [RAGE project](http://rageproject.eu/).

# Licensing
See the [LICENCE](LICENCE.md) file included in this project.

# Key Project Structure
- **doc**: *documentation generated with DocFX*.
- **lib**: *precompiled [Included Assets](#Included-Assets)* 
  - **GameWork**: *Game Development Framework.*  
  - **High-Speed-Priority-Queue-for-C-Sharp**: *used by the simulation for pathfinding.*
  - **Photon**: *used by the photon server.*
  - **SUGAR**: *Social Gamification Backend*
  - **Unity**: *prequired Unity dlls.*
- **Server**: *server side code, based on the Photon server architecture*
- **Simulation**: *client side code*
- **Tools**: *batch files for rebuilding project*
- **Unity**: *IT Alert! Unity project files*
  - **PlayGen.ITAlert.Unity**: *precompiled game Logic, builds to Assets/PlayGen.ITAlert.Unity*
  - **PlayGen.ITAlert.Installer**: *[WiX](http://wixtoolset.org/) installer project*

# Included Assets:
- [SUGAR](http://www.sugarengine.org/) is a Social Gamification Backend.
- [ExcelToJsonConverter](https://github.com/Benzino/ExcelToJsonConverter): is used to convert Excel Localization files to jSON, game uses a modified version of the original to handle rich text more efficiently
- [PlayGen Unity Utilities](https://gitlab.com/playgen/unity-utilities): is a collection of simple game utilities.
- [GameWork](https://github.com/Game-Work/GameWork.Unity) is a game development framework. 
- [Photon](https://www.photonengine.com/en/OnPremise) Game Server Backend.

todo ECS, Simulation, etc

# Cloning
- When the project is cloned the ECS solution will need to be opened to perform a NuGet package restore, this will only need to be repeated if the ECS is updated with new package dependencies.
- If cloning via SourceTree or another graphical git tool the submodules must be manually opened to force the LFS content to be pulled.

# Development:
## Requirements:
- Windows
- Git LFS
- Visual Studio 2017
- Unity Editor

## Environment Setup
For .pdb debugging it is expected that you work from `P:\it-alert` so that the source files can be resolved.

If you are going to be commiting DLLs or want to debug code inside the DLLs, you will need to do the following:

1. Open CMD.
2. Map the parent folder of your IT Alert repository folder to the drive letter P:  
`SUBST P: [`Parent dir of it-alert project`]`  
If the path of the it-alert project is C:\Users\Bob\Projects\it-alert, the command would be:  
`SUBST P: C:\Users\Bob\Projects`
3. Navigate to P:\it-alert to make sure the mapping has worked.
4. Remember to always use this path when working on it-alert.

## Run
1. This project uses Git Submodules so make sure those have been pulled too.  
At the time of writing, this can be done with: `git submodule update --recursive` if already cloned  
or to clone: `git clone --recurse-submodules [it alert repository url]`  
Note: Make sure that Submodules have been pulled properly. Source Tree likes to ignore the Newtonsoft.Json Submodule.

2. Build Simulation/PlayGen.ITAlert.sln.

3. Run Server/PlayGen.ITAlert.Photon.sln (Make sure PlayGen.ITAlert.Photon.Plugin is set as startup project).

4. Build Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity.sln.

5. Run Tools/CreateLibJunctions.bat to setup the required symlinks so the correct dlls are included (**you need to do this before opening .Unity so Unity doesn't create the folders instead**). This will create 2 symlinks \Unity\Assets\Gamework and \Unity\Assets\SUGAR

6. Open and run Unity/ in the Unity Editor.


## Conventions
- Work from P:\it-alert.
- Commit .pdb files when committing .dlls.

## SUGAR
### Build Instructions
Currently using the unity client built from commit: 65b3f05407eb5f58096c1ae862da18ab8d643e1c

1. Build SUGAR-Unity
2. Copy build output of SUGAR-Unity build to lib/SUGAR/bin

OR

1. Build the SUGAR-Unity unitypackage
2. Import into IT Alert

# Deployment
## Run Instructions
To run the Photon Server as a service, see this [ReadMe](Server/deploy/ReadMe.md).

# Unity Project
For more information about the unity project, see this [Guide](doc/guides/Unity/UnityProject.md)

# Installer:
[Wix](http://wixtoolset.org/) is used to create the Windows installer.

The game-launcher repository is used to launch the game from url.

## Requirements:
- Wix Toolset
- Visual Studio 2017
- Wix Visual Studio Extension
- [game-launcher](https://gitlab.com/playgen/game-launcher) project

## Process:
1. Create a Unity PC Build at Unity\Build\ITAlert_StandaloneWindows64 called “ITAlert_StandaloneWindows64”.
2. Once built, go to the project solution (Unity/PlayGen.ITAlert.Installer) and build the PlayGen.ITAlert.Installer project.
3. The resulting windows installer can be found at Unity/PlayGen.ITAlert.Installer/bin/PlayGen.ITAlert.Installer.msi

## Setting up your game with SUGAR
For information on Setting up IT Alert!. using SUGAR, see [SUGAR Quick Start Guide](http://api.sugarengine.org/v1/unity-client/tutorials/quick-start.html). *make sure that Assets\StreamingAssets\SUGAR.config.json exists and the BaseUri value matches the Base Address in the SUGAR Prefab.* 

``` c#
  {
    "BaseUri": "http://api.sugarengine.org/v1/"
  }
````

## Running SUGAR Locally
Using IT Alert!. with a local version of SUGAR is as simple as changing the Base Address in the SUGAR Prefab, and the BaseUri value in *Assets\StreamingAssets\SUGAR.config.json*

## Developer Guide
For more information of project structure for Game, Server and Client, see the the [Developer Guide](doc/guides/DeveloperGuide.md)
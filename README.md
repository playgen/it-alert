# Overview 
IT Alert! is a system admin, network protection, cooperative survival game.

It is part of the [RAGE project](http://rageproject.eu/).

The project has three main areas: 
- The **client**.
- The **server**.
- The shared **simulation**.

# Licensing
See the [LICENCE](LICENCE.md) file included in this project.

# Cloning
- When the project is cloned the ECS solution will need to be opened to perform a NuGet package restore, this will only need to be repeated if the ECS is updated with new package dependencies.
- If cloning via SourceTree or another graphical git tool plese ensure that the LFS content has been pulled for all of the submodules.
- Within the Unity folder you will need to run the CreateLibJunctions.bat file in order to set up the GameWork SymLink required for the project structure to be set up correctly.

# Key Project Structure
- **doc**: *Documentation*
- **lib**: *Precompiled [Included Assets](#Included-Assets) used by the client and server.* 
  - **GameWork**: *Game Development Framework.*  
  - **High-Speed-Priority-Queue-for-C-Sharp**: *Used by the simulation for pathfinding.*
  - **Photon**: *Used by the Photon server.*  
- **Server**: *[Server side code](Server/README.md), based on the Photon server architecture.*
- **Simulation**: *[Simulation source](Simulation/README.md).*
- **Unity**: *[IT Alert Game Client Unity project files](Unity/ReadMe.md).*
  - **Assets**
    - **SUGAR**: *Social Gamification Unity Client. See [Included Assets](#Included-Assets).*
  - **lib**: *Precompiled [Included Assets](#Included-Assets) used by the client.*     
    - **Unity**: *Prequired Unity DLLs.*
  - **PlayGen.ITAlert.Unity**: *[Source game client logic](Unity/PlayGen.ITAlert.Unity/README.md), builds to Assets/PlayGen.ITAlert.Unity.*
  - **Tools**
    - **CreateLibJunctions.bat**: *Setup script to create SymLink to necessary lib files.*
  - **PlayGen.ITAlert.Installer**: *[WiX](http://wixtoolset.org/) installer project.*

# Included Assets:
- [Client-Side Tracker](https://gamecomponents.eu/content/232)
- [Server-Side Interaction Storage and Analytics](https://www.gamecomponents.eu/content/220)
- [Server-Side Dashboard and Analysis](https://www.gamecomponents.eu/content/195)
- [Evaluation Component](https://gamecomponents.eu/content/338)
- [SUGAR](https://gamecomponents.eu/content/200)
- [PlayGen Unity Utilities](https://github.com/playgen/unity-utilities) - a collection of simple Unity utilities.
- [GameWork](https://github.com/JaredGG/GameWork.Unity) - a game development framework.
- [ExcelToJsonConverter](https://github.com/Benzino/ExcelToJsonConverter) - used to convert Excel Localization files to JSON.
- [Photon](https://www.photonengine.com/en/OnPremise) Game Server Backend.

# Development
## Requirements
- Windows
- Git LFS
- Visual Studio 2017
- Unity Editor

## Environment Setup
For .pdb debugging it is expected that you work from `P:\it-alert` so that the source files can be resolved.

If you are going to be commiting DLLs or want to debug code inside the DLLs, you will need to do the following:

1. Open CMD.
2. We will refer to the parent folder of your IT Alert repository as your "Projects Folder".
3. Map your Projects Folder to the drive letter P:  
`SUBST P: [`Parent dir of it-alert project`]`  
If the path of the it-alert project is C:\Users\Bob\Projects\it-alert, the command would be:  
`SUBST P: C:\Users\Bob\Projects`
3. Navigate to P:\it-alert to make sure the mapping has worked.
4. Remember to always use this path when working on it-alert.
5. Pull any of the repositories with source code you want to debug to your Projects Folder, leaving the cloned repository name as the default that git has defined.

## Run Process
1. This project uses Git Submodules so make sure those have been pulled too.  
At the time of writing, this can be done with: `git submodule update --recursive` if already cloned  
or to clone: `git clone --recurse-submodules [it alert repository url]`  
Note: Make sure that Submodules have been pulled properly. SourceTree likes to ignore the Newtonsoft.Json Submodule.

2. Build Engine/Ephemeris.ECS-Alpha.sln and Simulation/PlayGen.ITAlert.sln.

3. Run Server/PlayGen.ITAlert.Photon.sln (Make sure PlayGen.ITAlert.Photon.Plugin is set as startup project).

4. Build Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity.sln.

5. Run Unity/Tools/CreateLibJunctions.bat to setup the required symlinks so the correct dlls are included (**you need to do this before opening .Unity so Unity doesn't create the folders instead**). This will create a symlink at \Unity\Assets\Gamework.

6. Open and run Unity/ in the Unity Editor.

## Updating
### GameWork.Unity
Build solution and place new DLLs (found in GameWork.Unity\bin) into lib\GameWork folder. Note that code changes could be needed as a result of updates to this asset.

**Commit hash: 37b623daf815667d6f523c38ab47304bfac82b22**

### PlayGen Unity Utilities 
Build solution and place new DLLs (found in various folders in this project) into lib\PlayGenUtilities folder. Note that code changes could be needed as a result of updates to this asset. New prefabs may also need copying, although take care not to overwrite customised versions of previous prefabs.  

New DLLs should also be copied into the lib\PlayGen Utilities folder in the SUGAR Unity project.

**Commit hash: 99d0daaa429430b36807bc5c28e567a61fc75e7d**

### SUGAR Unity Asset
Build solution and place new DLLs (found in SUGAR-Unity\Assets\SUGAR\Plugins) into Assets\SUGAR\Plugins folder. Note that code changes could be needed as a result of updates to this asset. It is advised that you do not remove the prefabs for SUGAR, as these have been edited to match the styling of Space Modules Inc. Only replace these assets if they are no longer compatible with the latest version of the SUGAR Unity asset, and even then be aware that you will need to recreate the previous styling.

**Commit hash: 51bbdcd3658af28823471a09f3be89dff0b641f9**

### RAGE Analytics
Follow the instructions provided in the [RAGE Analytics Documentation](Assets/RAGE%20Analytics/ReadMe.md).

**Commit hash: 652a562c11d3b2ddb85bae509a719d30ed6ecd0c**

### Evaluation Asset
Follow the instructions provided in the [Evaluation Asset Documentation](Assets/Evaluation%20Asset/ReadMe.md).

**Commit hash: 6c4551df61ac1a1829ed0cbf7b9788362ee1342a**

## Conventions
- Work from P:\it-alert.
- Commit .pdb files when committing .dlls.

## Developer Guide
For detailed information on the project structure for the Game Logic, Server and Client, see the the [Developer Guide](docs/guides/DeveloperGuide.md).

## Versioning
Each main area of the IT Alert project has its own version located in:  
- **Client**: [*Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity/Version.cs*](Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity/Version.cs)
- **Server**: [*Server/PlayGen.ITAlert.Photon.Common/Version.cs*](Server/PlayGen.ITAlert.Photon.Common/Version.cs)
- **Simulation**: [*Simulation/PlayGen.ITAlert.Simulation/Version.cs*](Simulation/PlayGen.ITAlert.Simulation/Version.cs)

The version is composed of:
- **Major**: Increment for backwards compatibility breaking changes.
- **Minor**: Increment for features.
- **Build**: Increment for bug fixes, minor changes etc that result in a new build.

# Build
## Run Instructions
To run the Photon Server as a service, see the [Deployment Documenetation](Server/deploy/README.md).

# Installer:
[Wix](http://wixtoolset.org/) is used to create the Windows installer.

Using the game-launcher repository, games can be launched using a url.

## Requirements:
- Wix Toolset
- Visual Studio 2017
- Wix Visual Studio Extension
- [Game Launcher](https://gitlab.com/playgen/game-launcher) project

## Process
1. Create a Unity PC Build at Unity\Build\ITAlert_StandaloneWindows64 called “ITAlert_StandaloneWindows64”.
2. Once built, go to the project solution (Unity/PlayGen.ITAlert.Installer) and build the PlayGen.ITAlert.Installer project.
3. The resulting windows installer can be found at Unity/PlayGen.ITAlert.Installer/bin/PlayGen.ITAlert.Installer.msi

The process for setting up a game installer is also detailed within the [Game Launcher documentation](https://gitlab.com/playgen/game-launcher/blob/master/ReadMe.md#game-installer).

``` c#
  {
    "BaseUri": "http://api.sugarengine.org/v1/"
  }
````
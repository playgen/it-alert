# Overview 
IT Alert! is a system admin, network protection, multiplayer cooperative survival game.

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
- **Server**: *todo*

todo


# Included Assets:
- [SUGAR](http://www.sugarengine.org/) is a Social Gamification Backend.
- ExcelToJsonConverter: is used to convert Excel Localization files to jSON.
- [PlayGen Unity Utilities](git@codebasehq.com:playgen/components/unityutilities.git): is a collection of simple game utilities.
- [GameWork](https://github.com/Game-Work/GameWork.Unity) is a game development framework. 
- [Photon](https://www.photonengine.com/en/OnPremise) Game Server Backend.

todo ECS, Simulation, etc


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

5. Run Tools/CreateLibJunctions.bat to setup the required symlinks so the correct dlls are included (you need to do this before opening .Unity so Unity doesn't create the folders instead).

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
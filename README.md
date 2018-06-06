# Cloning
This project uses Git LFS so make sure that it is installed on your system.
It also makes use of Git Submodules so when cloning, remember to clone the submodules too.

At the time of writing, this can be done with: `git clone --recurse-submodules [it alert repository url]`

# Project Structure
IT Alert! is made up of 3 major components:

## Simulation
- Visual Studio Solution: Simulation/PlayGen.ItAlert.sln

## Photon Game Server
- Visual Studio Solution: Server/PlayGen.ITAlert.Photon.sln
- This requires the Simulation to be built.

## Unity Game
- Visual Studio Solution: Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity.sln
- Unity Project: Unity/
- The Unity Project requires the Simulation, the Photon Game and the ServerPlayGen.ITAlert.Unity.sln to be built.
- [Unity Game ReadMe.](Unity/ReadMe.md)

## Other Components
Other components that are included and are pre-built reside in:

lib/

- [GameWork/](https://github.com/Game-Work/GameWork.Unity) is a game development framework. This is used in the Unity Game as well as the Photon Game Server, mostly for game state management.
- [SUGAR/](http://api.sugarengine.org/v1/) is a Social Gamification Backend. This provides the account management for ITAlert.
- [High-Speed-Priority-Queue-for-C-Sharp/](https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp) is used in the simulation's pathfinding.
- Unity/ contains the dlls bundled with the Unity Editor that are necessary for the PlayGen.ITAlert.Unity.sln to incorporate Unity functionality.
- Photon/ used by the Photon Game Server.


Note: 
Make sure that Submodules have been pulled properly. Source Tree likes to ignore the Newtonsoft.Json Submodule

# Development
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

## Conventions
- Work from P:\it-alert.
- Commit .pdb files when committing .dlls.

## Run Instructions
1. Build Simulation/PlayGen.ITAlert.sln
2. Run Server/PlayGen.ITAlert.Photon.sln (Make sure PlayGen.ITAlert.Photon.Plugin is set as startup project)
3. Build Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity.sln
4. Run Tools/CreateLibJunctions.bat to setup the required symlinks so the correct dlls are included (you need to do this before opening Unity so Unity doesn't create the folders instead).
5. Open and run Unity/ in the Unity Editor.

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
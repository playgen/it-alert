# Unity Project
The Unity project has 1 scene (Unity\Assets\Scenes\IT Alert.unity) from which all User Interfaces for the game states are set up. Game logic is compiled from Unity\PlayGen.ITAlert.Unity.sln and built into the Unity\Assets\PlayGen.ITAlert.Unity folder

## Project Structure
  - **Assets**
      - **Editor**: *Utilities that are used in the editor for setup and ease, these are not included in builds*
      - **Prefabs**: *UI objects for loading in during runtime, objects in 'Resources' can be loaded with Resource.Load*
      - **Sprites**: *UI Textures, for use on Unity UI components*
      - **Scenes/IT Alert.unity**: *the main scene for the project*
  - **PlayGen.ITAlert.Unity/**: *source project containing game Logic, builds to Assets/PlayGen.ITAlert.Unity*
  - **PlayGen.ITAlert.Installer**: *[WiX](http://wixtoolset.org/) installer project*
  - **Tools**
    - **CreateLibJunctions.bat**: *Creates symlinks for the required dependencies.*

## Scene Structure
- **Camera**: *The Main Camera*
- **SUGAR**: *prefab containing all components relating to SUGAR *
- **Game**: *game UI*
  - **Canvas**: *in game UI*
  - **End Canvas**: *post game UI*
- **Menu**: *menu UI*
  - **[stateName]Container**: *UI for each state within the menu*
- **Voice**: *voice panel UI*
- **Popup**: *generic popup UI container*
- **Hover**: *hover container for in game information*
- **Loading**: *loading spinner panel, from PlayGen Utilities*

# Configuration
In Unity/Assets/StreamingAssets you can find configurations for the following:

## Photon Client
The Unity/Assets/StreamingAssets/Photon.config file is loaded at startup and used to configure the [Photon Unity Client](https://www.photonengine.com/en-us/PUN).

## SUGAR Client
In the ITAlert.unity scene, you can configure the SUGAR client by selecting the SUGAR game object in the scene hierarchy view.

The SugarUnityManager component has the various configurable settings including the SUGAR server url to use.

For more info on the SUGAR Unity Client see [here](http://api.sugarengine.org/v1/unity-client/index.html).

# Development
1. Run Unity/Tools/CreateLibJunctions.bat to setup the required symlinks so the correct dlls are included (**you need to do this before opening .Unity so Unity doesn't create the folders instead**). This will create 2 symlinks \Unity\Assets\Gamework and \Unity\Assets\SUGAR

# Build
## Requirements
- The [visual studio client project](PlayGen.ITAlert.Unity/ReadMe.md) has already been built.
- The [Development](#Development) steps have been followed.

## Android
At the time of writing, builds need to be done using the Unity Build Setting "Internal" rather than "Gradle".
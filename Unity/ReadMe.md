# Unity Project
The Unity project has one scene (Unity\Assets\Scenes\IT Alert.unity) which contains all menu interfaces and gameplay. Game logic is compiled from Unity\PlayGen.ITAlert.Unity.sln and built into the Unity\Assets\PlayGen.ITAlert.Unity folder.

## Project Structure
  - **Assets**
    - **Animations**: *Animations used when interacting with menu items and when displaying item and item container usage options.*
    - **Debugging**: *Obsolete debugging functionality.*
    - **Editor**: *Utilities that are used in the editor for setup and ease and are not included in builds.*
    - **Fonts**
    - **GameWork**: *GameWork DLLs.*
    - **Materials**
    - **PlayGen.ITAlert.Unity**: *DLLs from PlayGen.ITAlert.Unity solution*.
    - **Prefabs**
    - **Resources**: *Contains Localization JSON.*
    - **Scenes**: *Contains the sole game scene (ITAlert.unity).*
    - **Sprites**: *Contains all textures, all intended for use on Unity UI Image components.*
    - **StreamingAssets**: *Contains config files for Photon, SUGAR and level of debugging.*
    - **SUGAR**: *Contains all SUGAR assets, including DLLs and prefabs.*
    - **unity-ui-extensions**: *Contains the Color Picker prefabs.*
    - **Scenes/IT Alert.unity**: *the main scene for the project*
  - **PlayGen.ITAlert.Unity/**: *source solution containing game logic, builds to Assets/PlayGen.ITAlert.Unity*
  - **PlayGen.ITAlert.Installer**: *[WiX](http://wixtoolset.org/) installer project*
  - **Tools**
    - **CreateLibJunctions.bat**: *Creates symlinks for the required dependencies.*

## Key Scene Structure
- **State**: *Contains the PhotonConfigLoader and GameBehaviour MonoBehaviours that are fundermental to the game working.*
- **SUGAR**: *Prefab containing all components relating to SUGAR.*
- **Game**: *Game UI*
  - **Canvas**: *In-game UI.*
  - **End Canvas**: *Post-game UI.*
- **Menu**: *Menu UI.*
  - **[stateName]Container**: *UI for each state within the menu system.*
- **Voice**: *Voice panel UI, displayed when in a lobby or game.*
- **Popup**: *Generic pop-up UI container, currently primarily in use for errors.*
- **Hover**: *Hover container for in game information. On a separate canvas to reduce performance impact.*
- **Loading**: *loading spinner panel from PlayGen Utilities.*

# Configuration
In Unity/Assets/StreamingAssets you can find configurations for the following:
- **Debug**: *Configure how much debug information is displayed in the editor console/log file. Numbers map to the LogLevel enum in GameWork.*
- **Photon**: *Configure which server to connect to and game settings related to networking and Photon. Configures the used to configure the [Photon Unity Client](https://www.photonengine.com/en-us/PUN).*
- **SUGAR**: *Confgure which SUGAR backend to use.*

# Development
1. Run Unity/Tools/CreateLibJunctions.bat to setup the required symlinks so the correct dlls are included (**you need to do this before opening Unity so it doesn't create the folders instead**). This will create a symlink for \Unity\Assets\Gamework.

# Build
## Requirements
- The [visual studio client project](PlayGen.ITAlert.Unity/ReadMe.md) has already been built.
- The [Development](#Development) steps have been followed.

## Android
At the time of writing, builds need to be done using the Unity Build Setting "Internal" rather than "Gradle".
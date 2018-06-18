# Unity Project
The current version of Unity that IT Alert! uses can be found [here](../../../Unity/ProjectSettings/ProjectVersion.txt)

The Unity project has 1 scene (Unity\Assets\Scenes\IT Alert.unity) from which all User Interfaces for the game states are set up. Game logic is compiled from Unity\PlayGen.ITAlert.Unity.sln and built into the Unity\Assets\PlayGen.ITAlert.Unity folder

## Project Structure
- **Unity/**: *unity project directory, open folder in Unity*
    - **Assets/**: *unity project assets*
        - **Editor**: *Utilities that are used in the editor for setup and ease, these are not included in builds*
        - **Prefabs**: *UI objects for loading in during runtime, objects in 'Resources' can be loaded with Resource.Load*
        - **Sprites**: *UI Textures, for use on Unity UI components*
        - ****: **
        - **Scenes/IT Alert.unity**: *the main scene for the project*
    - **PlayGen.ITAlert.Unity/**: *precompiled game Logic, builds to Assets/PlayGen.ITAlert.Unity*
    - **PlayGen.ITAlert.Installer**: *[WiX](http://wixtoolset.org/) installer project*

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
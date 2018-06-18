# Game Logic
Following the ECS (Entity Component System) pattern, game entities are containers for collections of components. Components store state and have little or no logic defined. Game Logic is implemented via Systems that modify the state of components associated with specific entities. Entities are generalised through templates called Archetypes that define component configuration and initial state. 

The project is located at *Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity.sln*, and once built, is moved to the Unity/Assets/PlayGen.ITAlert.Unity folder. 

## Project Structure
- **PlayGen.ITAlert.Unity**
- **PlayGen.ITAlert.Unity.Tests**
    - All Client game logic, which will be reflected in game UI in Unity project
        - Client Commands (Join, Leave, Create etc.)
        - Behaviours for states in game
        - Definition of Game States 
        - Interaction Controllers (Join Game, Popups, Scenarios, Hover)
        - Photon Client side logic
        - Tracking for Player Metrics for end game summary
        - LogProxy for logging to unity. 
- **PlayGen.ITAlert.Unity.Editor**
    - Useful utilities for checking functionality in the Unity Editor. 
        - Character validity check for fonts
        - PlaymodeStateChanged, used for stopping the director when the editor state changes to prevent unity from crashing
- **PlayGen.Photon.Unity**
    - Photon client wrapper for using Photon in unity and retrieving client status'
- **UnityEngine.UI.Extensions.ColorPicker**
    - Config values for colors available in game, and events that occur when colors change 
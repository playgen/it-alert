# Game Logic
Following the ECS (Entity Component System) pattern, game entities are containers for collections of components. Components store state and have little or no logic defined. Game Logic is implemented via Systems that modify the state of components associated with specific entities. Entities are generalised through templates called Archetypes that define component configuration and initial state. 

The project is located at *Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity.sln*, and once built, is moved to the Unity/Assets/PlayGen.ITAlert.Unity folder. 

This project uses the [Photon Unity Client](https://www.photonengine.com/en/PUN) to communicate with the server.

## Visual Studio Project Structure
- **PlayGen.ITAlert.Unity**: *All Client game logic, which will be reflected in game UI in Unity project.*
  - **Behaviours**: *for states in game.*
  - **Commands**: *Client Commands (Join, Leave, Create etc.).*  
  - **Controllers**: *Interaction Controllers (Join Game, Popups, Scenarios, Hover).*
  - **States**: *Game States.*
  - **Photon**: *Photon Client side logic.*
  - **Simulation**: *Client side logic to reproduce the simulation.*
    - **Summary**: *Post simulation summary statistics.*
        - **PlayerMetrics.cs**: *Calculations for Player Metrics.*        
  - **LogProxy.cs**: *LogProxy for logging to unity.*
- **PlayGen.ITAlert.Unity.Tests**    
- **PlayGen.ITAlert.Unity.Editor**: *Useful utilities for checking functionality in the Unity Editor.*
- **PlayGen.Photon.Unity**: *Photon client wrapper for using Photon in unity and retrieving client status'.*
- **UnityEngine.UI.Extensions.ColorPicker**: *Config values for colors available in game, and events that occur when colors change.*
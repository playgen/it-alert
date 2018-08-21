# Game Logic
Following the ECS (Entity Component System) pattern, game entities are containers for collections of components. Components store state and have little or no logic defined. Game Logic is implemented via Systems that modify the state of components associated with specific entities. Entities are generalised through templates called Archetypes that define component configuration and initial state. 

The project is located at *Unity/PlayGen.ITAlert.Unity/PlayGen.ITAlert.Unity.sln*, and once built, is moved to the Unity/Assets/PlayGen.ITAlert.Unity folder. 

This project uses the [Photon Unity Client](https://www.photonengine.com/en/PUN) to communicate with the server.

## Visual Studio Project Structure
- **PlayGen.ITAlert.Unity**: *All Client game logic, which will be reflected in game UI in Unity project.*
  - **Behaviours**: *MonoBehaviours used to control object functionality.*
  - **Commands**: *Client Commands (Join, Leave, Create etc.).*  
  - **Controllers**: *Interaction Controllers (Join Game, Popups, Scenarios, Hover).*
  - **States**: *Management of transitions between states and how the user interface should be displayed and act within each state.*
  - **Photon**: *Photon Client side logic.*
  - **Simulation**: *Client side logic to display the simulation.*
    - **Summary**: *Post simulation summary statistics.*
        - **PlayerMetrics.cs**: *Calculations for Player Metrics.*        
  - **LogProxy.cs**: *LogProxy for logging to Unity in place of Debug.Log etc.*
- **PlayGen.ITAlert.Unity.Tests**    
- **PlayGen.Photon.Unity**: *Photon client wrapper for using Photon in Unity and retrieving client status.*
- **UnityEngine.UI.Extensions.ColorPicker**: *Contains logic relating to the ColorPicker previously used to select player color in-game.*
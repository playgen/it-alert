# IT Alert! Client/Server Simulation Loop
The Simulation Loop is based around Photon Networking, see [Shared Game Logic](#shared-game-logic). 

The Simulation Loop executes commands from clients on the server, then broadcasts the new state to clients. The following steps show the process:
1. Player Commands are sent immediately to the server
2. Server deduplicates redundant commands
3. On tick, server attempts to process queued commands
4. Successfully processed commands are buffered for broadcast
5. Scenario frame is processed
6. Simulation ticks
7. Commands and checksum broadcast to clients

![Simulation Loop](images/SimulationLoop.png)

# Project Architecture
![Project Architecture](images/ProjectArchitecture.png)

The project is primarily composed of three solutions, shared game logic, server and client code

## Shared Game Logic
Common logic that is shared between to server and client

The game uses a client/server architecture based around Photon Networking. Photon uses a plugin architecture where each game instance maps to a logical ‘room’. For each room an instance of the plugin is instantiated within the load balancing host. Messages are transferred between client and server over TCP or UDP transports, optionally using WebSockets for proxy traversal.
- [OnPremise](https://www.photonengine.com/en/OnPremise)
- [PUN](https://www.photonengine.com/en/PUN)

### Location
- **Simulation/PlayGen.ItAlert.sln** [Structure](Simulation/Simulation.md)

## Server Side Code
The server code was based on the OnPremise server Loadbalancing example which has been structurally altered to suit the needs of the project. Upgrading the photon dependencies, to for example deploy to the Photon cloud, would involve mapping the photon code onto the altered directory structure and upgrading the required binary references within the Photon-Plugin and PlayGen.ITAlert.Photon solution.

### Location
- **Server/PlayGen.ITAlert.Photon.sln** [Structure](Server/Photon.md)

## Client Side Code
The client integration has modified the structure of the PUN unity package deployment to suit the DLL based workflow required to share assemblies between unity and non-unity projects. 

### Location
- **Unity/PlayGen.ITAlert.Unity/**: *precompiled game Logic, builds to Assets/PlayGen.ITAlert.Unity* [Structure](Unity/Unity.md)

## Unity Project
The unity project controls the UI elements in game and uses the compiled Client Side Code

### Location
- **Unity/**: *unity project directory, open folder in Unity* [Structure](Unity/UnityProject.md)

# Creating Scenarios
For information on creating new scenarios, see [Game Scenarios](Simulation/GameScenarios.md)
# Developer Guide
The following guide will provide more information about the project architecture and game/server sequencing, looking at each of the key projects and providing an overview of what tasks each one does.

The following topics are covered:
- [Project Architecture](#project-architecture) - Overview of project structure and functionality.
- [Scenario Creation](#creating-scenarios) - Introduction to how game scenarios are created.

# IT Alert! Client/Server Simulation Loop
The game uses a client/server architecture based around Photon Networking. Photon uses a plugin architecture where each game instance maps to a logical ‘room’. For each room an instance of the plugin is instantiated within the load balancing host. Messages are transferred between client and server over TCP or UDP transports, optionally using WebSockets for proxy traversal.
- [Server](https://www.photonengine.com/en/OnPremise)
- [Client](https://www.photonengine.com/en/PUN)

The Simulation Loop executes commands from clients on the server, then broadcasts the new state to clients. The following steps show the process:
1. Player Commands are sent immediately to the server
2. Server deduplicates redundant commands
3. On tick, server attempts to process queued commands
4. Successfully processed commands are buffered for broadcast
5. Scenario frame is processed
6. Simulation ticks
7. Commands and checksum broadcast to clients

![Simulation Loop](images/SimulationLoop.png)

## Simulation
The simulation logic is shared between to server and client. See the [Simulation Documentation](../../Simulation/README.md) for further information.  

## Server Code
The server code was based on the OnPremise server Loadbalancing example which has been structurally altered to suit the needs of the project. Upgrading the photon dependencies, to for example deploy to the Photon cloud, would involve mapping the photon code onto the altered directory structure and upgrading the required binary references within the Photon-Plugin and PlayGen.ITAlert.Photon solution. See the [Server Documentation](../../Server/README.md) for further information.

## Client Code
The client integration has modified the structure of the PUN unity package deployment to suit the DLL based workflow required to share assemblies between Unity and non-Unity projects. See the [Unity Solution Documentation](../../Unity/PlayGen.ITAlert.Unity/README.md) for further information.

## Client Unity Project
The Unity project controls the UI elements in game and uses the compiled Client Side Code. See the [Unity Documentation](../../Unity/ReadMe.md) for further information.    

![Project Architecture](images/ProjectArchitecture.png)

# Creating Scenarios
Each playable 'level' in IT Alert is called a scenario. These are currently hard coded within Simulation/PlayGen.ItAlert.Simulation.Configuration, which contains configurations for level layouts, items available, virus types, and win/loss conditions. 
- [Creating New Scenarios](GameScenarios.md#game-scenarios)
- [Example Scenario](ScenarioCreation.md#example-scenario)
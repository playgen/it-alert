# Simulation Logic
The simulation project is found at *Simulation/Playgen.ItAlert.sln*.

# Key Project Structure
- **Engine**: *[Entity-Component System](Engine/README.md)*
- **lib**: *precompiled referenced .dlls.*
- **Tools**
  - **ExcelToJsonConverter**: *Used in the Scenario Localization project to generate the .json localization file from .xlsx.*

# Visual Studio Project Structure
This project contains shared logic between both Server and Client
- **PlayGen.ItAlert.Simulation.Modules**
    - **PlayGen.ITAlert.Simulation.Modules.Antivirus**: *Antivirus, capture and anaylsis system enhancement and tools.* 
    - **PlayGen.ITAlert.Simulation.Modules.GarbageDisposal**: *Garbage disposal system enhancement.*
    - **PlayGen.ITAlert.Simulation.Modules.Malware**: *Malware propagation and detection components.*
    - **PlayGen.ITAlert.Simulation.Modules.Resources**: *Subsystem and connection resource components.*
    - **PlayGen.ITAlert.Simulation.Modules.Transfer**: *Transfer system enhancements.*
    - **PlayGen.ITAlert.Simulation.Modules.Tutorial**: *Tutorial system components.*
- **PlayGen.ItAlert.Simulation**
- **PlayGen.ItAlert.Simulation.Tests**: *Base project containing config files and object definitions.*
- **PlayGen.ItAlert.Simulation.Archetypes**: *Base archetypes to be extended in PlayGen.ITAlert.Simulation.Modules.*
- **PlayGen.ItAlert.Simulation.Common**
- **PlayGen.ItAlert.Simulation.Common.Tests**: *Common identifiers used.*
- **PlayGen.ItAlert.Simulation.Configuration**
- **PlayGen.ItAlert.Simulation.Configuration.Tests**: *Scenario Definitions in Scenarios/ and Configuration loader. Definitions of available player colours .*
- **PlayGen.ItAlert.Simulation.Logging**: *Handles logging of events to database for query later, see [Logging](#logging-and-analytics).*
- **PlayGen.ItAlert.Simulation.Scenario**
- **PlayGen.ItAlert.Simulation.Scenario.Tests**: *Creation of scenarios loaded from PlayGen.ITAlert.Simulation.Configuration.*
- **PlayGen.ItAlert.Simulation.Scenario.Localization**: *Localization for retrieval of strings for a given key and langugae.*
- **PlayGen.ItAlert.Simulation.Scenario.Serialization**: *Not currently used. Would handle serialization/deserialization of game scenarios currently defined in PlayGen.ItAlert.Simulation.Configuration.*
- **PlayGen.ItAlert.Simulation.Scoring**: *Scoring system for tracking player scores. Event Handlers that handle scorable events.*
- **PlayGen.ItAlert.Simulation.Serialization**
- **PlayGen.ItAlert.Simulation.Serialization.Tests**: *Not currently used.*
- **PlayGen.ItAlert.Simulation.Startup**: *Handles setup of simulations life cycle.*
- **PlayGen.ItAlert.Simulation.UI**

# Systems
Systems act upon entities through matchers, matchers maintain a collection of all entities that contain a specific set of components. In this manner all entities of a given ‘type’ can be acted upon without knowledge of the archetypes from which they were created.

Systems can implement any of two interfaces depending on the required behaviour:
- **InitializingSystem**: *perform operations before the first tick of the simulation loop, this can be used for set up activities and setting initial state*
- **Tickable System**: *ticked as part of the simulation loop and implement the majority of the game logic by testing conditional actions and updating state on a regular basis*

There are also systems that have no automatic behaviour and are used to accumulate counters, provide services, such as RNG (Random Number Generator) and provide interfaces to components outside of the simulation.

System configuration is currently hardcoded but it is intended that this configuration will be serializable and form part of the parameterizable scenario configuration.

# Multiplayer Implementation

- Game client hosts slace instance of simulations (ECS) 
- Photon Loadbalancing Server hosts multiple, on demand instances of master simulation
- Server broadcasts instructs clients to tick and apply commands, latent clients can fast forward and interpolate
- Clients validate state against server checksum, fail and disconnect if out of sync.

![MultiplayerImplementation](docs/MultiplayerImplementation.png)

# Logging and Analytics 
The ECS and simulation layer provide a database event logging architecture that capture low level data about the game instances and the actions of players.

Table | Column | Type | Value | Comment
--- | --- | --- | --- | --- 
Instances | Id | GUID | {GUID} | 
Instances | Name | String | Game name from creating player | Creating player will usually be player Id 0, unless they left the lobby after other players had joined before the game started
Instances | ScenarioId | String | Selected scenario identifier | 
Instances | Initialized | DateTime | Date/Time game was started
InstancePlayers | Game Id | GUID | Foreign Key Instances => Id |
InstancePlayers | Player Id | int | Player Index (0, 3)
InstanceEvents | GameId | GUID | Foreign Key Instances => Id |
IntsanceEvents | EventId | Int | Unique sequence number within game instance 0..* |
InstanceEvents | PlayerId | Nullable int | Foreign Key InstancePlayers => PlayerId | Null if system event
InstanceEvents | Data | String | JSON blob with event context |
InstanceEvents | Event Code | String | Event type enumeration |
InstanceEvents | Tick | Int | Simulation tick in which event was generated | Timestamp in seconds => Tick/10
PlayerFeedbacks | GameId | GUID | Foreign Key Instances => Id |
PlayerFeedbacks | PlyerId | Int | Foreign Key InstancePlayers => PlayerId | Player providing Feedback
PlayerFeedbacks | RankedPlayerId | Int | Foreign Key InstancePlayers => PlayerId | Player feedback is about
PlayerFeedbacks | LeadershipRank | Int | Ranking in first category 0..6 | 
PlayerFeedbacks | CommunicationRank | Int | Ranking in second category 0..6 |
Player Feedbacks | CooperationRank | Int | Ranking in third category 0..6 | 
InstancePlayerIdentifiers | GameId | GUID | Foreign Key InstancePlayers => PlayerId | 
InstancePlayerIdentifiers | PlayerId | Int | Foreign Key InstancePlayers => PlayerId |
InstancePlayerIdentifiers | IdentifierType | String | Identifier Token | currently: SUGAR, RAGE_CLASS
InstancePlauerIdentifiers | Identifier | String | Identifier Value |

The event table stores a time series of events associated with a specific game instance and player. Event records contain an EventCode token and JSON payload along with the simulation tick in which they were generated. 

It is intended that the event processing components of the system will be extended with evaluators allowing integration with SUGAR and/or other achievement systems. The event stream could be used either via or independently from the achievement system to feed data to RAGE analytics assets such as the competency assessment and performance statistics components.
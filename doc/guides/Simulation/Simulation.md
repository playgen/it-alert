# Multiplayer Implementation

- Game client hosts slace instance of simulations (ECS) 
- Photon Loadbalancing Server hosts multiple, on demand instances of master simulation
- Server broadcasts instructs clients to tick and apply commands, latent clients can fast forward and interpolate
- Clients validate state against server checksum, fail and disconnect if out of sync.

![MultiplayerImplementation](../images/MultiplayerImplementation.png)

# Simulation Logic
The simulation project is found at *Simulation/Playgen.ItAlert.sln*.

This project contains shared logic between both Server and Client
## Project Structure
- **PlayGen.ItAlert.Simulation.Modules**
    - **PlayGen.ITAlert.Simulation.Modules.Antivirus**
        - Antivirus, capture and anaylsis system enhancement and tools 
    - **PlayGen.ITAlert.Simulation.Modules.GarbageDisposal** 
        - Garbage disposal system enhancement
    - **PlayGen.ITAlert.Simulation.Modules.Malware** 
        - Malware propagation and detection components
    - **PlayGen.ITAlert.Simulation.Modules.Resources**
        - Subsystem and connection resource components 
    - **PlayGen.ITAlert.Simulation.Modules.Transfer** 
        - Transfer system enhancements
    - **PlayGen.ITAlert.Simulation.Modules.Tutorial** 
        - Tutorial system components
- **PlayGen.ItAlert.Simulation**
- **PlayGen.ItAlert.Simulation.Tests**
    - Base project containing config files and object definitions
- **PlayGen.ItAlert.Simulation.Archetypes**
    - Base archetypes to be extended in PlayGen.ITAlert.Simulation.Modules
- **PlayGen.ItAlert.Simulation.Common**
- **PlayGen.ItAlert.Simulation.Common.Tests**
    - Common identifiers used
- **PlayGen.ItAlert.Simulation.Configuration**
- **PlayGen.ItAlert.Simulation.Configuration.Tests**
    - Scenario Definitions in Scenarios/ and Configuration loader
    - Definitions of available player colours 
- **PlayGen.ItAlert.Simulation.Logging**
    - Handles logging of events to database for query later
- **PlayGen.ItAlert.Simulation.Scenario**
- **PlayGen.ItAlert.Simulation.Scenario.Tests**
    - Creation of scenarios loaded from PlayGen.ITAlert.Simulation.Configuration
- **PlayGen.ItAlert.Simulation.Scenario.Localization**
    - Localization for retrieval of strings for a given key and langugae
- **PlayGen.ItAlert.Simulation.Scenario.Serialization**
    - DOES NOT SEEM TO BE USED
- **PlayGen.ItAlert.Simulation.Scoring**
    - Scoring system for tracking player scores
    - Event Handlers that handle scorable events
- **PlayGen.ItAlert.Simulation.Serialization**
- **PlayGen.ItAlert.Simulation.Serialization.Tests**
    - DOES NOT SEEM TO BE USED
- **PlayGen.ItAlert.Simulation.Startup**
    - Handles setup of simulations life cycle
- **PlayGen.ItAlert.Simulation.UI**
    - todo


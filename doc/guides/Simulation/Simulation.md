# Simulation Logic
The simulation project is found at *Simulation/Playgen.ItAlert.sln*.
## Project Structure
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
    - tofo
- **PlayGen.ItAlert.Simulation.Scoring**
    - Scoring system for tracking player scores
    - Event Handlers that handle scorable events
- **PlayGen.ItAlert.Simulation.Serialization**
- **PlayGen.ItAlert.Simulation.Serialization.Tests**
    - unused
- **PlayGen.ItAlert.Simulation.Startup**
    - Handles setup of simulations life cycle
- **PlayGen.ItAlert.Simulation.UI**
    - todo


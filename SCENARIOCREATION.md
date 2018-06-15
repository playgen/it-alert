# Creating Scenarios
A scenario is the definition for a playable ‘level’ in the game and contains configuration for the IT Alert engine, currently a scenario must be defined in code.

## Archetypes
Archetypes are the templates used to generate classes of entity in the game. Every object in the game is represented by an entity and the specific behaviour is implemented through a collection of components associated with that entity. Archetypes can extend other archetype definitions and so inherit and override component configuration from their parent allowing specialized types to be defined.

The game provides a number of default archetypes and the scenario allows the developer to extend this list with customised entities according to their need.

Currently Implemented archetypes are defined in the following table:

Archetype | Description | Parameters of Interest
--- | --- | ---
System | Respresents a system in the network | Item storage<br>CPU/Memory/Network Capacity
Antivirus Workstation (system) | Specialized system providing antivirus factory behaviour | 
Connection | Represents a network connection | Movement Cost
Actor | The base type for entities with agency in the game | 
Player (Actor) | Represents a player in the game, typically human but can be automated | Name<br>Colour<br>Logical Id
Virus (Actor) | The standard enemy actor in the game | Genome
Item | The base type for items that can be stored, carried and used on the network | Location<br>Owner<br>Activation State<br>Activation Time
Scanner (Item) | The scanner reveals udetected viruses when activated on an infected system | 
Antivirus (Item) | The antibirus neutralises viruses with the corresponding genome when activated on an infected system | Target genome
Capture (Item) | The capture gathers a sample of a virus on an infected system. *The sample gathered will be the oldest virus present on the system* | Captured Genome

## Viruses
Viruses represent autonomous or untargeted threats to the system integrity. Viruses spawn invisible to the players and their effects must be observed to deduce their presence. Players must analyse affected systems to reveal the virus and to permit restorative action to be taken.

The virus genome is expressed in terms of colours. The primary genes available are Red, Green and Blue. Genes can be combined to form more complex threats using a combination of colours, eg. Red/Green (yellow)

The effects of the virus are mapped onto specific genes in the scenario configuration. The available effects are currently:

- **Consume system memory**: reducing item storage capacity of system.
- **Consume processor cycles**: increasing the time required to perform actions and move within a system.
- **Generate network traffic**: slowing movement between nodes.

## Items
The player’s primary method of interaction with the game is via items. Items can be stored on certain locations and may be picked-up and carried by the players between those locations. Most items can be activated on locations to provide the player with specific abilities, items may activate for a specified time and may also be consumed after a single or multiple activation(s).

## Networks
A network is a series of systems that have a series of connections between them, each system in a network is defined as follows:
Property | Description
--- | ---
Coordinate | X, Y position in the network, from bottom-left
Name | Text label for the system
Type | 'Default' or one of the available specialized types available, currently: 'Antivirus Workstation'
Logical Id | A unique numerical identifier for the system. *This can be inferred where all adjacent systems are connected with bi-directional edges.*

### Specialized Systems
There are currently 3 specialized workstations in IT Alert! Antivirus, Garbage Disposal and Transfer.
Enhancement | Description
--- | ---
Antivirus Workstation | The antivirus workstation allows players to create Antivirus from captured samples<br> - The Sample must be dropped on the microscope location and then the double helix item must be activated to perform the analysis<br> - When complete the sample will be consumed from the capture too and an antivirus on the corresponding colour will be produced in the syringe location
Garbage Disposal | The garbage disposal allows players to destroy items for the purpose of creating space on the network<br> - Items to be destroyed must be placed on the waste bin location and then the recycle item must be activated to destroy them 
Transfer Station | The transfer station must exist as a pair, in two separate locations. The transfer station allows items to be teleported across the network<br> - The item(s) to transfer must be placed on the transfer location and the transfer item must be activated<br> - When activation has completed, items will be swapped between the two transfer station locations

### System Capacity
The resources on each system are specified by its capacities:
- **Storage**: The number of item storage locations available on the system, these may be reduced by enhancements (default = 4)
- **Memory**: This modulates the number of enabled item storage slots in the standard games (default = 5)
- **CPU**: This modulates the item activation and movement speed on the system (default = 4)
- **Network**: This modulates the movement speed on adjacent connections (default = 2*numberOfConnection)

### Connections
Connections are unidirectional and can be automatically generated for simple networks where bi-directional movement is required and movement cost constant. Connections have the following properties:
Property | Description
--- | ---
Head Id | Logical Id of the head (entry) system
Tail Id | Logical Id of the tail (exit) system
Movement Cost | Synonumous with bandwidth, regulates the speed of movement along the connection (default = 5)

# Adding Scenarios
The scenario loading currently requires the scenarios to be hardcoded into the Client and Server assemblies. It is intended that the scenario definitions will become serializable and read at runtime from a directory on the server and transferred to the clients upon selection. Further development will enable parameterization of scenario variables and exposure of these settings to the developer via a client developer UI. 

Scenario definition files are located in the Simulation solution at the following path:

**Namespace**:	PlayGen.ITAlert.Simulation.Configuration.Scenarios

**Path**:		\Simulation\PlayGen.ITAlert.Simulation.Configuration\Scenarios

Scenarios are constructed via a factory pattern that creates a new instance of the scenario object on demand. The scenarios available to play are included via the temporary implementation in the ScenarioLoader class.

# Example Scenario
Before thinking about the implementation of the conditional logic and sequence of actions required to implement a scenario an abstract should be prepared detailing the required objectives for that scenario in terms of the player interaction with the game and/or other players and defining completion criteria.

Tutorial 1 is defined as follows:
- **Abstract**: Introduce the player to the fundamental mechanics, provide a task that requires them to demonstrate their comprehension.
- **Players**: 1
- **Network**: 2x1
- **Steps**
    - How to move
    - How to activate items
    - How to pick up items
    - How to reveal viruses
    - How to neutralise viruses
- **Success**: all steps completed, virus neutralised
- **Failure**: N/A

The final setup for the tutorial can be seen in [Tutorial1_Movement](Simulation/PlayGen.ITAlert.Simulation.Configuration/Scenarios/Tutorial/Tutorial1_Movement.cs)

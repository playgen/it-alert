# Simulation Logic

# Game Scenarios 
Creating game scenarios for IT Alert! requires a full understanding of game systems, please see the guide for [Creating Scenarios Guide](ScenarioCreation.md) for full details on creating and adding scenarios to the game.

# Sequencing
The sequence defines a number of scripted frames which are progressed through linearly as the player(s) interact with the game. It is intended that the engine will support nonlinear, branching and/or cyclical sequences but this is currently not implemented.

Each frame defines three configuration elements:

- **OnEnter Actions**: A variable length list of operations that will be executed when the sequence enters the current frame
- **Evaluation Criteria**: An operation that can evaluate the current state of the game and wthiill return true when satisfied.
- **OnExit Actions**: A variable length list of operations that will be executed when the sequence exits the current frame

The sequence will progress through frames in the order they are defined and will not proceed to the next frame until the evaluation criteria is satisfied. 

## Actions
All manipulation of the game state by the players or sequence are implemented via commands. 

### Player Commands
Player commands can be executed by the sequence on behalf of the player however this should be discouraged unless used to simulate AI player behaviour.

Currently implemented player commands are:

Command | Description | Parameters
--- | --- | ---
Move | Instructs the game to automatically route the player to the destination system. | Player<br>Destination
Activate | Instructs the game to attempt to activate the specified item, if:<br>- It is located in the same system as the player<br>- It is not currently active | Player<br>Item
Pickup | Instructs the game to attempt to move the specified item from the system to the player’s inventory, if:<br> - The player is located on a system<br> - The item is located in the same system as the player<br> - The item is not currently active<br> -The player has no item currently in their inventory | Player<br>Item
Drop | Instructs the game to attempt to move the  specified item from the player’s inventory to the current system, if:<br> The player has an item in their inventory, <br> - The player is located on a system<br> - The system has empty, enabled storage locations | Player<br>Item

### System Commands
For the purpose of sequencing the following system commands have been implemented although the list is not exhaustive and can be added to on request:

Command | Description | Parameters 
--- | --- | ---
ShowText | Displays an instructional text box to all players, with optional press to continue button. | Text<br>Recommended not to exceed 4 lines of 60 characters each.<br>EnableButton: Should the continue button be shown.
HideText | Hides previously displayed text boxes. |
Enable/Disable Player Command | Enable/Disable processing of specific player command type. *This is the only means by which you can disable player movement or item interaction.* | CommandType: See Player Commands<br> Enable (True/False)
CreateItemOnSystem | Spawn a new item on the specified subsystem | System Id<br>Item Type
CreateItemOnPlayer | Spawn a new item in the specified player’s inventory. | Player Id<br>Item Type
CreateNPCOnSystem | Spawn an NPC (Virus) on the specified system. | System Id<br>NPC Type
EndGame | Ends the game in a success or failure state. *This implementation is intended to be temporary until scenario scoped evaluators are implemented.* | ExitCode: (Success/Failure)

## Evaluators
Evaluators are functions that evaluate the state of the game and return true when satisfied. Evaluators can be chained and therefore combined to form more complex expressions. 

The following evaluators are currently implemented:

Evaluator | Description | Parameters
--- | --- |---
AND | Performs a logical AND operation on the left and right operands. | Left: Evaluator<br>Right: Evaluator
OR | Performs a logical OR operation on the left and right operands. | Left: Evaluator<br>Right: Evaluator
NOT | Performs a logical NOT operation on the operand. | Right: Evaluator
WaitForContinue | Satisfied when the continue flag has been set by any player | See Tutorial System
WaitForContinueAll | Satisfied when the continue flag has been set by all players. | See Tutorial System
WaitForSeconds | Satisfied when the specified number of seconds have elapsed since entering the current frame. | Seconds (default = 3)
PlayerIsAtLocation | Satisfied when the specified player is at the specified location. | Player Id<br>System or Connection Id
PlayersAtLocation | Satisfied when the specified number of players are at the specified location. | Player Count<br>System or Connection Id
SystemIsInfected | Satisfied when the specified system has at least one virus present. Optionally the genome can be tested. | System Id<br>Optional: Genome
SystemsAreInfected | Satisfied when any of the systems have at least one virus present. |
ItemIsAtLocation | Satisfied when the specified item or any instance of the specified item type is at the specified location. | Item Id  OR  Item Type<br>System Id
ItemIsInPlayerInventory | Satisfied when the specified item or any instance of the specified item type is in the player’s inventory. | Item Id  OR  Item Type<br>Player Id

Custom evaluators can be implemented to perform more complex evaluation operations and implementation of the standard evaluators is ongoing and will grow over time.

## Tutorial System
The tutorial system provides an index of continue flags associated with the players and the flags can be interacted with via various behaviours, the most obvious of these is the player clicking on a ‘Continue’ button on the instructional text box. The continue behaviour can be applied to other entity behaviours where a bespoke evaluator is not required.

# Example Scenario
For an example of a scenario, please see [example](ScenarioCreation.md#example-scenario)
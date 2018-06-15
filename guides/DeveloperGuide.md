# Project Architecture
![Project Architecture](http://www.plantuml.com/plantuml/img/XL7B3e903Bpp5Vo0Ve4NOWWnCKf4OyA5eAbQXLsp58r_BoxowDh40w4pizFTJlNX2aVoL_0ahIfz8a-tJ0hgy-zWSidDFSNx4WyT5SwJhbniWm8CJHCiwo8drCfLuekcWMrH5XVr6HrohM839LQGW7c0uU9E8RrMG4zjRgZExYLMxb-yYu8D0ndJexo4VTF7nRpB9eZ2B51vyI8yLfut1ENF9XERmJUf7e-YwhdcytLJ2q7zKjRb16S7i0BFZY79AAOgDFhpZ3OKtbiu8Av9E3UCCzxCg1OytOAiv6oBmtzcFZbViU1z0m00)

The project is primarily composed of three solutions, shared game logic, server and client code

## Shared Game Logic
Common logic that is shared between to server and client

The game uses a client/server architecture based around Photon Networking. Photon uses a plugin architecture where each game instance maps to a logical ‘room’. For each room an instance of the plugin is instantiated within the load balancing host. Messages are transferred between client and server over TCP or UDP transports, optionally using WebSockets for proxy traversal.
- [OnPremise](https://www.photonengine.com/en/OnPremise)
- [PUN](https://www.photonengine.com/en/PUN)

### Location
- **Simulation/PlayGen.ItAlert.sln**

## Server Side Code
The server code was based on the OnPremise server Loadbalancing example which has been structurally altered to suit the needs of the project. Upgrading the photon dependencies, to for example deploy to the Photon cloud, would involve mapping the photon code onto the altered directory structure and upgrading the required binary references within the Photon-Plugin and PlayGen.ITAlert.Photon solution.

### Location
- **Server/PlayGen.ITAlert.Photon.sln**

## Client Side Code
The client integration has modified the structure of the PUN unity package deployment to suit the DLL based workflow required to share assemblies between unity and non-unity projects. 

### Location
- **Unity/PlayGen.ITAlert.Unity/**: *precompiled game Logic, builds to Assets/PlayGen.ITAlert.Unity*

## Unity Project
The unity project controls the UI elements in game and uses the compiled Client Side Code

### Location
- **Unity/**: *unity project directory, open folder in Unity*
    - **Assets/**: *unity project assets*
        - **Scenes/IT Alert.unity**: *the main scene for the project*
    - **PlayGen.ITAlert.Unity/**: *precompiled game Logic, builds to Assets/PlayGen.ITAlert.Unity*
    - **PlayGen.ITAlert.Installer**: *[WiX](http://wixtoolset.org/) installer project*
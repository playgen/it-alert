# Server Logic
The Photon project is found at *Server/PlayGen.ITAlert.Photon.sln*.

This project is based on the [Photon Loadbalancing server](https://www.photonengine.com/en/OnPremise).

## Visual Studio Project Structure
**Plugin**: *Server specific.*
- **PlayGen.Photon.Plugin**: *Photon server project.*
- **PlayGen.Photon.Analytics**: *Analytics logging for match status and ranking data for each client.*
- **PlayGen.Photon.Messaging**: *Handles listeners/subscribers for photon messages.*
- **PlayGen.Photon.SUGAR**: *SUGAR Integration.*
- **PlayGen.ITAlert.Photon.Plugin**: *IT Alert specific extension of the Photon server project (PlayGen.Photon.SUGAR).*
  - **RoomStates**: *Various states of the IT Alert game rooms.*
  - **ITAlertPluginFactory.cs**: *The entry point for the ITAlert game room logic.*
  - **ITAlertRoomStateControllerFactory**: *Initializes the root game room state controller.*

**Shared**: *Used by both the Server and the Client.*
- **PlayGen.Photon.Common**
- **PlayGen.Photon.Players**: *Photon player objects and manager.*
- **PlayGen.Photon.Messages**: *Commands applicable for the Photon context.*
- **PlayGen.ITAlert.Photon.Common**: *Metadata for Custom game rooms.*
- **PlayGen.ITAlert.Photon.Messages**: *Commands applicable for the IT Alert context.*
- **PlayGen.ITAlert.Photon.Players**: *IT Alert specific Photon player objects and manager.*
- **PlayGen.ITAlert.Photon.Serialization**: *Handles serialization and deserialization of messages sent between the server and client.*

# Run
Running the server as a standalone application:

## Process
1. Build CounterPublisher.
2. Build LoadBalancing (3 times to resolve all .dll dependencies - this may be overkill though).
3. Run \deploy\bin_Win64\_run-Photon-as-application.start.cmd to start the server.

### Notes:
- Run \deploy\bin_Win64\_run-Photon-as-application.stop.cmd to stop the server.
- Logs located in \deploy\bin_Win64\log.
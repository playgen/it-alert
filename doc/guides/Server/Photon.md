# Server Logic
The Photon project is found at *Server/PlayGen.ITAlert.Photon.sln*.

**TODO** update

## Project Structure
**Plugin**
- **PlayGen.ITAlert.Photon.Plugin**
    - ITAlert.Photon plugin for ITAlert! to allow for communication between game code and photon
- **PlayGen.Photon.Analytics**
    - Analytics logging for match status and ranking data for each client
- **PlayGen.Photon.Messaging**
    - Handles listeners/subscribers to photon message 
- **PlayGen.Photon.Plugin**
    - Photon plugin for communication between game code and photon
- **PlayGen.Photon.SUGAR**
    - SUGAR Configuration

**Shared**
- **PlayGen.Photon.Common**
- **PlayGen.ITAlert.Photon.Common**
    - Metadata for Custom game rooms
- **PlayGen.ITAlert.Photon.Messages**
    - Commands that players can send to the server
- **PlayGen.ITAlert.Photon.Players**
    - Player states for synchronizing connected players across clients, including player glyphs, colours and client state
- **PlayGen.ITAlert.Photon.Serialization**
    - Handles serialization and deserialization of messages sent to/from server by clients
- **PlayGen.Photon.Messages**
    - Photon Messaging for logging from server to client
- **PlayGen.Photon.Players**
    - Player management through photon, creating new players and updating their state.

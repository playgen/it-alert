namespace PlayGen.ITAlert.Photon.Events
{
    /// <summary>
    /// Range 101 - 200
    /// </summary>
    public enum ServerEventCode : byte
    {
        // System
        Message = 101,

        // Players
        PlayerList = 111,

        // Game
        GameEntered = 121,

        GameInitialized = 122,

        GameTick = 123,

        GameFinalized = 124,

        // Lobby
        LobbyRentered = 131
    }
}
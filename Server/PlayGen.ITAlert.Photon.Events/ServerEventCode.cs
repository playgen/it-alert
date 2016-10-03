namespace PlayGen.ITAlert.Photon.Events
{
    /// <summary>
    /// Range 101 - 200
    /// </summary>
    public enum ServerEventCode : byte
    {
        // Players
        PlayerList = 101,

        // Game
        GameEntered = 111,

        GameInitialized = 112,

        GameTick = 113,

        GameFinalized = 114,

        // Lobby
        LobbyRentered = 121
    }
}
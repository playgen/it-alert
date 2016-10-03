namespace PlayGen.ITAlert.Photon.Events
{
    /// <summary>
    /// Range 1 - 100
    /// </summary>
    public enum PlayerEventCode : byte
    {
        // Room
        ChangeName = 1,

        ChangeColor = 2,

        SetReady = 3,

        SetNotReady = 4,

        ListPlayers = 5,

        StartGame = 6,

        // Game
        GameInitialized = 11,

        GameCommand = 12,

        GameFinalized = 13,
    }
}
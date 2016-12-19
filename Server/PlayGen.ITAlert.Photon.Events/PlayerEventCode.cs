namespace PlayGen.ITAlert.Photon.Events
{
    /// <summary>
    /// Range 1 - 100
    /// </summary>
    public enum PlayerEventCode : byte
    {
        // Room
        ChangeExternalId = 1,

        ChangeName = 2,

        ChangeColor = 3,

        SetReady = 4,

        SetNotReady = 5,

        ListPlayers = 6,

        StartGame = 7,

        // Game
        GameInitialized = 11,

        GameCommand = 12,

        GameFinalized = 13,
    }
}
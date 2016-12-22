namespace PlayGen.ITAlert.Photon.Events
{
    /// <summary>
    /// Range 1 - 100
    /// </summary>
    public enum ClientEventCode : byte
    {
        // System
        Message = 1,

        // Room
        ChangeExternalId = 11,

        ChangeName = 12,

        ChangeColor = 13,

        SetReady = 14,

        SetNotReady = 15,

        ListPlayers = 16,

        StartGame = 17,

        // Game
        GameInitialized = 21,

        GameCommand = 22,

        GameFinalized = 23,
    }
}
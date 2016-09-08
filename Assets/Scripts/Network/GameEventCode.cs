namespace PlayGen.ITAlert.Network
{
    internal enum GameEventCode : byte
    {
        PlayerReady = 1,

        PlayerStartGame = 2,

        ListReadyPlayers = 11,

        SimulationInitialized = 50,

        SimulationTick = 51,

        SimulationFinalized = 52,

        SimulationCommand = 53,
    }
}
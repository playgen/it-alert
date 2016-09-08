namespace PlayGen.ITAlert.Network
{
    internal enum GameEventCode : byte
    {
        PlayerReady = 1,

        PlayerStartGame = 2,

        ListReadyPlayers = 11,

        SimulationInitialized = 50,

        SimulationDump = 51,

        SimulationDelta = 52,

        SimulationFinalized = 53,

        SimulationCommand = 54,
    }
}
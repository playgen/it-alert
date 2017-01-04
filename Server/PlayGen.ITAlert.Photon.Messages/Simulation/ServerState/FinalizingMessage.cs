namespace PlayGen.ITAlert.Photon.Messages.Simulation.ServerState
{
    public class FinalizingMessage : ServerMessage
    {
        public byte[] SerializedSimulation { get; set; }
    }
}

namespace PlayGen.ITAlert.Photon.Messages.Simulation.ServerState
{
    public class TickMessage : ServerMessage
    {
        public byte[] SerializedSimulation { get; set; }
    }
}

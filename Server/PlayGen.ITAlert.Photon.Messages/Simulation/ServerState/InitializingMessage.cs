namespace PlayGen.ITAlert.Photon.Messages.Simulation.ServerState
{
	public class InitializingMessage : ServerMessage
	{
		public byte[] SerializedSimulation { get; set; }
	}
}

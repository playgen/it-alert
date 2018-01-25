namespace PlayGen.ITAlert.Photon.Messages.Simulation.States
{
	public class TickMessage : SimulationMessage
	{
		public string EntityState { get; set; }

		public string TickString { get; set; }

		public uint CRC { get; set; }
	}
}

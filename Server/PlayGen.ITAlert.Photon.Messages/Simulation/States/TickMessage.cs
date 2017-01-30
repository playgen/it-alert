using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation.States
{
	public class TickMessage : SimulationMessage
	{
		public string EntityState { get; set; }

	}
}

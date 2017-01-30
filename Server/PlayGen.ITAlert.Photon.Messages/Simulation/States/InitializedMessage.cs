using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation.States
{
	public class InitializedMessage : SimulationMessage
	{
		public string SimulationState { get; set; }

		public string SimulationConfiguration { get; set; }
	}
}

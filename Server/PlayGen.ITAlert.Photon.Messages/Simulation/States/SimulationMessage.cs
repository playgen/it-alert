using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation.States
{
	public class SimulationMessage : Message
	{
		public override int Channel => (int)ITAlertChannel.SimulationState;

	}
}

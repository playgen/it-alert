using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation.States
{
	public class StateMessage : Message
	{
		public override int Channel => Messages.Channel.SimulationState.IntValue();

	}
}

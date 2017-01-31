using Engine.Commands;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation.Commands
{
	public class CommandMessage : Message
	{
		public override int Channel => (int)ITAlertChannel.SimulationCommand;

		public ICommand Command { get; set; }
	}
}
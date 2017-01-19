using PlayGen.ITAlert.Simulation.Commands.Interfaces;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation.Commands
{
	public class CommandMessage : Message
	{
		public override int Channel => Messages.Channel.SimulationCommand.IntValue();

		public ICommand Command { get; set; }
	}
}
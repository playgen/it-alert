using PlayGen.ITAlert.Simulation.Commands.Interfaces;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation
{
    public class CommandMessage : Message
    {
        public override int Channel => (int)Channels.SimulationCommands;

        public ICommand Command { get; set; }
    }
}
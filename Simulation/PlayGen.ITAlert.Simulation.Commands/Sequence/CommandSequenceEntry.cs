using PlayGen.ITAlert.Simulation.Commands.Interfaces;

namespace PlayGen.ITAlert.Simulation.Commands.Sequence
{
    public class CommandSequenceEntry
    {
        public int Tick { get; set; }

        public ICommand[] Commands { get; set; }
    }
}

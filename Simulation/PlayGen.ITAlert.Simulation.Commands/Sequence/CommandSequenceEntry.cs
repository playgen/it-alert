using Engine.Commands;

namespace PlayGen.ITAlert.Simulation.Commands.Sequence
{
    public class CommandSequenceEntry
    {
        public int Tick { get; set; }

        public ICommand[] Commands { get; set; }
    }
}

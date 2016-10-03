using PlayGen.ITAlert.Simulation.Commands.Interfaces;

namespace PlayGen.ITAlert.Simulation.Commands
{
    public class SpawnVirusCommand : ICommand
    {
        public int SubsystemLogicalId { get; set; }
    }
}

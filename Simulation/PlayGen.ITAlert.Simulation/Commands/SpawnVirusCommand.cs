using Engine.Commands;

namespace PlayGen.ITAlert.Simulation.Commands
{
    public class SpawnVirusCommand : ICommand
    {
        public int SystemLogicalId { get; set; }
    }
}

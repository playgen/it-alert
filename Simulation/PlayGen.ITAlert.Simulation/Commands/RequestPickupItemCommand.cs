using Engine.Commands;

namespace PlayGen.ITAlert.Simulation.Commands
{
    public class RequestPickupItemCommand : ICommand
    {
        public int PlayerId { get; set; }

        public int ItemId { get; set; }

        public int LocationId { get; set; }
    }
}

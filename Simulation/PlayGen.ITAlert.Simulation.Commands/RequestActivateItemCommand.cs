using Engine.Commands;

namespace PlayGen.ITAlert.Simulation.Commands
{
    public class RequestActivateItemCommand : ICommand
    {
        public int PlayerId { get; set; }

        public int ItemId { get; set; }
    }
}

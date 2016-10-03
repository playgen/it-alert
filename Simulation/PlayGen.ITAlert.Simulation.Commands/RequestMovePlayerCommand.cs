using PlayGen.ITAlert.Simulation.Commands.Interfaces;

namespace PlayGen.ITAlert.Simulation.Commands
{
    public class RequestMovePlayerCommand : ICommand
    {
        public int PlayerId { get; set; }

        public int DestinationId { get; set; }
    }
}

using PlayGen.ITAlert.Simulation.Commands.Interfaces;
using System.Collections.Generic;

namespace PlayGen.ITAlert.Simulation.Commands
{
    public class CommandResolver
    {
        private readonly Simulation _simulation;

        public CommandResolver(Simulation simulation)
        {
            _simulation = simulation;
        }

        public void ProcessCommands(IEnumerable<ICommand> commands)
        {
            if (commands == null) return;

            foreach (var command in commands)
            {
                ProcessCommand(command);
            }
        }

        public void ProcessCommand(ICommand command)
        {
            if (command == null) return;

            var requestMovePlayerCommand = command as RequestMovePlayerCommand;
            if (requestMovePlayerCommand != null)
            {
                _simulation.RequestMovePlayer(requestMovePlayerCommand.PlayerId, requestMovePlayerCommand.DestinationId);
                return;
            }

            var requestActivateItemCommand = command as RequestActivateItemCommand;
            if (requestActivateItemCommand != null)
            {
                _simulation.RequestActivateItem(requestActivateItemCommand.PlayerId, requestActivateItemCommand.ItemId);
                return;
            }

            var requestDropItemCommand = command as RequestDropItemCommand;
            if (requestDropItemCommand != null)
            {
                _simulation.RequestDropItem(requestDropItemCommand.PlayerId, requestDropItemCommand.ItemId);
                return;
            }

            var requestPickupItemCommand = command as RequestPickupItemCommand;
            if (requestPickupItemCommand != null)
            {
                _simulation.RequestPickupItem(requestPickupItemCommand.PlayerId, requestPickupItemCommand.ItemId, requestPickupItemCommand.LocationId);
                return;
            }

            var spawnVirusCommand = command as SpawnVirusCommand;
            if (spawnVirusCommand != null)
            {
                _simulation.SpawnVirus(spawnVirusCommand.SubsystemLogicalId);
                return;
            }
        }
    }
}

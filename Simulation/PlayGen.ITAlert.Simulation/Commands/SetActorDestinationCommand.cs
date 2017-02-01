using Engine.Commands;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Flags;
using PlayGen.ITAlert.Simulation.Components.Intents;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class SetActorDestinationCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int DestinationId { get; set; }
	}

	public class SetActorDestinationCommandHandler : CommandHandler<SetActorDestinationCommand>
	{
		private readonly IEntityRegistry _entityRegistry;

		private readonly ComponentMatcher _playerComponentMatcher;
		private readonly ComponentMatcher _subsystemMatcher;

		public SetActorDestinationCommandHandler(IComponentRegistry componentRegistry, IEntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;

			_playerComponentMatcher = componentRegistry.CreateMatcher(new[] {typeof(Player), typeof(Intents)});
			_subsystemMatcher = componentRegistry.CreateMatcher(new[] {typeof(Subsystem)});
		}

		protected override bool TryProcessCommand(SetActorDestinationCommand command)
		{
			Entity playerEntity;
			Entity destinationEntity;
			Intents playerIntents;
			if (_entityRegistry.TryGetEntityById(command.PlayerId, out playerEntity) 
				&& _playerComponentMatcher.IsMatch(playerEntity)
				&& _entityRegistry.TryGetEntityById(command.DestinationId, out destinationEntity)
				&& _subsystemMatcher.IsMatch(destinationEntity)
				&& playerEntity.TryGetComponent(out playerIntents))
			{
				playerIntents.Replace(new MoveIntent(destinationEntity.Id));
				return true;
			}
			return false;
		}
	}
}

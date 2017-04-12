using System.Collections.Generic;
using Engine.Commands;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Systems.Players;

namespace PlayGen.ITAlert.Simulation.Commands.Movement
{
	[Deduplicate(DeduplicationPolicy.Replace)]
	public class SetActorDestinationCommand : ICommand
	{
		public int? PlayerId { get; set; }

		public int? PlayerEntityId { get; set; }

		public int DestinationEntityId { get; set; }
	}

	public class SetActorDestinationCommandHandler : CommandHandler<SetActorDestinationCommand>
	{
		private readonly IEntityRegistry _entityRegistry;

		private readonly ComponentMatcherGroup<Player, Destination> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem> _subsystemMatcherGroup;

		private readonly PlayerSystem _playerSystem;

		#region Overrides of CommandHandler<SetActorDestinationCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new SetActorDestinationCommandEqualityComparer();

		#endregion

		public SetActorDestinationCommandHandler(IMatcherProvider matcherProvider, 
			IEntityRegistry entityRegistry,
			PlayerSystem playerSystem)
		{
			_entityRegistry = entityRegistry;

			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, Destination>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem>();
			_playerSystem = playerSystem;
		}

		protected override bool TryProcessCommand(SetActorDestinationCommand command)
		{
			int playerEntityId = -1;

			if (command.PlayerEntityId.HasValue == false
				&& (command.PlayerId.HasValue == false
					|| _playerSystem.TryGetPlayerEntityId(command.PlayerId.Value, out playerEntityId) == false))
			{
				return false;
			}

			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerEntityId ?? playerEntityId, out var playerTuple)
				&& _subsystemMatcherGroup.TryGetMatchingEntity(command.DestinationEntityId, out var subsystemTuple))
			{
				playerTuple.Component2.Value = command.DestinationEntityId;
				return true;
			}
			return false;
		}
	}

	public class SetActorDestinationCommandEqualityComparer : CommandEqualityComparer<SetActorDestinationCommand>
	{
		#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

		protected override bool IsDuplicate(SetActorDestinationCommand x, SetActorDestinationCommand other)
		{
			// player can only ever have one destination
			return x.PlayerId == other.PlayerId;
		}

		#endregion
	}
}

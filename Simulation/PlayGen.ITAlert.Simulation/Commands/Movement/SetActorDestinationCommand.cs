using System.Collections.Generic;
using Engine.Commands;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Intents;

namespace PlayGen.ITAlert.Simulation.Commands.Movement
{
	[Deduplicate(DeduplicationPolicy.Replace)]
	public class SetActorDestinationCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int DestinationEntityId { get; set; }
	}

	public class SetActorDestinationCommandHandler : CommandHandler<SetActorDestinationCommand>
	{
		private readonly IEntityRegistry _entityRegistry;

		private readonly ComponentMatcherGroup<Player, Intents> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem> _subsystemMatcherGroup;

		#region Overrides of CommandHandler<SetActorDestinationCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new SetActorDestinationCommandEqualityComparer();

		#endregion

		public SetActorDestinationCommandHandler(IMatcherProvider matcherProvider, IEntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;

			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, Intents>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem>();
		}

		protected override bool TryProcessCommand(SetActorDestinationCommand command)
		{
			ComponentEntityTuple<Player, Intents> playerTuple;
			ComponentEntityTuple<Subsystem> subsystemTuple;
			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out playerTuple)
				&& _subsystemMatcherGroup.TryGetMatchingEntity(command.DestinationEntityId, out subsystemTuple))
			{
				playerTuple.Component2.Replace(new MoveIntent(command.DestinationEntityId));
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

using Engine.Commands;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Intents;

namespace PlayGen.ITAlert.Simulation.Commands.Movement
{
	public class SetActorDestinationCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int DestinationId { get; set; }

		#region Equality members

		protected bool Equals(SetActorDestinationCommand other)
		{
			return PlayerId == other.PlayerId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SetActorDestinationCommand) obj);
		}

		public override int GetHashCode()
		{
			return PlayerId.GetHashCode();
		}

		#region Implementation of IEquatable<ICommand>

		public bool Equals(ICommand other)
		{
			return Equals(other as SetActorDestinationCommand);
		}

		#endregion

		#endregion
	}

	public class SetActorDestinationCommandHandler : CommandHandler<SetActorDestinationCommand>
	{
		private readonly IEntityRegistry _entityRegistry;

		private readonly ComponentMatcherGroup<Player, Intents> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem> _subsystemMatcherGroup;

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
				&& _subsystemMatcherGroup.TryGetMatchingEntity(command.DestinationId, out subsystemTuple))
			{
				playerTuple.Component2.Replace(new MoveIntent(command.DestinationId));
				return true;
			}
			return false;
		}
	}
}

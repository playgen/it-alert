using System.Collections.Generic;
using Engine.Commands;
using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Events;
using PlayGen.ITAlert.Simulation.Systems.Players;

namespace PlayGen.ITAlert.Simulation.Commands
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
		private readonly ComponentMatcherGroup<Player, Destination> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem> _subsystemMatcherGroup;

		private readonly PlayerSystem _playerSystem;
		private readonly EventSystem _eventSystem;

		#region Overrides of CommandHandler<SetActorDestinationCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new SetActorDestinationCommandEqualityComparer();

		#endregion

		public SetActorDestinationCommandHandler(IMatcherProvider matcherProvider, 
			PlayerSystem playerSystem,
			EventSystem eventSystem)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, Destination>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem>();
			_playerSystem = playerSystem;
			_eventSystem = eventSystem;
		}

		protected override bool TryHandleCommand(SetActorDestinationCommand command, int currentTick, bool handlerEnabled)
		{
			// TODO: this logic sucks hard.
			var playerEntityId = -1;
			if (command.PlayerEntityId.HasValue)
			{
				playerEntityId = command.PlayerEntityId.Value;
			}
			else if (command.PlayerId.HasValue)
			{
				_playerSystem.TryGetPlayerEntityId(command.PlayerId.Value, out playerEntityId);
			}

			var @event = new SetActorDestinationEvent() {
				PlayerEntityId = command.PlayerEntityId ?? playerEntityId,
				DestinationEntityId = command.DestinationEntityId,
			};

			if (playerEntityId >= 0
				&& handlerEnabled
				&& _playerMatcherGroup.TryGetMatchingEntity(command.PlayerEntityId ?? playerEntityId, out var playerTuple)
				&& _subsystemMatcherGroup.TryGetMatchingEntity(command.DestinationEntityId, out var subsystemTuple))
			{
				playerTuple.Component2.Value = command.DestinationEntityId;

				@event.Result = SetActorDestinationEvent.CommandResult.Success;
				_eventSystem.Publish(@event);

				return true;
			}
			if (handlerEnabled == false)
			{
				@event.Result = SetActorDestinationEvent.CommandResult.Failure_CommandDisabled;
			}

			_eventSystem.Publish(@event);
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

	public class SetActorDestinationEvent : PlayerEvent
	{
		public enum CommandResult
		{
			Error = 0,
			Success,
			Failure_CommandDisabled,
		}

		public CommandResult Result { get; set; }

		public int DestinationEntityId { get; set; }
	}
}

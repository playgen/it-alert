using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Engine.Commands;
using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	[Deduplicate(DeduplicationPolicy.Replace)]
	public class SwapSubsystemItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int FromItemId { get; set; }

		public int? ToItemId { get; set; }

		public int FromContainerIndex { get; set; }

		public int ToContainerIndex { get; set; }
	}

	public class SwapSubsystemItemCommandHandler : CommandHandler<SwapSubsystemItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, IItemType> _itemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		private readonly EventSystem _eventSystem;

		#region Overrides of CommandHandler<SwapSubsystemItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new SwapSubsystemItemCommandEqualityComparer();
		#endregion

		public SwapSubsystemItemCommandHandler(IMatcherProvider matcherProvider, EventSystem eventSystem)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, IItemType>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
		}

		protected override bool TryHandleCommand(SwapSubsystemItemCommand command, int currentTick, bool handlerEnabled)
		{
			ComponentEntityTuple<Item, Owner, CurrentLocation, IItemType> toItemTuple = null;

			if (handlerEnabled
				&& _playerMatcherGroup.TryGetMatchingEntity(command.PlayerId,
					out var playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.FromItemId,
					out var fromItemTuple)
				&& (!command.ToItemId.HasValue
					|| _itemMatcherGroup.TryGetMatchingEntity(command.ToItemId.Value,
						out toItemTuple))
				// Player must be on a subsystem
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value,
					out var subsystemTuple))
			{
				// No one must own either item
				if (fromItemTuple.Component2.Value == null && toItemTuple?.Component2.Value == null)
				{
					var toContainer = subsystemTuple.Component2.Items[command.ToContainerIndex];
					var fromContainer = subsystemTuple.Component2.Items[command.FromContainerIndex];

					// Items must still be in same locations as when the command was issued
					if (fromContainer.Item == command.FromItemId
						&& toContainer.Item == command.ToItemId)
					{
						// Containers must be able to accept items
						if (toContainer.CanContain(command.FromItemId)
							&& (!command.ToItemId.HasValue || fromContainer.CanContain(command.ToItemId.Value)))
						{
							toContainer.Item = command.FromItemId;
							fromContainer.Item = command.ToItemId;

							return true;
						}
					}
				}
			}

			return false;
		}
	}

	public class SwapSubsystemItemCommandEqualityComparer : CommandEqualityComparer<SwapSubsystemItemCommand>
	{
		#region Overrides of CommandEqualityComparer<SwapSubsystemItemCommand>

		protected override bool IsDuplicate(SwapSubsystemItemCommand a, SwapSubsystemItemCommand b)
		{
			return a.PlayerId == b.PlayerId
					&& (a.ToItemId == b.ToItemId
						|| a.ToItemId == b.FromItemId
						|| a.FromItemId == b.ToItemId
						|| a.FromItemId == b.FromItemId);
		}

		#endregion
	}
}

using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	[Deduplicate(DeduplicationPolicy.Discard)]
	public class DropItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }

		public int ContainerId { get; set; }
	}

	public class DropItemCommandHandler : CommandHandler<DropItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;

		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation> _itemMatcherGroup;

		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;


		#region Overrides of CommandHandler<DropItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new DropItemCommandEqualityComparer();

		#endregion

		public DropItemCommandHandler(IMatcherProvider matcherProvider)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
		}

		protected override bool TryProcessCommand(DropItemCommand command, int currentTick)
		{
			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out var playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out var itemTuple)
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value, out var subsystemTuple)
				&& itemTuple.Component2.Value == playerTuple.Entity.Id)
			{
				var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
				var target = subsystemTuple.Component2.Items[command.ContainerId];
				if (inventory != null 
					&& inventory.Item == itemTuple.Entity.Id
					&& target != null
					&& target.CanCapture(itemTuple.Entity.Id))
				{
					target.Item = itemTuple.Entity.Id;
					itemTuple.Component3.Value = subsystemTuple.Entity.Id;
					inventory.Item = null;
					itemTuple.Component2.Value = null;
					return true;
				}

			}
			return false;
		}

		public class DropItemCommandEqualityComparer : CommandEqualityComparer<DropItemCommand>
		{
			#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

			protected override bool IsDuplicate(DropItemCommand x, DropItemCommand other)
			{
				// player can only ever have one destination
				return x.PlayerId == other.PlayerId && x.ItemId == other.ItemId;
			}

			#endregion
		}
	}
}

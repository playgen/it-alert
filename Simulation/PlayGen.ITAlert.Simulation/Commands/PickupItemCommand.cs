using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class PickupItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }

		public int LocationId { get; set; }
	}

	public class PickupItemCommandHandler : CommandHandler<PickupItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage> _playerMatcherGroup;

		private readonly ComponentMatcherGroup<Item, Owner> _itemMatcherGroup;

		public PickupItemCommandHandler(IMatcherProvider matcherProvider)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner>();
		}

		protected override bool TryProcessCommand(PickupItemCommand command)
		{
			ComponentEntityTuple<Player, ItemStorage> playerTuple;
			ComponentEntityTuple<Item, Owner> itemTuple;
			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out itemTuple)
				&& itemTuple.Component2.Value.HasValue == false)
			{
				var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
				if (inventory != null && inventory.Item.HasValue == false)
				{
					inventory.Item = itemTuple.Entity.Id;
					itemTuple.Component2.Value = playerTuple.Entity.Id;
					return true;
				}

			}
			return false;
		}
	}
}

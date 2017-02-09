using System.Linq;
using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class DropItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }

		public int LocationId { get; set; }
	}

	public class DropItemCommandHandler : CommandHandler<DropItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;

		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation> _itemMatcherGroup;

		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		public DropItemCommandHandler(IMatcherProvider matcherProvider)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
		}

		protected override bool TryProcessCommand(DropItemCommand command)
		{
			ComponentEntityTuple<Player, ItemStorage, CurrentLocation> playerTuple;
			ComponentEntityTuple<Item, Owner, CurrentLocation> itemTuple;
			ComponentEntityTuple<Subsystem, ItemStorage> subsystemTuple;

			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out itemTuple)
				&& playerTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value, out subsystemTuple)
				&& itemTuple.Component2.Value == playerTuple.Entity.Id)
			{
				var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
				var target = subsystemTuple.Component2.Items.FirstOrDefault(ic => ic != null && ic.CanCapture(itemTuple.Entity.Id));
				if (inventory != null 
					&& inventory.Item == itemTuple.Entity.Id
					&& target != null)
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
	}
}

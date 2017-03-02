using System.Linq;
using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class PickupItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }

		public int LocationId { get; set; }

		#region Equality members

		protected bool Equals(PickupItemCommand other)
		{
			return PlayerId == other.PlayerId && ItemId == other.ItemId && LocationId == other.LocationId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PickupItemCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = PlayerId;
				hashCode = (hashCode * 397) ^ ItemId;
				hashCode = (hashCode * 397) ^ LocationId;
				return hashCode;
			}
		}

		#region Implementation of IEquatable<ICommand>

		public bool Equals(ICommand other)
		{
			return Equals(other as PickupItemCommand);
		}

		#endregion

		#endregion
	}

	public class PickupItemCommandHandler : CommandHandler<PickupItemCommand>
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage> _playerMatcherGroup;

		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation, Activation> _itemMatcherGroup;

		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		public PickupItemCommandHandler(IMatcherProvider matcherProvider)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation, Activation>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
		}

		protected override bool TryProcessCommand(PickupItemCommand command)
		{
			ComponentEntityTuple<Player, ItemStorage> playerTuple;
			// TODO: item activation might not need to be mandatory?
			ComponentEntityTuple<Item, Owner, CurrentLocation, Activation> itemTuple;
			ComponentEntityTuple<Subsystem, ItemStorage> subsystemTuple;
			if (_playerMatcherGroup.TryGetMatchingEntity(command.PlayerId, out playerTuple)
				&& _itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out itemTuple)
				&& itemTuple.Component2.Value.HasValue == false
				&& itemTuple.Component4.ActivationState == ActivationState.NotActive
				&& itemTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component3.Value.Value, out subsystemTuple))
			{
				var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
				var source = subsystemTuple.Component2.Items.SingleOrDefault(ic => ic.Item == itemTuple.Entity.Id);
				if (inventory != null && inventory.Item.HasValue == false
					&& source != null && source.CanRelease)
				{
					inventory.Item = itemTuple.Entity.Id;
					itemTuple.Component2.Value = playerTuple.Entity.Id;
					itemTuple.Component3.Value = null;
					source.Item = null;
					return true;
				}

			}
			return false;
		}
	}
}

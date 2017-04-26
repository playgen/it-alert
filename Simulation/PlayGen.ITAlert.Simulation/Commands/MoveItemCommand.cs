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
	public class MoveItemCommand : ICommand
	{
		public int SystemEntityId { get; set; }

		public int PlayerId { get; set; }

		public int ItemId { get; set; }

		public int SourceContainerId { get; set; }

		public int DestinationContainerId { get; set; }

	}

	public class MoveItemCommandHandler : CommandHandler<MoveItemCommand>
	{

		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation> _itemMatcherGroup;

		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;


		#region Overrides of CommandHandler<DropItemCommand>

		public override IEqualityComparer<ICommand> Deduplicator => new MoveItemCommandEqualityComparer();

		#endregion

		public MoveItemCommandHandler(IMatcherProvider matcherProvider)
		{
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
		}

		protected override bool TryHandleCommand(MoveItemCommand command, int currentTick, bool handlerEnabled)
		{
			if (_itemMatcherGroup.TryGetMatchingEntity(command.ItemId, out var itemTuple)
				&& _subsystemMatcherGroup.TryGetMatchingEntity(command.SystemEntityId, out var subsystemTuple)
				&& itemTuple.Component2.Value == null)
			{
				var source = subsystemTuple.Component2.Items[command.SourceContainerId];
				var target = subsystemTuple.Component2.Items[command.DestinationContainerId];
				if (source != null 
					&& source.Item == itemTuple.Entity.Id
					&& source.CanRelease
					&& target != null
					&& target.CanCapture(itemTuple.Entity.Id))
				{
					target.Item = itemTuple.Entity.Id;
					source.Item = null;
					return true;
				}

			}
			return false;
		}

		public class MoveItemCommandEqualityComparer : CommandEqualityComparer<MoveItemCommand>
		{
			#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

			protected override bool IsDuplicate(MoveItemCommand x, MoveItemCommand other)
			{
				// player can only ever have one destination
				return x.PlayerId == other.PlayerId && x.ItemId == other.ItemId;
			}

			#endregion
		}
	}
}

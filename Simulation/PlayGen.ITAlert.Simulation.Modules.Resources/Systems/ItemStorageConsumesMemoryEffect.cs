using Engine.Components;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Resources.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Resources.Systems
{
	/// <summary>
	/// This subsystem resource effect causes the Memory Resource to be consumed by items located in the item containers of Subsystems with the ItemStorage component
	/// </summary>
	public class ItemStorageConsumesMemoryEffect : ISubsystemResourceEffect
	{
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage, MemoryResource> _subsystemMatcher;
		private readonly ComponentMatcherGroup<IItemType, ConsumeMemory> _itemMatcher;

		public ItemStorageConsumesMemoryEffect(IMatcherProvider matcherProvider)
		{
			_subsystemMatcher = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage, MemoryResource>();
			_itemMatcher = matcherProvider.CreateMatcherGroup<IItemType, ConsumeMemory>();
		}

		public void Tick()
		{
			foreach (var subsystemTuple in _subsystemMatcher.MatchingEntities)
			{
				var sum = subsystemTuple.Component3.Value;
				foreach (var itemContainer in subsystemTuple.Component2.Items)
				{
					ComponentEntityTuple<IItemType, ConsumeMemory> itemTuple;
					if (itemContainer.Item != null && _itemMatcher.TryGetMatchingEntity(itemContainer.Item.Value, out itemTuple))
					{
						sum += itemTuple.Component2.Value;
					}
				}
				subsystemTuple.Component3.Value = RangeHelper.AssignWithinBounds(sum, 0, subsystemTuple.Component3.Maximum);
			}
		}

		public void Dispose()
		{
			_subsystemMatcher?.Dispose();
			_itemMatcher?.Dispose();
		}
	}
}

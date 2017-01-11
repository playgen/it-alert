using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public class AnalyserEnhancementExtension : IEnhancementSystemExtension
	{
		// TODO: load this from somewhere else / constants
		// TODO: verify everything that is declared as const in the solution
		private const int AnalysisActivatorStorageLocation = 0;
		private const int AnalysisTargetStorageLocation = 2;

		public void Initialize(Entity entity)
		{
			ItemStorage itemStorage;
			if (entity.TryGetComponent(out itemStorage))
			{
				itemStorage.SetCustomContainer(2, new AnalysisiTargetItemContainer());
			}
			//TODO: implement the item activator
		}

		public static bool CanActivate(Entity entity)
		{
			ItemStorage itemStorage;
			if (entity.TryGetComponent(out itemStorage))
			{
				var itemContainer = itemStorage.Items[2] as AnalysisiTargetItemContainer;
				if (itemContainer != null)
				{
					return itemContainer.HasItem && itemContainer.Item.TestComponent<EntityTypeProperty>(et => et.Value == EntityType.Npc) && itemContainer.Item.HasComponent<MalwareGenome>();
				}
			}
			return false;
		} 
	}

	#region custom containers

	public class AnalysisiTargetItemContainer : ItemContainer
	{

		public override bool CanPickup { get; }
	}

	public class AnalysisiActivatorItemContainer : ItemContainer
	{

		public override bool CanPickup { get; }
	}


	#endregion
}

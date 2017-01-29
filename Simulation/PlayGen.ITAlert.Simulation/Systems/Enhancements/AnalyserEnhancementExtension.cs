using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public class AnalyserEnhancementExtension : IEnhancementSystemExtension
	{
		//public const int AnalysisActivatorStorageLocation = 0;
		public const int AnalysisTargetStorageLocation = 2;

		private ComponentMatcherGroup _analyserMatcherGroup;

		public AnalyserEnhancementExtension(IComponentRegistry componentRegistry)
		{
			// TODO: the matcher should be smart enough to infer all required types from the ComponentDependency attributes on the types specified
			_analyserMatcherGroup = componentRegistry.CreateMatcherGroup(new[] {typeof(AnalyserEnhancement)});
			_analyserMatcherGroup.MatchingEntityAdded += OnNewEntity;
		}

		// TODO: this should be run on every new entity created matching the Analyser flag
		public void OnSystemInitialize(Entity entity)
		{
		}

		public void OnNewEntity(Entity entity)
		{
			ItemStorage itemStorage;
			if (entity.TryGetComponent(out itemStorage))
			{
				itemStorage.Items[AnalysisTargetStorageLocation] = new AnalysisTargetItemContainer();
			}
			//TODO: implement the item activator
		}

		public static bool CanActivate(Entity entity)
		{
			ItemStorage itemStorage;
			if (entity.TryGetComponent(out itemStorage))
			{
				var itemContainer = itemStorage.Items[AnalysisTargetStorageLocation] as AnalysisTargetItemContainer;
				if (itemContainer != null)
				{
					return itemContainer.HasItem && itemContainer.Item.TestComponent<EntityTypeProperty>(et => et.Value == EntityType.Npc) && itemContainer.Item.HasComponent<MalwareGenome>();
				}
			}
			return false;
		} 
	}

	#region custom containers

	public class AnalysisTargetItemContainer : ItemContainer
	{

		public override bool CanPickup { get; }
	}

	public class AnalysisActivatorItemContainer : ItemContainer
	{

		public override bool CanPickup { get; }
	}


	#endregion
}

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

		private readonly ComponentMatcherGroup<AnalyserEnhancement, ItemStorage> _analyserMatcherGroup;

		private readonly IEntityRegistry _entityRegistry;

		public AnalyserEnhancementExtension(IEntityRegistry entityRegistry, IComponentRegistry componentRegistry)
		{
			_entityRegistry = entityRegistry;

			// TODO: the matcher should be smart enough to infer all required types from the ComponentDependency attributes on the types specified
			_analyserMatcherGroup = componentRegistry.CreateMatcherGroup<AnalyserEnhancement, ItemStorage>();
			_analyserMatcherGroup.MatchingEntityAdded += OnNewEntity;
		}

		// TODO: this should be run on every new entity created matching the Analyser flag
		public void OnSystemInitialize(Entity entity)
		{
		}

		public void OnNewEntity(ComponentEntityTuple<AnalyserEnhancement, ItemStorage> tuple)
		{
			var itemStorage = tuple.Component2;
			itemStorage.Items[AnalysisTargetStorageLocation] = new AnalysisTargetItemContainer();
			//TODO: implement the item activator
		}

		public bool CanActivate(Entity entity)
		{
			ItemStorage itemStorage;
			if (entity.TryGetComponent(out itemStorage))
			{
				var itemContainer = itemStorage.Items[AnalysisTargetStorageLocation] as AnalysisTargetItemContainer;
				Entity item;
				if (itemContainer != null && itemContainer.Item.HasValue && _entityRegistry.TryGetEntityById(itemContainer.Item.Value, out item))
				{
					return item.TestComponent<EntityTypeProperty>(et => et.Value == EntityType.Npc) && item.HasComponent<MalwareGenome>();
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

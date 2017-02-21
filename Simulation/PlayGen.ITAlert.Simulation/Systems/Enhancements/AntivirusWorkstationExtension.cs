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
using PlayGen.ITAlert.Simulation.Exceptions;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public class AntivirusWorkstationExtension : IEnhancementSystemExtension
	{
		public const string AnalysisActivatorArchetypeName = "AnalysisActivator";
		public const int AnalysisActivatorStorageLocation = 0;

		public const int AnalysisTargetStorageLocation = 2;
		public const int AnalysisOutputStorageLocation = 3;

		private readonly ComponentMatcherGroup<AntivirusEnhancement, ItemStorage> _antivirusMatcherGroup;

		private readonly ComponentMatcherGroup<Capture> _captureMatcherGroup;

		private readonly IEntityFactoryProvider _entityFactoryProvider;

		public AntivirusWorkstationExtension(IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			_entityFactoryProvider = entityFactoryProvider;

			// TODO: the matcher should be smart enough to infer all required types from the ComponentDependency attributes on the types specified
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<AntivirusEnhancement, ItemStorage>();
			_antivirusMatcherGroup.MatchingEntityAdded += OnNewEntity;

			_captureMatcherGroup = matcherProvider.CreateMatcherGroup<Capture>();
		}

		// TODO: this should be run on every new entity created matching the Analyser flag
		public void OnSystemInitialize(Entity entity)
		{
		}

		public void OnNewEntity(ComponentEntityTuple<AntivirusEnhancement, ItemStorage> tuple)
		{
			var itemStorage = tuple.Component2;

			itemStorage.Items[AnalysisTargetStorageLocation] = new AnalysisTargetItemContainer(_captureMatcherGroup);

			Entity activatorEntity;
			if (_entityFactoryProvider.TryCreateEntityFromArchetype(AnalysisActivatorArchetypeName, out activatorEntity) == false)
			{
				throw new SimulationException("AnalysisActivator archetype not registered");
			}
			itemStorage.Items[AnalysisActivatorStorageLocation] = new AnalysisActivatorItemContainer()
			{
				Item = activatorEntity.Id,
			};
			itemStorage.Items[AnalysisOutputStorageLocation] = new AnalysisOutputItemContainer();
		}

		//public bool CanActivate(Entity entity)
		//{
		//	ItemStorage itemStorage;
		//	if (entity.TryGetComponent(out itemStorage))
		//	{
		//		var itemContainer = itemStorage.Items[AnalysisTargetStorageLocation] as AnalysisTargetItemContainer;
		//		Entity item;
		//		if (itemContainer != null && itemContainer.Item.HasValue && _entityRegistry.TryGetEntityById(itemContainer.Item.Value, out item))
		//		{
		//			return item.TestComponent<EntityTypeProperty>(et => et.Value == EntityType.Npc) && item.HasComponent<MalwareGenome>();
		//		}
		//	}
		//	return false;
		//} 
	}

	#region custom containers

	public class AnalysisTargetItemContainer : ItemContainer
	{
		private readonly ComponentMatcherGroup<Capture> _captureMatcherGroup;

		public AnalysisTargetItemContainer(ComponentMatcherGroup<Capture> captureMatcherGroup)
		{
			_captureMatcherGroup = captureMatcherGroup;
		}

		public override bool CanRelease => false;

		public override bool CanCapture(int? itemId = null)
		{
			Entity capturEntity;
			return itemId.HasValue && _captureMatcherGroup.TryGetMatchingEntity(itemId.Value, out capturEntity);
		}
	}

	public class AnalysisOutputItemContainer : ItemContainer
	{
		public override bool CanRelease => true;

		public override bool CanCapture(int? itemId = null)
		{
			return false;
		}
	}

	public class AnalysisActivatorItemContainer : ItemContainer
	{

		public override bool CanRelease => false;

		public override bool CanCapture(int? itemId = null)
		{
			return false;
		}
	}


	#endregion
}

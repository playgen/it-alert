using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public class TransferEnhancementSystemExtension : IEnhancementSystemExtension
	{
		#region activator acrchetype

		public const string TransferActivatorArchetypeName = "TransferActivator";

		public static readonly Archetype TransferActivator = new Archetype(TransferActivatorArchetypeName)
			.Extends(Archetypes.Item.Archetype)
			.HasComponent(new ComponentBinding<TransferActivator>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		#endregion

		public const int TransferActivatorStorageLocation = 0;

		public const int TransferStorageLocation = 2;

		private readonly ComponentMatcherGroup<TransferEnhancement, ItemStorage> _transferMatcherGroup;

		private readonly IEntityFactoryProvider _entityFactoryProvider;

		public TransferEnhancementSystemExtension(IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			_entityFactoryProvider = entityFactoryProvider;

			// TODO: the matcher should be smart enough to infer all required types from the ComponentDependency attributes on the types specified
			_transferMatcherGroup = matcherProvider.CreateMatcherGroup<TransferEnhancement, ItemStorage>();
			_transferMatcherGroup.MatchingEntityAdded += OnNewEntity;
		}

		// TODO: this should be run on every new entity created matching the AnalyserActivator flag
		public void OnSystemInitialize(Entity entity)
		{
		}

		public void OnNewEntity(ComponentEntityTuple<TransferEnhancement, ItemStorage> tuple)
		{
			var itemStorage = tuple.Component2;

			itemStorage.Items[TransferStorageLocation] = new TransferItemContainer();

			ComponentEntityTuple<CurrentLocation, Owner> activatorEntityTuple;
			if (_entityFactoryProvider.TryCreateItem(TransferActivatorArchetypeName, tuple.Entity.Id, null, out activatorEntityTuple) == false)
			{
				throw new SimulationException($"{TransferActivatorArchetypeName} archetype not registered");
			}
			itemStorage.Items[TransferActivatorStorageLocation] = new TransferActivatorTargetItemContainer()
			{
				Item = activatorEntityTuple.Entity.Id,
			};
		}
	}

	#region custom containers

	public class TransferItemContainer : TargetItemContainer
	{
	}

	public class TransferActivatorTargetItemContainer : ActivatorItemContainer
	{
		
	}
	
	#endregion
}

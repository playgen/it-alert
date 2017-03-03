using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public class GarbageDisposalExtension : IEnhancementSystemExtension
	{
		public const string GarbageDisposalActivatorArchetypeName = "GarbageDisposalActivator";
		public const int GarbageDisposalActivatorStorageLocation = 0;
		public const int GarbageDisposalTargetStorageLocation = 2;

		private readonly ComponentMatcherGroup<GarbageDisposalEnhancement, ItemStorage> _garbageDisposalMatcherGroup;

		private readonly IEntityFactoryProvider _entityFactoryProvider;

		public GarbageDisposalExtension(IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			// TODO: the matcher should be smart enough to infer all required types from the ComponentDependency attributes on the types specified
			_garbageDisposalMatcherGroup = matcherProvider.CreateMatcherGroup<GarbageDisposalEnhancement, ItemStorage>();
			_garbageDisposalMatcherGroup.MatchingEntityAdded += OnNewEntity;
			_entityFactoryProvider = entityFactoryProvider;
		}

		// TODO: this should be run on every new entity created matching the Analyser flag
		public void OnSystemInitialize(Entity entity)
		{
		}

		public void OnNewEntity(ComponentEntityTuple<GarbageDisposalEnhancement, ItemStorage> tuple)
		{
			var itemStorage = tuple.Component2;

			itemStorage.Items[GarbageDisposalTargetStorageLocation] = new GarbageDisposalTargetItemContainer();

			ComponentEntityTuple<CurrentLocation, Owner> activatorEntityTuple;
			if (_entityFactoryProvider.TryCreateItem(GarbageDisposalActivatorArchetypeName, tuple.Entity.Id, null, out activatorEntityTuple) == false)
			{
				throw new SimulationException($"{GarbageDisposalActivatorArchetypeName} archetype not registered");
			}
			itemStorage.Items[GarbageDisposalActivatorStorageLocation] = new AnalysisActivatorItemContainer()
			{
				Item = activatorEntityTuple.Entity.Id,
			};
		}
	}

	#region custom containers

	public class GarbageDisposalTargetItemContainer : ItemContainer
	{

		public GarbageDisposalTargetItemContainer()
		{
		}

		public override bool Enabled => true;

		public override bool CanRelease => true;

		public override bool CanCapture(int? itemId = null)
		{
			return true;
		}
	}

	public class GarbageDisposalActivatorItemContainer : ItemContainer
	{
		public override bool CanRelease => false;

		public override bool CanCapture(int? itemId = null)
		{
			return false;
		}
	}


	#endregion
}

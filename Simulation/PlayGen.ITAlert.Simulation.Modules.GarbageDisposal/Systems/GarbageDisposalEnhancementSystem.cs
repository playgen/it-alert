using Engine.Archetypes;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Components;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Systems
{
	public class GarbageDisposalEnhancementSystem : ISystem
	{
		#region Archetype

		public const string GarbageDisposalActivatorArchetypeName = "GarbageDisposalActivator";

		public static readonly Archetype GarbageDisposalActivator = new Archetype(GarbageDisposalActivatorArchetypeName)
			.Extends(Item.Archetype)
			.HasComponent(new ComponentBinding<GarbageDisposalActivator>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		#endregion

		public const int GarbageDisposalActivatorStorageLocation = 0;
		public const int GarbageDisposalTargetStorageLocation = 2;

		private readonly IEntityFactoryProvider _entityFactoryProvider;

		public GarbageDisposalEnhancementSystem(IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			// TODO: the matcher should be smart enough to infer all required types from the ComponentDependency attributes on the types specified
			var garbageDisposalMatcherGroup = matcherProvider.CreateMatcherGroup<GarbageDisposalEnhancement, ItemStorage>();
			garbageDisposalMatcherGroup.MatchingEntityAdded += OnNewEntity;
			_entityFactoryProvider = entityFactoryProvider;
		}

		// TODO: this should be run on every new entity created matching the AnalyserActivator flag
		public void OnSystemInitialize(Entity entity)
		{
		}

		public void OnNewEntity(ComponentEntityTuple<GarbageDisposalEnhancement, ItemStorage> tuple)
		{
			var itemStorage = tuple.Component2;

			itemStorage.Items[GarbageDisposalTargetStorageLocation] = new GarbageDisposalTargetItemContainer();

			if (_entityFactoryProvider.TryCreateItem(GarbageDisposalActivatorArchetypeName, tuple.Entity.Id, null, out var activatorEntityTuple))
			{
				itemStorage.Items[GarbageDisposalActivatorStorageLocation] = new GarbageDisposalActivatorItemContainer()
				{
					Item = activatorEntityTuple.Entity.Id,
				};
			}
			else
			{
				throw new SimulationException($"{GarbageDisposalActivatorArchetypeName} archetype not registered");
			}
		}

		public void Dispose()
		{
			// nothing to dispose
		}
	}

	#region custom containers

	public class GarbageDisposalTargetItemContainer : TargetItemContainer
	{
	}

	public class GarbageDisposalActivatorItemContainer : ActivatorItemContainer
	{
	}

	#endregion
}

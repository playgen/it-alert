using Engine.Archetypes;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems
{
	public class AntivirusEnhancementSystem : ISystem
	{
		#region activator acrchetype

		public const string AnalyserActivatorArchetypeName = "AnalyserActivator";

		public static readonly Archetype AnalysisActivator = new Archetype(AnalyserActivatorArchetypeName)
			.Extends(Item.Archetype)
			.HasComponent(new ComponentBinding<AnalyserActivator>())
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

		#endregion

		public const int AnalysisActivatorStorageLocation = 0;

		public const int AnalysisTargetStorageLocation = 2;
		public const int AnalysisOutputStorageLocation = 3;

		private readonly ComponentMatcherGroup<Capture> _captureMatcherGroup;

		private readonly IEntityFactoryProvider _entityFactoryProvider;

		#region constructors

		public AntivirusEnhancementSystem(IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			_entityFactoryProvider = entityFactoryProvider;

			// TODO: the matcher should be smart enough to infer all required types from the ComponentDependency attributes on the types specified
			var antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<AntivirusEnhancement, ItemStorage>();
			antivirusMatcherGroup.MatchingEntityAdded += OnNewEntity;

			_captureMatcherGroup = matcherProvider.CreateMatcherGroup<Capture>();
		}

		#endregion

		// TODO: this should be run on every new entity created matching the AnalyserActivator flag
		public void OnSystemInitialize(Entity entity)
		{
		}

		public void OnNewEntity(ComponentEntityTuple<AntivirusEnhancement, ItemStorage> tuple)
		{
			var itemStorage = tuple.Component2;

			itemStorage.Items[AnalysisTargetStorageLocation] = new AnalysisTargetItemContainer(_captureMatcherGroup);

			ComponentEntityTuple<CurrentLocation, Owner> activatorEntityTuple;
			if (_entityFactoryProvider.TryCreateItem(AnalyserActivatorArchetypeName, tuple.Entity.Id, null, out activatorEntityTuple) == false)
			{
				throw new SimulationException($"{AnalyserActivatorArchetypeName} archetype not registered");
			}
			itemStorage.Items[AnalysisActivatorStorageLocation] = new AnalysisActivatorItemContainer()
			{
				Item = activatorEntityTuple.Entity.Id,
			};
			itemStorage.Items[AnalysisOutputStorageLocation] = new AnalysisOutputItemContainer();
		}
	}

}

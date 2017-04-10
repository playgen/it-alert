using Engine.Components;
using Engine.Entities;
using Engine.Systems.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems.Activation
{
	public class AnalyserBehaviour : IActivationExtension
	{
		public const string AnalysisOutputArchetypeName = "Antivirus";

		/// <summary>
		/// should the sample be consumed by the analysis process
		/// </summary>
		private const bool ConsumeSample = true;
		
		private readonly ComponentMatcherGroup<Analyser, CurrentLocation, Owner> _analyserMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<Capture> _captureMatcherGroup;

		private readonly IEntityFactoryProvider _entityFactoryProvider;

		public AnalyserBehaviour(IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			_analyserMatcherGroup = matcherProvider.CreateMatcherGroup<Analyser, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_captureMatcherGroup = matcherProvider.CreateMatcherGroup<Capture>();

			_entityFactoryProvider = entityFactoryProvider;
		}

		public void OnNotActive(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_analyserMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple)
				&& itemTuple.Component2.Value.HasValue
				&& itemTuple.Component3.Value.HasValue)
			{
				itemTuple.Component3.Value = null;
			}
		}

		public void OnActivating(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_analyserMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple))
			{
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out var locationTuple)
					&& locationTuple.Component2.TryGetItemContainer<AnalysisTargetItemContainer>(out var analysisTargetItemContainer)
					&& analysisTargetItemContainer.Item.HasValue
					&& _captureMatcherGroup.TryGetMatchingEntity(analysisTargetItemContainer.Item.Value, out var captureTuple)
					&& captureTuple.Component1.CapturedGenome != 0)
				{
					analysisTargetItemContainer.Locked = true;
				}
				else
				{
					activation.CancelActivation();
				}
			}

		}

		public void OnActive(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_analyserMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple))
			{
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out var locationTuple)
					&& locationTuple.Component2.TryGetItemContainer<AnalysisTargetItemContainer>(out var analysisTargetItemContainer)
					&& analysisTargetItemContainer.Item.HasValue
					&& _captureMatcherGroup.TryGetMatchingEntity(analysisTargetItemContainer.Item.Value, out var captureTuple)
					&& captureTuple.Component1.CapturedGenome != 0
					&& locationTuple.Component2.TryGetItemContainer<AnalysisOutputItemContainer>(out var analysisOutputItemContainer)
					&& analysisOutputItemContainer.Item.HasValue == false)
				{
					if (_entityFactoryProvider.TryCreateItem(AnalysisOutputArchetypeName, locationTuple.Entity.Id, null, out var antivirusEntityTuple)
						&& antivirusEntityTuple.Entity.TryGetComponent<Components.Antivirus>(out var antivirus))
					{
						antivirus.TargetGenome = captureTuple.Component1.CapturedGenome;
						analysisOutputItemContainer.Item = antivirusEntityTuple.Entity.Id;

						if (ConsumeSample)
						{
							captureTuple.Component1.CapturedGenome = 0;
						}
					}
					else
					{
						antivirusEntityTuple.Entity.Dispose();
					}
					analysisTargetItemContainer.Locked = false;
				}
			}
		}
	}
}

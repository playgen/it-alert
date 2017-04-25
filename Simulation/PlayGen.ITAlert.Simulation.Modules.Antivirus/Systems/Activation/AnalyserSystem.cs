using Engine.Components;
using Engine.Entities;
using Engine.Events;
using Engine.Systems;
using Engine.Systems.Activation;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems.Activation
{
	public class AnalyserSystem : ITickableSystem
	{
		/// <summary>
		/// should the sample be consumed by the analysis process
		/// </summary>
		private const bool ConsumeSample = false;
		
		private readonly ComponentMatcherGroup<Engine.Systems.Activation.Components.Activation, Analyser, CurrentLocation, Owner> _analyserMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<Capture> _captureMatcherGroup;

		private readonly IEntityFactoryProvider _entityFactoryProvider;

		private readonly EventSystem _eventSystem;

		public AnalyserSystem(IMatcherProvider matcherProvider, 
			IEntityFactoryProvider entityFactoryProvider,
			EventSystem eventSystem)
		{
			_analyserMatcherGroup = matcherProvider.CreateMatcherGroup<Engine.Systems.Activation.Components.Activation, Analyser, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_captureMatcherGroup = matcherProvider.CreateMatcherGroup<Capture>();

			_entityFactoryProvider = entityFactoryProvider;
			_eventSystem = eventSystem;
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _analyserMatcherGroup.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.NotActive:
						OnNotActive(match, currentTick);
						break;
					case ActivationState.Activating:
						OnActivating(match, currentTick);
						break;
					case ActivationState.Deactivating:
						OnDeactivating(match, currentTick);
						break;
				}
			}
		}

		private void OnNotActive(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, Analyser, CurrentLocation, Owner> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue
				&& entityTuple.Component4.Value.HasValue)
			{
				entityTuple.Component4.Value = null;
			}
		}

		private void OnActivating(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, Analyser, CurrentLocation, Owner> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(entityTuple.Component3.Value.Value, out var locationTuple)
				&& locationTuple.Component2.TryGetItemContainer<AnalysisTargetItemContainer>(out var analysisTargetItemContainer)
				&& analysisTargetItemContainer.Item.HasValue
					&& _captureMatcherGroup.TryGetMatchingEntity(analysisTargetItemContainer.Item.Value, out var captureTuple)
					&& captureTuple.Component1.CapturedGenome != 0)
			{
				analysisTargetItemContainer.Locked = true;
			}
			else
			{
				entityTuple.Component1.SetState(ActivationState.NotActive, currentTick);
				var @event = new AnalyserActivationEvent()
				{
					PlayerEntityId = entityTuple.Component4.Value.Value,
					ActivationResult = AnalyserActivationEvent.AnalyserActivationResult.NoSamplePresent,
					LocationEntityId = entityTuple.Component3.Value,
				};
				_eventSystem.Publish(@event);
			}
		}

		private void OnDeactivating(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, Analyser, CurrentLocation, Owner> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(entityTuple.Component3.Value.Value, out var locationTuple)
				&& locationTuple.Component2.TryGetItemContainer<AnalysisTargetItemContainer>(out var analysisTargetItemContainer)
				&& analysisTargetItemContainer.Item.HasValue
				&& _captureMatcherGroup.TryGetMatchingEntity(analysisTargetItemContainer.Item.Value, out var captureTuple)
				&& captureTuple.Component1.CapturedGenome != 0
				&& locationTuple.Component2.TryGetItemContainer<AnalysisOutputItemContainer>(out var analysisOutputItemContainer))
			{
				var @event = new AnalyserActivationEvent()
				{
					PlayerEntityId = entityTuple.Component4.Value.Value,
					LocationEntityId = entityTuple.Component3.Value,
				};

				if (analysisOutputItemContainer.Item.HasValue == false)
				{
					_eventSystem.Publish(@event);

					if (_entityFactoryProvider.TryCreateItem(AntivirusTool.Archetype, locationTuple.Entity.Id, null, out var antivirusTuple)
						&& antivirusTuple.Entity.TryGetComponent<Components.Antivirus>(out var antivirus))
					{
						antivirus.TargetGenome = captureTuple.Component1.CapturedGenome;
						analysisOutputItemContainer.Item = antivirusTuple.Entity.Id;

						if (ConsumeSample)
						{
							captureTuple.Component1.CapturedGenome = 0;
						}
						@event.ActivationResult = AnalyserActivationEvent.AnalyserActivationResult.AnalysisComplete;
						@event.GenomeProduced = antivirus.TargetGenome;
					}
					else
					{
						antivirusTuple.Entity.Dispose();
					}
					analysisTargetItemContainer.Locked = false;
				}
				else
				{
					@event.ActivationResult = AnalyserActivationEvent.AnalyserActivationResult.OutputContainerFull;
				}
				_eventSystem.Publish(@event);
			}
		}

		public void Dispose()
		{
			_analyserMatcherGroup?.Dispose();
			_subsystemMatcherGroup?.Dispose();
			_captureMatcherGroup?.Dispose();
		}
	}
}

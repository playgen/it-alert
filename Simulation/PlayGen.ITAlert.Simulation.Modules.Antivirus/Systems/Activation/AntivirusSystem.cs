﻿using System.Linq;
using Engine.Components;
using Engine.Events;
using Engine.Systems;
using Engine.Systems.Activation;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems.Activation
{
	public class AntivirusSystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Engine.Systems.Activation.Components.Activation, Components.Antivirus, CurrentLocation, Owner, ConsumableActivation> _antivirusMatcherGroup;
		private readonly ComponentMatcherGroup<Visitors> _visitorsMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome> _malwareMatcherGroup;

		private readonly EventSystem _eventSystem;

		public AntivirusSystem(IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<Engine.Systems.Activation.Components.Activation, Components.Antivirus, CurrentLocation, Owner, ConsumableActivation>();
			_visitorsMatcherGroup = matcherProvider.CreateMatcherGroup<Visitors>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome>();

			_eventSystem = eventSystem;
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _antivirusMatcherGroup.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.Deactivating:
						OnDeactivating(match);
						break;
				}
			}
		}

		private void OnDeactivating(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, Components.Antivirus, CurrentLocation, Owner, ConsumableActivation> entityTuple)
		{
			if (entityTuple.Component3.Value.HasValue
				&&_visitorsMatcherGroup.TryGetMatchingEntity(entityTuple.Component3.Value.Value, out var locationTuple))
			{
				var malwareVisitors = locationTuple.Component1.Values
					.Join(_malwareMatcherGroup.MatchingEntities,
						k => k,
						k => k.Entity.Id,
						(o, i) => i)
					.ToArray();

				var @event = new AntivirusActivationEvent()
				{
					PlayerEntityId = entityTuple.Component4.Value.Value,
					LocationEntityId = locationTuple.Entity.Id,
					Uses = entityTuple.Component5.ActivationsRemaining - 1,
				};

				if (malwareVisitors.Length == 0)
				{
					@event.ActivationResult = AntivirusActivationEvent.AntivirusActivationResult.NoVirusPresent;
				}

				// join the current locations list of visitors with all malware entities
				foreach (var malwareVisitor in malwareVisitors)
				{
					if ((malwareVisitor.Component1.Value & entityTuple.Component2.TargetGenome) == malwareVisitor.Component1.Value)
					{
						malwareVisitor.Entity.Dispose();

						@event.ActivationResult = AntivirusActivationEvent.AntivirusActivationResult.SoloExtermination;
					}
					else
					{
						@event.ActivationResult = AntivirusActivationEvent.AntivirusActivationResult.IncorrectGenome;
					}
				}

				_eventSystem.Publish(@event);
			}
			entityTuple.Component4.Value = null;
		}

		public void Dispose()
		{
			_antivirusMatcherGroup?.Dispose();
			_visitorsMatcherGroup?.Dispose();
			_malwareMatcherGroup?.Dispose();
		}
	}
}

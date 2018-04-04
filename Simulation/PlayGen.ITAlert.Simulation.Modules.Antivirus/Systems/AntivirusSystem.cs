using System.Linq;
using Engine.Components;
using Engine.Events;
using Engine.Systems;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems
{
	public class AntivirusSystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Activation, Components.Antivirus, CurrentLocation, Owner, ConsumableActivation> _antivirusMatcherGroup;
		private readonly ComponentMatcherGroup<Visitors> _visitorsMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome> _malwareMatcherGroup;

		private readonly EventSystem _eventSystem;

		public AntivirusSystem(IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<Activation, Components.Antivirus, CurrentLocation, Owner, ConsumableActivation>();
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
					case ActivationState.NotActive:
						OnNotActive(match, currentTick);
						break;
					case ActivationState.Deactivating:
						OnDeactivating(match);
						break;
				}
			}
		}

		private void OnNotActive(ComponentEntityTuple<Activation, Components.Antivirus, CurrentLocation, Owner, ConsumableActivation> entityTuple, int currentTick)
		{
			if (entityTuple.Component3.Value.HasValue
				&& entityTuple.Component4.Value.HasValue)
			{
				entityTuple.Component4.Value = null;
				entityTuple.Component5.ActivationsRemaining -= 1;
				if (entityTuple.Component5.ActivationsRemaining == 0)
				{
					entityTuple.Entity.Dispose();
				}
			}
		}

		private void OnDeactivating(ComponentEntityTuple<Activation, Components.Antivirus, CurrentLocation, Owner, ConsumableActivation> entityTuple)
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
					SubsystemEntityId = locationTuple.Entity.Id,
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
						@event.GenomeEradicated = malwareVisitor.Component1.Value;
						@event.ActivationResult = AntivirusActivationEvent.AntivirusActivationResult.SoloExtermination;
						malwareVisitor.Entity.Dispose();
					}
					else
					{
						@event.ActivationResult = AntivirusActivationEvent.AntivirusActivationResult.IncorrectGenome;
					}
				}

				_eventSystem.Publish(@event);
			}
		}

		public void Dispose()
		{
			_antivirusMatcherGroup?.Dispose();
			_visitorsMatcherGroup?.Dispose();
			_malwareMatcherGroup?.Dispose();
		}
	}
}

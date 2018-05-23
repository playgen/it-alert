using System.Linq;
using Engine.Components;
using Engine.Events;
using Engine.Systems;
using Engine.Systems.Activation.Components;

using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems
{
	public class CoopMultiColourAntivirusSystem : ITickableSystem
	{
		private const bool AllowSamePlayerActivation = false;

		private readonly ComponentMatcherGroup<Engine.Systems.Activation.Components.Activation, Components.Antivirus, CurrentLocation, Owner> _antivirusMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage, Visitors> _locationMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome, MalwarePropogation, CurrentLocation> _malwareMatcherGroup;

		private readonly EventSystem _eventSystem;

		public CoopMultiColourAntivirusSystem(IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<Engine.Systems.Activation.Components.Activation, Components.Antivirus, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem>();
			_locationMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage, Visitors>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome, MalwarePropogation, CurrentLocation>();

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
						OnDeactivating(match, currentTick);
						break;
				}
			}
		}

		private void OnDeactivating(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, Components.Antivirus, CurrentLocation, Owner> itemTuple, int currentTick)
		{
			if (itemTuple.Component3.Value.HasValue
				&& _locationMatcherGroup.TryGetMatchingEntity(itemTuple.Component3.Value.Value, out var locationTuple))
			{
				var activeAntivirus = locationTuple.Component2.Items.Where(ic => ic?.Item != null && ic.Item.Value != itemTuple.Entity.Id)
						.Join(_antivirusMatcherGroup.MatchingEntities.Where(av => av.Component1.ActivationState == ActivationState.Active 
							&& (AllowSamePlayerActivation || av.Component4.Value != itemTuple.Component4.Value)),
							k => k.Item.Value,
							k => k.Entity.Id,
							(o, i) => i)
							.ToArray();

				var otherPlayerIds = activeAntivirus.Where(av => av.Component4.Value.HasValue
						&& av.Component4.Value != itemTuple.Component4.Value)
					.Select(av => av.Component4.Value)
					.Distinct()
					.ToArray();

				var combinedGenome = itemTuple.Component2.TargetGenome | activeAntivirus.Aggregate(0, (g, av) => g |= av.Component2.TargetGenome, g => g);
				if (combinedGenome != 0 && combinedGenome != itemTuple.Component2.TargetGenome)
				{
					// join the current locations list of visitors with all malware entities
					foreach (var malwareVisitor in locationTuple.Component3.Values
						.Join(_malwareMatcherGroup.MatchingEntities,
							k => k,
							k => k.Entity.Id,
							(o, i) => i).ToArray())
					{
						if ((malwareVisitor.Component1.Value & combinedGenome) == malwareVisitor.Component1.Value)
						{

							foreach (var antivirus in activeAntivirus)
							{
								antivirus.Component1.SetState(ActivationState.NotActive, currentTick);
							}

							var malwareType = malwareVisitor.Component1.Value;
							malwareVisitor.Entity.Dispose();

							var @event = new AntivirusActivationEvent()
							{
								PlayerEntityId = itemTuple.Component4.Value.Value,
								ActivationResult = AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination,
								SubsystemEntityId = locationTuple.Entity.Id,
								GenomeEradicated = malwareType,
							};

							var infectionCount = new System.Collections.Generic.Dictionary<int, int>();
							var infections = _malwareMatcherGroup.MatchingEntities.Join(_subsystemMatcherGroup.MatchingEntities,
									mw => mw.Component3.Value,
									ss => ss.Entity.Id,
									(mw, ss) => mw)
								.ToArray();

							infectionCount.Add(SimulationConstants.MalwareGeneRed, infections.Count(i => i.Component1.Value == SimulationConstants.MalwareGeneRed || i.Component1.Value == (SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneGreen) || i.Component1.Value == (SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneBlue) || i.Component1.Value == (SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneGreen | SimulationConstants.MalwareGeneBlue)));
							infectionCount.Add(SimulationConstants.MalwareGeneGreen, infections.Count(i => i.Component1.Value == SimulationConstants.MalwareGeneGreen || i.Component1.Value == (SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneGreen) || i.Component1.Value == (SimulationConstants.MalwareGeneGreen | SimulationConstants.MalwareGeneBlue) || i.Component1.Value == (SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneGreen | SimulationConstants.MalwareGeneBlue)));
							infectionCount.Add(SimulationConstants.MalwareGeneBlue, infections.Count(i => i.Component1.Value == SimulationConstants.MalwareGeneBlue || i.Component1.Value == (SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneBlue) || i.Component1.Value == (SimulationConstants.MalwareGeneGreen | SimulationConstants.MalwareGeneBlue) || i.Component1.Value == (SimulationConstants.MalwareGeneRed | SimulationConstants.MalwareGeneGreen | SimulationConstants.MalwareGeneBlue)));
							@event.MalwareCount = infectionCount;

							_eventSystem.Publish(@event);

							foreach (var otherPlayer in otherPlayerIds)
							{
								@event = new AntivirusActivationEvent() {
									PlayerEntityId = otherPlayer.Value,
									ActivationResult = AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination,
									SubsystemEntityId = locationTuple.Entity.Id,
									GenomeEradicated = malwareType,
									MalwareCount = infectionCount
								};
								_eventSystem.Publish(@event);
							}
						}
					}
						
				}
			}
		}

		public void Dispose()
		{
			_antivirusMatcherGroup?.Dispose();
			_locationMatcherGroup?.Dispose();
			_malwareMatcherGroup?.Dispose();
		}
	}
}

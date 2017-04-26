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
using PlayGen.ITAlert.Simulation.Components.Player;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems
{
	public class CaptureSystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Engine.Systems.Activation.Components.Activation, Capture, CurrentLocation, Owner> _captureMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, Visitors> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome, MalwareVisibility> _malwareMatcherGroup;
		private readonly ComponentMatcherGroup<Player, PlayerBitMask> _playerMatcherGroup;

		private readonly EventSystem _eventSystem;

		public CaptureSystem(IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_captureMatcherGroup = matcherProvider.CreateMatcherGroup<Engine.Systems.Activation.Components.Activation, Capture, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, Visitors>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome, MalwareVisibility>();
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, PlayerBitMask>();

			_eventSystem = eventSystem;
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _captureMatcherGroup.MatchingEntities)
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

		public void OnDeactivating(ComponentEntityTuple<Engine.Systems.Activation.Components.Activation, Capture, CurrentLocation, Owner> entityTuple)
		{
			if (entityTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(entityTuple.Component3.Value.Value, out var locationTuple)
				&& _playerMatcherGroup.TryGetMatchingEntity(entityTuple.Component4.Value.Value, out var playerTuple))
			{
				// join the current locations list of visitors with all malware entities
				var malwareVisitor = locationTuple.Component2.Values
					.Join(_malwareMatcherGroup.MatchingEntities,
						k => k,
						k => k.Entity.Id,
						(o, i) => i)
					.FirstOrDefault();

				var @event = new CaptureActivationEvent()
				{
					PlayerEntityId = playerTuple.Entity.Id,
					LocationEntityId = locationTuple.Entity.Id,
				};

				// TODO: probably need a better way of choosing the malware than selecting first, but this will do for now
				if (malwareVisitor != null
					&& entityTuple.Component4.Value.HasValue
					&& (malwareVisitor.Component2.VisibleTo & playerTuple.Component2.Value) == playerTuple.Component2.Value)
				{
					// cyclical behaviour for capture on advanced genomes
					int gene = entityTuple.Component2.CapturedGenome;
					do
					{
						switch (gene)
						{
							case 0:
								gene = SimulationConstants.MalwareGeneRed;
								break;
							case SimulationConstants.MalwareGeneRed:
								gene = SimulationConstants.MalwareGeneGreen;
								break;
							case SimulationConstants.MalwareGeneGreen:
								gene = SimulationConstants.MalwareGeneBlue;
								break;
							case SimulationConstants.MalwareGeneBlue:
								gene = SimulationConstants.MalwareGeneRed;
								break;
						}

						if (HasGene(malwareVisitor.Component1.Value, gene))
						{
							entityTuple.Component2.CapturedGenome = gene;
						}
					} while (HasGene(malwareVisitor.Component1.Value, gene) == false);

					var genes = new []
						{
							SimulationConstants.MalwareGeneRed,
							SimulationConstants.MalwareGeneGreen,
							SimulationConstants.MalwareGeneBlue
						}
						.Count(g => HasGene(malwareVisitor.Component1.Value, g));

					@event.ActivationResult = genes > 1 
						? CaptureActivationEvent.CaptureActivationResult.ComplexGenomeCaptured
						: CaptureActivationEvent.CaptureActivationResult.SimpleGenomeCaptured;
				}
				else
				{
					@event.ActivationResult = CaptureActivationEvent.CaptureActivationResult.NoVirusPresent;
				}

				_eventSystem.Publish(@event);
			}
		}

		private bool HasGene(int genome, int gene)
		{
			return (genome & gene) == gene;
		}

		public void Dispose()
		{
			_captureMatcherGroup?.Dispose();
			_subsystemMatcherGroup?.Dispose();
			_malwareMatcherGroup?.Dispose();
			_playerMatcherGroup?.Dispose();
		}
	}
}

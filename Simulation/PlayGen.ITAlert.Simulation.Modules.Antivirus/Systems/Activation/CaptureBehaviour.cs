﻿using System.Linq;
using Engine.Components;
using Engine.Systems.Activation;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems.Activation
{
	public class CaptureBehaviour : IActivationExtension
	{
		public const string AnalysisOutputArchetypeName = "Antivirus";
		
		private readonly ComponentMatcherGroup<Capture, CurrentLocation, Owner> _captureMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, Visitors> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome, MalwareVisibility> _malwareMatcherGroup;
		private readonly ComponentMatcherGroup<Player, PlayerBitMask> _playerMatcherGroup;


		public CaptureBehaviour(IMatcherProvider matcherProvider)
		{
			_captureMatcherGroup = matcherProvider.CreateMatcherGroup<Capture, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, Visitors>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome, MalwareVisibility>();
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, PlayerBitMask>();

		}

		public void OnNotActive(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{

		}

		public void OnActivating(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			// do nothing
		}

		public void OnActive(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_captureMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple))
			{
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out var locationTuple)
					&& _playerMatcherGroup.TryGetMatchingEntity(itemTuple.Component3.Value.Value, out var playerTuple))
				{
					// join the current locations list of visitors with all malware entities
					var malwareVisitor = locationTuple.Component2.Values
						.Join(_malwareMatcherGroup.MatchingEntities,
							k => k,
							k => k.Entity.Id,
							(o, i) => i)
						.FirstOrDefault();

					// TODO: probably need a better way of choosing the malware than selecting first, but this will do for now
					if (malwareVisitor != null
						&& itemTuple.Component3.Value.HasValue
						&& (malwareVisitor.Component2.VisibleTo & playerTuple.Component2.Value) == playerTuple.Component2.Value)
					{
						// cyclical behaviour for capture on advanced genomes

						int gene = itemTuple.Component1.CapturedGenome;
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
								itemTuple.Component1.CapturedGenome = gene;
							}
						} while (HasGene(malwareVisitor.Component1.Value, gene) == false);
					}
				}
			}
		}

		private bool HasGene(int genome, int gene)
		{
			return (genome & gene) == gene;
		}
	}
}
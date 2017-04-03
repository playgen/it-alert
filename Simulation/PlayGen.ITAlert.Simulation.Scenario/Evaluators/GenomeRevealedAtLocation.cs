﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Exceptions;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class GenomeRevealedAtLocation : IEvaluator<Simulation, SimulationConfiguration>
	{
		private readonly int _genome;

		private readonly int _revealedTo;

		private readonly int _nodeId;

		private NodeConfig _node;

		private ComponentMatcherGroup<Visitors> _visitorsMatcherGroup;

		private ComponentMatcherGroup<MalwareGenome, MalwareVisibility> _malwareMatcherGroup;

		/// <summary>
		/// Evaluate whether there is an entity with the specified genome at the specified location that has been revealed to the specified players
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="genome">Genome bit flags. 0: Any genome</param>
		/// <param name="revealedTo">Visibility bit flags. 0: Any player (or none)</param>
		public GenomeRevealedAtLocation(int nodeId, int genome, int revealedTo)
		{
			_genome = genome;
			_revealedTo = revealedTo;
			_nodeId = nodeId;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			if (configuration.TrySelectNode(_nodeId, out _node) == false)
			{
				throw new ScenarioConfigurationException($"Node not found with id {_nodeId}");
			}
			_visitorsMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Visitors>();
			_malwareMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<MalwareGenome, MalwareVisibility>();
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			return ecs.Entities.TryGetValue(_node.EntityId, out var locationEntity)
				&& locationEntity.TryGetComponent<Visitors>(out var visitors)
				&& visitors.Values.Any(v => ecs.Entities.TryGetValue(v, out var visitorEntity)
					&& visitorEntity.TryGetComponent<MalwareGenome>(out var malwareGenome)
					&& (malwareGenome.Value & _genome) == _genome
					&& visitorEntity.TryGetComponent<MalwareVisibility>(out var malwareVisibility)
					&& (malwareVisibility.VisibleTo & _revealedTo) == _revealedTo);
		}

		public void Dispose()
		{
			_visitorsMatcherGroup?.Dispose();
			_malwareMatcherGroup?.Dispose();
		}
	}
}

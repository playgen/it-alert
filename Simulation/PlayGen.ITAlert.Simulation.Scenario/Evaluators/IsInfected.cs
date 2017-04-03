using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Evaluators;using PlayGen.ITAlert.Simulation;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Exceptions;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class IsInfected : IEvaluator<Simulation, SimulationConfiguration>
	{

		private readonly int? _nodeId;
		private NodeConfig _node;

		private ComponentMatcherGroup<Visitors> _locationMatcherGroup;
		private ComponentMatcherGroup<Malware> _malwareMatcherGroup;

		/// <summary>
		/// Evaluate if there is any malware present on the specified node, or anywhere if omitted
		/// </summary>
		/// <param name="nodeId">NodeConfig, Default: null (anywhere)</param>
		public IsInfected(int? nodeId = null)
		{
			_nodeId = nodeId;
		}
		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			if (_nodeId.HasValue && (configuration.TrySelectNode(_nodeId.Value, out _node) == false))
			{
				throw new ScenarioConfigurationException($"Node not found with id {_nodeId}");
			}
			_locationMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Visitors>();
			_malwareMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Malware>();
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			if (_node == null)
			{
				return _locationMatcherGroup.MatchingEntities.Any(locationTuple =>
					locationTuple.Component1.Values.Any(v => _malwareMatcherGroup.TryGetMatchingEntity(v, out var malwareTuple)));
			}
			else
			{
				return _locationMatcherGroup.TryGetMatchingEntity(_node.EntityId, out var locationTuple)
					&& locationTuple.Component1.Values.Any(v => _malwareMatcherGroup.TryGetMatchingEntity(v, out var malwareTuple));
			}
		}

		public void Dispose()
		{
			_locationMatcherGroup?.Dispose();
			_malwareMatcherGroup?.Dispose();
		}
	}
}

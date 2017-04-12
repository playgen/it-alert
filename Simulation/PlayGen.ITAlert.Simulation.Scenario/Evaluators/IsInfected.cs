using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters;
using PlayGen.ITAlert.Simulation.Scenario.Exceptions;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class IsInfected : IEvaluator<Simulation, SimulationConfiguration>
	{
		private readonly EntityConfig _nodeEntityConfig;

		private ComponentMatcherGroup<Visitors> _locationMatcherGroup;
		private ComponentMatcherGroup<Malware> _malwareMatcherGroup;

		private readonly IEntityFilter<Simulation, SimulationConfiguration> _filter;


		/// <summary>
		/// Evaluate if there is any malware present on the specified node, or anywhere if omitted
		/// </summary>
		/// <param name="nodeEntityConfig">NodeConfig, Default: null (anywhere)</param>
		public IsInfected(EntityConfig nodeEntityConfig = null, IEntityFilter<Simulation, SimulationConfiguration> filter = null)
		{
			_nodeEntityConfig = nodeEntityConfig;
			_filter = filter;
		}
		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			_locationMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Visitors>();
			_malwareMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Malware>();
			_filter?.Initialize(ecs, configuration);
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			if (_nodeEntityConfig == null)
			{
				return _locationMatcherGroup.MatchingEntities.Any(locationTuple =>
					locationTuple.Component1.Values.Any(v => _malwareMatcherGroup.TryGetMatchingEntity(v, out var malwareTuple)
						&& (_filter == null || _filter.Evaluate(malwareTuple.Entity))));
			}
			else
			{
				return _locationMatcherGroup.TryGetMatchingEntity(_nodeEntityConfig.EntityId, out var locationTuple)
					&& locationTuple.Component1.Values.Any(v => _malwareMatcherGroup.TryGetMatchingEntity(v, out var malwareTuple)
						&& (_filter == null || _filter.Evaluate(malwareTuple.Entity)));
			}
		}

		public void Dispose()
		{
			_locationMatcherGroup?.Dispose();
			_malwareMatcherGroup?.Dispose();
		}
	}
}

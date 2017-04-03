using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters;
using PlayGen.ITAlert.Simulation.Scenario.Exceptions;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class ItemTypeIsActivated<TItemType> : IEvaluator<Simulation, SimulationConfiguration>
		where TItemType : class, IItemType
	{
		private readonly int? _nodeId;

		private NodeConfig _node;
		private readonly IEntityFilter<Simulation, SimulationConfiguration> _filter;

		private ComponentMatcherGroup<TItemType, Activation, CurrentLocation> _itemMatcherGroup;

		/// <summary>
		/// Evaluate whether an item of the specified type has been activated optionally at the specidied location
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="filter"></param>
		public ItemTypeIsActivated(int? nodeId = null, IEntityFilter<Simulation, SimulationConfiguration> filter = null)
		{
			_nodeId = nodeId;
			_filter = filter;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			if (_nodeId.HasValue && (configuration.TrySelectNode(_nodeId.Value, out _node) == false))
			{
				throw new ScenarioConfigurationException($"Node not found with id {_nodeId}");
			}
			_itemMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<TItemType, Activation, CurrentLocation>();
			_filter?.Initialize(ecs, configuration);
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			return _itemMatcherGroup.MatchingEntities.Any(itemTuple =>
				itemTuple.Component2.ActivationState == ActivationState.Active
				&& (_node == null || itemTuple.Component3.Value == _node.EntityId)
				&& (_filter == null || _filter.Evaluate(itemTuple.Entity)));
		}

		public void Dispose()
		{
			_filter?.Dispose();
			_itemMatcherGroup?.Dispose();
		}
	}
}

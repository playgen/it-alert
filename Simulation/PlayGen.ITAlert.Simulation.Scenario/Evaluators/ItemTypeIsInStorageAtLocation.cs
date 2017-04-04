﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Evaluators;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters;
using PlayGen.ITAlert.Simulation.Scenario.Exceptions;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class ItemTypeIsInStorageAtLocation<TItemType> : IEvaluator<Simulation, SimulationConfiguration>
		where TItemType : class, IItemType
	{
		private readonly int _nodeId;
		private NodeConfig _node;
		private readonly IEntityFilter<Simulation, SimulationConfiguration> _entityFilter;

		private ComponentMatcherGroup<Components.EntityTypes.Subsystem, ItemStorage> _locationMatcherGroup;
		private ComponentMatcherGroup<TItemType> _itemMatcherGroup;

		/// <summary>
		/// Evaluates wheter an item of the specified type is present in the storage on the specified node.
		/// </summary>
		/// <param name="nodeId">Storage location</param>
		/// <param name="filter">Optional entity filter</param>
		public ItemTypeIsInStorageAtLocation(int nodeId, IEntityFilter<Simulation, SimulationConfiguration> filter = null)
		{
			_nodeId = nodeId;
			_entityFilter = filter;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			if (configuration.TrySelectNode(_nodeId, out _node) == false)
			{
				throw new ScenarioConfigurationException($"Node not found with id {_nodeId}");
			}
			_locationMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Components.EntityTypes.Subsystem, ItemStorage>();
			_itemMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<TItemType>();
			_entityFilter?.Initialize(ecs, configuration);
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			return _locationMatcherGroup.TryGetMatchingEntity(_node.EntityId, out var locationTuple)
				&& locationTuple.Component2.Items
					.Where(i => i != null)
					.Any(i => i.Item.HasValue 
						&& _itemMatcherGroup.TryGetMatchingEntity(i.Item.Value, out var itemTuple)
						&& (_entityFilter == null || _entityFilter.Evaluate(itemTuple.Entity)));
		}

		public void Dispose()
		{
			_locationMatcherGroup?.Dispose();
			_itemMatcherGroup?.Dispose();
			_entityFilter?.Dispose();
		}
	}
}
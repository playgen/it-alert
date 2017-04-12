using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Evaluators;
using Engine.Systems.Activation.Components;
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
		private readonly ActivationState _activationState;
		private EntityConfig _entityConfig;
		private readonly IEntityFilter<Simulation, SimulationConfiguration> _filter;

		private ComponentMatcherGroup<TItemType, Activation, CurrentLocation> _itemMatcherGroup;

		/// <summary>
		/// Evaluate whether an item of the specified type has been activated optionally at the specidied location
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="filter"></param>
		public ItemTypeIsActivated(EntityConfig entityConfig = null, 
			ActivationState activationState = ActivationState.Active,
			IEntityFilter<Simulation, SimulationConfiguration> filter = null)
		{
			_entityConfig = entityConfig;
			_activationState = activationState;
			_filter = filter;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			_itemMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<TItemType, Activation, CurrentLocation>();
			_filter?.Initialize(ecs, configuration);
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			return _itemMatcherGroup.MatchingEntities.Any(itemTuple =>
				itemTuple.Component2.ActivationState == _activationState
				&& (_entityConfig == null || itemTuple.Component3.Value == _entityConfig.EntityId)
				&& (_filter == null || _filter.Evaluate(itemTuple.Entity)));
		}

		public void Dispose()
		{
			_filter?.Dispose();
			_itemMatcherGroup?.Dispose();
		}
	}
}

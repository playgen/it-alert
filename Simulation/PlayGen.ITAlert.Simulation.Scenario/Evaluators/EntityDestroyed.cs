using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Engine.Components;
using Engine.Entities;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class EntityDisposed<TEntityType> : IEvaluator<Simulation, SimulationConfiguration>
		where TEntityType : class, IEntityType
	{
		private ComponentMatcherGroup<TEntityType> _entityMatcherGroup;

		private readonly IEntityFilter<Simulation, SimulationConfiguration> _entityFilter;

		private readonly AutoResetEvent _disposedHandle = new AutoResetEvent(false);

		public EntityDisposed(IEntityFilter<Simulation, SimulationConfiguration> itemFilter = null)
		{
			_entityFilter = itemFilter;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			_entityMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<TEntityType>();
			_entityMatcherGroup.MatchingEntityRemoved += MatchingEntityRemoved;

			_entityFilter?.Initialize(ecs, configuration);
		}

		private void MatchingEntityRemoved(Entity entity)
		{
			if (_entityFilter?.Evaluate(entity) ?? true)
			{
				_disposedHandle.Set();
			}
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			return _disposedHandle.WaitOne(0);
		}

		public void Dispose()
		{
			_entityMatcherGroup.Dispose();
			_entityFilter?.Dispose();
		}
	}
}

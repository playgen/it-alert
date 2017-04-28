using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Engine.Components;
using Engine.Entities;
using Engine.Evaluators;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class OnEvent<TEvent> : IEvaluator<Simulation, SimulationConfiguration>
		where TEvent : class, IEvent
	{
		private readonly Predicate<TEvent> _eventFilter;

		private readonly AutoResetEvent _eventHandle = new AutoResetEvent(false);

		private IDisposable _eventSubscription;

		public OnEvent(Predicate<TEvent> eventFilter = null)
		{
			_eventFilter = eventFilter;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			if (ecs.TryGetSystem<EventSystem>(out var eventSystem))
			{
				_eventSubscription = eventSystem.Subscribe<TEvent>(OnTEvent);
			}
		}

		private void OnTEvent(TEvent @event)
		{
			if (_eventFilter == null || _eventFilter(@event))
			{
				_eventHandle.Set();
			}
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			return _eventHandle.WaitOne(0);
		}

		public void Dispose()
		{
			_eventSubscription?.Dispose();
			((IDisposable)_eventHandle).Dispose();
		}
	}
}

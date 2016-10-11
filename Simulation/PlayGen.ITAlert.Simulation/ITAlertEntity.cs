using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine;
using PlayGen.Engine.Components;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation
{
	// ReSharper disable once InconsistentNaming
	public abstract class ITAlertEntity<TState> : TickableEntity<ITAlertEntityState, TState>, IITAlertEntity<TState>
		where TState : ITAlertEntityState
	{
		[SyncState(StateLevel.Full)]
		public EntityType EntityType { get; private set; }

		[SyncState(StateLevel.Full)]
		protected ISimulation Simulation { get; private set; }

		protected ITAlertEntity(ISimulation simulation, IEnumerable<IComponent> components, EntityType entityType)
			: base(simulation, components)
		{
			Simulation = simulation;
			EntityType = entityType;
		}

		protected ITAlertEntity(ISimulation simulation, EntityType entityType) 
			: base(simulation)
		{
			Simulation = simulation;
			EntityType = entityType;
		}

		protected ITAlertEntity()
		{
		}
	}
}

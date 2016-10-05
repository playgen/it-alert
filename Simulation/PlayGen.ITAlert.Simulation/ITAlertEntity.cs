using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation
{
	// ReSharper disable once InconsistentNaming
	public abstract class ITAlertEntity<TState> : TickableEntity<TState>, IITAlertEntity<TState>
		where TState : EntityState
	{
		public EntityType EntityType { get; private set; }

		protected ISimulation Simulation { get; private set; }

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

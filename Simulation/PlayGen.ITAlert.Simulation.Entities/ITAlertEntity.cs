using PlayGen.Engine.Components;
using PlayGen.Engine.Entities;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Entities.Interfaces;

namespace PlayGen.ITAlert.Simulation.Entities
{
	// ReSharper disable once InconsistentNaming
	public abstract class ITAlertEntity<TState> : TickableEntity<ITAlertEntityState, TState>, IITAlertEntity<TState>
		where TState : ITAlertEntityState
	{
		[SyncState(StateLevel.Full)]
		public EntityType EntityType { get; private set; }

		[SyncState(StateLevel.Full)]
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

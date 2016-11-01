using Engine.Core.Serialization;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
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

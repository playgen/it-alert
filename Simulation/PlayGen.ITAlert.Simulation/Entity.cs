using System;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation
{
	public abstract class Entity<TState> : EntityBase, IEntity<TState>
		where TState : EntityState
	{

		#region constructors

		protected Entity(ISimulation simulation, EntityType entityType) 
			: base(simulation, entityType)
		{
		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		// TODO: cehck if this needs a different signature
		protected Entity()
		{
			
		}

		#endregion

		public abstract TState GenerateState();

		public override EntityState GetState()
		{
			return GenerateState();
		}

		//public abstract void SetState(TState state, ISimulation simulation);
	}
}

using System;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Interfaces
{
	public interface IEntity : IDisposable
	{

		int Id { get; }

		int CurrentTick { get; }

		/// <summary>
		/// Tick entry point called by Simulation
		/// </summary>
		/// <param name="currentTick"></param>
		void Tick(int currentTick);

		EntityType EntityType { get; }

		EntityState GetState();

		event EventHandler EntityDestroyed;

		void OnDeserialized();
	}

	public interface IEntity<TState> : IEntity
		where TState : EntityState
	{
		/// <summary>
		/// Return the serialized representation of this entity
		/// </summary>
		/// <returns></returns>
		TState GenerateState();

//		void SetState(TState state, ISimulation simulation);
	}
}

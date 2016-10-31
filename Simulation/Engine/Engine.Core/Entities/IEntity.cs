using System;
using PlayGen.Engine.Components;
using PlayGen.Engine.Serialization;

namespace PlayGen.Engine.Entities
{
	// TODO: the dependency on Rx-Main can be broken in Engine.Entites and Simulation.Entities when we upgrade to .NET 4 and IObservable is included in System

	public interface IEntity : IComponentContainer, ISerializable, IDisposable
	{
		int Id { get; }

		event EventHandler EntityDestroyed;
	}

	public interface IEntity<out TGameState> : IEntity
		where TGameState : EntityState
	{
		TGameState GetState();
	}

	public interface IEntity<out TGameState, out TState> : IEntity<TGameState>
		where TState : TGameState
		where TGameState : EntityState
	{
		/// <summary>
		/// Return the serialized representation of this entity
		/// </summary>
		/// <returns></returns>
		TState GenerateState();
	}
}

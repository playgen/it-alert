using System;
using PlayGen.Engine.Components;
using PlayGen.Engine.Serialization;

namespace PlayGen.Engine.Entities
{
	public interface IEntity : ISerializable, IDisposable
	{
		int Id { get; }

		event EventHandler EntityDestroyed;

		IComponentContainer Container { get; }
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

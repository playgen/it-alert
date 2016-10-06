using System;
using PlayGen.Engine.Serialization;

namespace PlayGen.Engine
{
	public interface IEntity : ISerializable, IDisposable
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

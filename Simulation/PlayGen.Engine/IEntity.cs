using System;

namespace PlayGen.Engine
{
	public interface IEntity : IDisposable
	{

		int Id { get; }




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

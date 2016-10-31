namespace PlayGen.Engine.Entities
{
	public interface ITickableEntity : IEntity
	{
		int CurrentTick { get; }

		/// <summary>
		/// Tick entry point called by EntityRegistry
		/// </summary>
		/// <param name="currentTick"></param>
		void Tick(int currentTick);
	}

	public interface ITickableEntity<out TGameState> : IEntity<TGameState>, ITickableEntity
		where TGameState : EntityState
	{
		
	}

	public interface ITickableEntity<out TGameState, out TState> : IEntity<TGameState, TState>, ITickableEntity<TGameState>
		where TState : TGameState
		where TGameState : EntityState
	{
	
	}
}

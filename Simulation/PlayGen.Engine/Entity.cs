namespace PlayGen.Engine
{
	public abstract class Entity<TGameState, TState> : EntityBase<TGameState>, IEntity<TGameState, TState>
		where TState : TGameState
		where TGameState : EntityState
	{

		#region constructors

		protected Entity(IEntityRegistry entityRegistry) 
			: base(entityRegistry)
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

		public override TGameState GetState()
		{
			return GenerateState();
		}
	}
}

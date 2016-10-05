namespace PlayGen.Engine
{
	public abstract class Entity<TState> : EntityBase, IEntity<TState>
		where TState : EntityState
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

		public override EntityState GetState()
		{
			return GenerateState();
		}
	}
}

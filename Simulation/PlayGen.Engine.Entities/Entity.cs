using PlayGen.Engine.Components;

namespace PlayGen.Engine.Entities
{
	public abstract class Entity<TGameState, TState> : EntityBase<TGameState>, IEntity<TGameState, TState>
		where TState : TGameState
		where TGameState : EntityState
	{

		#region constructors
		protected Entity(IEntityRegistry entityRegistry, IComponentContainer componentContainer)
			: base(entityRegistry, componentContainer)
		{
		}


		protected Entity(IEntityRegistry entityRegistry)
			: base(entityRegistry, null)
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

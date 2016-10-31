using PlayGen.Engine.Components;

namespace PlayGen.Engine.Entities
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

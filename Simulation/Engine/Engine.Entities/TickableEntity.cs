using Engine.Core.Entities;
using Engine.Core.Serialization;

namespace Engine.Entities
{
	public abstract class TickableEntity<TGameState, TState> : Entity<TGameState, TState>
		where TState : TGameState
		where TGameState : EntityState
	{
		[SyncState(StateLevel.Differential)]
		public int CurrentTick { get; protected set; }

		protected TickableEntity(IEntityRegistry entityRegistry)
			: base(entityRegistry)
		{
		}

		protected TickableEntity()
		{
		}

		public virtual void Tick(int currentTick)
		{
			if (CurrentTick < currentTick)
			{
				CurrentTick = currentTick;
				OnTick();
			}
		}
		protected abstract void OnTick();
	}
}

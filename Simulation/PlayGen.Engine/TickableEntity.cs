using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine
{
	public abstract class TickableEntity<TState> : Entity<TState>
		where TState : EntityState
	{
		////[SyncState(StateLevel.Minimal)]
		public int CurrentTick { get; protected set; }

		protected TickableEntity(IEntityRegistry entityRegistry) : base(entityRegistry)
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

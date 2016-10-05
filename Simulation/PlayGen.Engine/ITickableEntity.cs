using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine
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

	public interface ITickableEntity<TState> : IEntity<TState>, ITickableEntity
		where TState : EntityState
	{
	
	}
}

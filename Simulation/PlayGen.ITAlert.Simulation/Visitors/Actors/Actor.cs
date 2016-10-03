using System;
using System.Collections.Generic;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Common.Serialization;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Intents;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors 
{
	public abstract class Actor<TState> : Visitor<TState>, IActor
		where TState : EntityState
	{
		//TODO: make this minimal if speed varies
		[SyncState(StateLevel.Setup)]
		public int Speed { get; protected set; }

		[SyncState(StateLevel.Minimal)]
		public SimpleStack<Intent> Intents { get; private set; } = new SimpleStack<Intent>();

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="entityType"></param>
		/// <param name="speed"></param>
		protected Actor(ISimulation simulation, EntityType entityType, int speed)
			: base(simulation, entityType)
		{
			Speed = speed;
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected Actor()
		{
			
		}

		#endregion

		public bool TryGetIntent(out Intent currentIntent)
		{
			if (Intents.Count > 0)
			{
				currentIntent = Intents.Peek();
				return true;
			}
			currentIntent = null;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="intents">Intents in LIFO order</param>
		public void SetIntents(IList<Intent> intents)
		{
			Intents.Clear();
			foreach (var intent in intents)
			{
				Intents.Push(intent);
			}
		}


	}
}

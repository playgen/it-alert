using System;
using System.Collections.Generic;
using Engine.Core.Components;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Visitors.Actors.Intents;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors 
{
	public abstract class Actor<TState> : Visitor<TState>, IActor
		where TState : ITAlertEntityState
	{
		//TODO: make this minimal if speed varies
		[SyncState(StateLevel.Setup)]
		public int MovementSpeed { get; protected set; }

		[SyncState(StateLevel.Differential)]
		public SimpleStack<Intent> Intents { get; private set; } = new SimpleStack<Intent>();

		#region Constructors

		protected Actor(ISimulation simulation, EntityType entityType, int movementSpeed)
			: base(simulation, entityType)
		{
			MovementSpeed = movementSpeed;
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

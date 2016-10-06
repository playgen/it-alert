using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors
{
	public abstract class NpcActor<TState> : Actor<TState>
		where TState : ITAlertEntityState
	{
		[SyncState(StateLevel.Setup)]
		public NpcActorType NpcActorType { get; protected set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="npcActorType"></param>
		/// <param name="speed"></param>
		protected NpcActor(ISimulation simulation, NpcActorType npcActorType, int speed) 
			: base(simulation, EntityType.Npc, speed)
		{
			NpcActorType = npcActorType;
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected NpcActor()
		{
		}
	}
}

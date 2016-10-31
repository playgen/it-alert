using System;
using PlayGen.Engine.Components;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Entities.Interfaces;

namespace PlayGen.ITAlert.Simulation.Entities.Visitors.Actors.Npc
{
	public abstract class NpcActor<TState> : Actor<TState>
		where TState : ITAlertEntityState
	{
		[SyncState(StateLevel.Setup)]
		public NpcActorType NpcActorType { get; protected set; }

		protected NpcActor(ISimulation simulation, IComponentContainer componentContainer, NpcActorType npcActorType, int movementSpeed) 
			: base(simulation, componentContainer, EntityType.Npc, movementSpeed)
		{
			NpcActorType = npcActorType;
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected NpcActor()
		{
		}
	}
}

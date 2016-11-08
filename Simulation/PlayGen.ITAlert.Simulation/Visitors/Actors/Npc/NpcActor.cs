﻿using System;
using Engine.Core.Components;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors.Npc
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
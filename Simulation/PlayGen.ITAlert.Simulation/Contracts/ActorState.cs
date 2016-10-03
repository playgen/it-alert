﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Contracts.Intents;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public class ActorState : EntityState
	{
		public IntentState[] Intents { get; set; }

		public ActorState(int id, EntityType entityType) 
			: base(id, entityType)
		{
		}
	}
}
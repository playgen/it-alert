﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertEntityState : EntityState
	{
		public EntityType EntityType { get; private set; }

		public ITAlertEntityState(int id, EntityType entityType) 
			: base(id)
		{
			EntityType = entityType;
		}
	}
}

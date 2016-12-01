﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Systems
{
	public class ItemActivation : Engine.Systems.System
	{
		public ItemActivation(IComponentRegistry componentRegistry, IEntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
		}
	}
}
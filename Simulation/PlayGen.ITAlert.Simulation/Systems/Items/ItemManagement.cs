using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ItemManagement : Engine.Systems.System
	{
		public ItemManagement(ComponentRegistry componentRegistry, EntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
		}
	}
}

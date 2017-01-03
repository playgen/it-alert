using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Systems
{
	public class SubsystemResources : Engine.Systems.System
	{
		public SubsystemResources(ComponentRegistry componentRegistry, EntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
		}
	}
}

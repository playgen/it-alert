using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	public class SubsystemResources : Engine.Systems.System
	{
		private readonly List<ISubsystemResourceEffect> _resourceEffects;


		public SubsystemResources(ComponentRegistry componentRegistry, EntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Systems.Planning;

namespace PlayGen.ITAlert.Simulation.Systems.Commands
{
	public class PlayerCommandSystem : Engine.Systems.System
	{
		private IntentSystem _intentSystem;

		public PlayerCommandSystem(ComponentRegistry componentRegistry, EntityRegistry entityRegistry, SystemRegistry systemRegistry) 
			: base(componentRegistry, entityRegistry, systemRegistry)
		{
			_intentSystem = systemRegistry.GetSystem<IntentSystem>();
		}
	}
}

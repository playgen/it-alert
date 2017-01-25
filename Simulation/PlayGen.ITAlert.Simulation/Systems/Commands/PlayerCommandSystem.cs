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

		public PlayerCommandSystem(IComponentRegistry componentRegistry, 
			IEntityRegistry entityRegistry, 
			IntentSystem intentSystem) 
			: base(componentRegistry, entityRegistry)
		{
			_intentSystem = intentSystem;
		}
	}
}

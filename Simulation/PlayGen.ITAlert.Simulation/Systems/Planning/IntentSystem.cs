using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components.Common;

namespace PlayGen.ITAlert.Simulation.Systems.Planning
{
	public class IntentSystem : Engine.Systems.System
	{
		private ComponentMatcherGroup _visitorIntentsMatcher;

		public IntentSystem(ComponentRegistry componentRegistry, EntityRegistry entityRegistry, SystemRegistry systemRegistry)
			: base(componentRegistry, entityRegistry, systemRegistry)
		{
			_visitorIntentsMatcher = new ComponentMatcherGroup(new [] { typeof(IntentsProperty)});

		}


	}
}

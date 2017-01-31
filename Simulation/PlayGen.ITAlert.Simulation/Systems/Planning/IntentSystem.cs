using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components.Common;

namespace PlayGen.ITAlert.Simulation.Systems.Planning
{
	public class IntentSystem : Engine.Systems.System
	{
		private ComponentMatcherGroup _visitorIntentsMatcher;

		public IntentSystem(IComponentRegistry componentRegistry, IEntityRegistry entityRegistry)
			: base(componentRegistry, entityRegistry)
		{
			_visitorIntentsMatcher = componentRegistry.CreateMatcherGroup(new [] { typeof(Intents) });

		}


	}
}

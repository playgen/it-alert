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
	public class IntentSystem : ISystem
	{
		private ComponentMatcherGroup _visitorIntentsMatcher;

		public IntentSystem(IMatcherProvider matcherProvider, IEntityRegistry entityRegistry)
		{
			_visitorIntentsMatcher = matcherProvider.CreateMatcherGroup(new [] { typeof(Intents) });

		}


	}
}

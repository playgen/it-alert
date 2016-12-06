using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;
using Engine.Planning;

namespace PlayGen.ITAlert.Simulation.Components.Intents
{
	public class ActivateItemIntent : IIntent
	{
		public Entity Item { get; }
		public Entity Node { get; }

		public ActivateItemIntent(Entity item, Entity node)
		{
			Item = item;
			Node = node;
		}
	}
}

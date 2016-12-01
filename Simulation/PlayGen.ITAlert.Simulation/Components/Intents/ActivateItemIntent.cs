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
		public IEntity Item { get; }
		public IEntity Node { get; }

		public ActivateItemIntent(IEntity item, IEntity node)
		{
			Item = item;
			Node = node;
		}
	}
}

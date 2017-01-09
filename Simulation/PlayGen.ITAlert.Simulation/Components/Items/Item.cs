using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public class Item : IComponent
	{
		public string Name { get; }

		public Item(string name)
		{
			Name = name;
		}
	}
}

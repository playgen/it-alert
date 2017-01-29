using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class Visitors : IComponent
	{
		public List<int> Values { get; set; }

		public Visitors()
		{
			Values = new List<int>();
		}
	}
}

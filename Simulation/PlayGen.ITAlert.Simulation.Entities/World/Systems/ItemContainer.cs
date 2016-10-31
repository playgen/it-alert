using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Entities.Interfaces;
using PlayGen.ITAlert.Simulation.Entities.Visitors.Items;

namespace PlayGen.ITAlert.Simulation.Entities.World.Systems
{
	public class ItemContainer
	{
		public IItem Item { get; set; }

		public bool Enabled { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Components;
using Engine.Entities;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Items
{ 
	public enum OverLimitBehaviour
	{
		Dispose,
		Lock,
	}

	public class ItemStorage : IComponent
	{

		public ItemContainer[] Items { get; set; }

		public int ItemLimit { get; set; }

		public int MaxItems { get; set; }

		public OverLimitBehaviour OverLimitBehaviour { get; set; }
	}
}

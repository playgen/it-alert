using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Configuration;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public class ItemState : EntityState
	{
		public ItemType ItemType { get; }

		public bool Active { get; set; }

		public int ActiveDuration { get; set; }

		public int ActiveTicksRemaining { get; set; }

		public int? Owner { get; set; }

		public int? CurrentNode { get; set; }

		/// <summary>
		/// Can the item be activated at current location?
		/// </summary>
		public bool CanActivate { get; set; }

		public ItemState(int id, ItemType itemType) 
			: base(id, EntityType.Item)
		{
			ItemType = itemType;
		}
	}
}

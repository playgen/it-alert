using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public class PlayerState : ActorState
	{
		/// <summary>
		/// player name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Players colour
		/// </summary>
		public string Colour { get; set; }


		public int? InventoryItem { get; set; }



		public PlayerState(int id) 
			: base(id, EntityType.Player)
		{
		}
	}
}

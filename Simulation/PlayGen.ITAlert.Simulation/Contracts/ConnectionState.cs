using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public class ConnectionState : NodeState
	{
		public int RelativeWeight { get; set; }

		public int Weight { get; set; }

		public int Head { get; set; }
		public int Tail { get; set; }

		public ConnectionState(int id)
			: base(id, EntityType.Connection)
		{
		}
	}
}

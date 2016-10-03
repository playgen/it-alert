using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Common.Serialization;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public abstract class NodeState : EntityState
	{
		[SyncState(StateLevel.Ui)]
		public Dictionary<int, int> VisitorPositions { get; set; }

		protected NodeState(int id, EntityType entityType) 
			: base(id, entityType)
		{
		}
	}
}

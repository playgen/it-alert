using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public abstract class NodeState : ITAlertEntityState
	{
		[SyncState(StateLevel.Ui)]
		public Dictionary<int, int> VisitorPositions { get; set; }

		protected NodeState(int id, EntityType entityType) 
			: base(id, entityType)
		{
		}
	}
}

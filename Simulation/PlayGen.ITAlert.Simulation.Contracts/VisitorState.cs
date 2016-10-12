using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public abstract class VisitorState : ITAlertEntityState
	{
		protected VisitorState(int id, EntityType entityType) 
			: base(id, entityType)
		{
		}
	}
}

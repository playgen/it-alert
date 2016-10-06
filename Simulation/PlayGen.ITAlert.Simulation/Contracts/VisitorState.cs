using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

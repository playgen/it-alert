using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Contracts.Intents
{
	public class IntentState
	{
		public enum IntentAction
		{
			Undefined,
			Move,
			PickupItem,
			PickupItemType,
			Infect
		}

		public IntentAction Action { get; set; }

		public int ActionParameter { get; set; }
	}
}

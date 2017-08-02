using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public class OutputItemContainer : ItemContainer
	{
		public override bool Enabled => true;

		public override bool CanRelease => true;

		public override bool CanCapture(int itemId)
		{
			return false;
		}
	}
}

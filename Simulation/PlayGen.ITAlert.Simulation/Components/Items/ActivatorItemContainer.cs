using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public abstract class ActivatorItemContainer : ItemContainer
	{
		public override bool CanRelease => false;

		public override bool CanCapture(int? itemId = null)
		{
			return false;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public abstract class TargetItemContainer : ItemContainer
	{
		public override bool Enabled => true;

		public override bool CanRelease => Locked == false;

		public bool Locked { get; set; }
	}
}

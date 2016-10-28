using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.World.Systems
{
	public class SystemMemoryPropertyComponent : RangedIntegerPropertyComponent
	{
		#region Constructors

		public SystemMemoryPropertyComponent(IComponentContainer componentContainer, int initialValue, int maxValue)
			: base(componentContainer, "SystemMemory", true, initialValue, 0, maxValue)
		{
		}

		#endregion

		public override void ApplyDelta(int delta)
		{
			Set(Value + delta);
		}
	}
}

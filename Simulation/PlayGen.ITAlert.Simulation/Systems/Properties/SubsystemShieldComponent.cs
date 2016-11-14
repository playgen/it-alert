﻿using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public class SystemShield : RangedIntegerProperty
	{
		#region Constructors

		public SystemShield(IEntity entity, int initialValue, int maxValue)
			: base(entity, initialValue, 0, maxValue)
		{
		}

		#endregion

		public void ApplyDelta(int delta)
		{
			Set(Value + delta);
		}
	}
}

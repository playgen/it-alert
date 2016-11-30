using Engine.Components.Property;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class SystemHealth : RangedIntegerProperty
	{
		#region Constructors

		public SystemHealth(int initialValue, int maxValue)
			: base(initialValue, 0, maxValue)
		{
		}

		#endregion

		public void ApplyDelta(int delta)
		{
			SetValue(Value + delta);
		}
	}
}

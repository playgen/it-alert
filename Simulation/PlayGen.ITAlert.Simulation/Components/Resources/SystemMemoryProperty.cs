using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Resources
{
	public class SystemMemoryProperty : RangedIntegerProperty
	{
		#region Constructors

		public SystemMemoryProperty(int initialValue, int maxValue)
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

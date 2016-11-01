using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public class SystemHealth : RangedIntegerProperty
	{
		#region Constructors

		public SystemHealth(IComponentContainer componentContainer, int initialValue, int maxValue)
			: base(componentContainer, "Health", true, initialValue, 0, maxValue)
		{
		}

		#endregion

		public override void ApplyDelta(int delta)
		{
			Set(Value + delta);
		}
	}
}

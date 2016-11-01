using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public class SystemShield : RangedIntegerProperty
	{
		#region Constructors

		public SystemShield(IComponentContainer componentContainer, int initialValue, int maxValue)
			: base(componentContainer, "Shield", false, initialValue, 0, maxValue)
		{
		}

		#endregion

		public override void ApplyDelta(int delta)
		{
			Set(Value + delta);
		}
	}
}

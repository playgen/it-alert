using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.World.Systems
{
	public class SubsystemShieldComponent : RangedIntegerPropertyComponent
	{
		#region Constructors

		public SubsystemShieldComponent(IComponentContainer componentContainer, int initialValue, int maxValue)
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

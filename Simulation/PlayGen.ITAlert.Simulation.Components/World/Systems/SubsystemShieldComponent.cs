using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Entities.World.Systems.Components
{
	public class SubsystemShieldComponent : RangedIntegerPropertyComponent
	{
		#region Constructors

		public SubsystemShieldComponent(IComponentContainer componentContainer, int initialValue, int maxValue)
			: base(componentContainer, "Shield", initialValue, 0, maxValue)
		{
		}

		#endregion

		public override void ApplyDelta(int delta)
		{
			Set(Value + delta);
		}
	}
}

using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Entities.World.Systems.Components
{
	public class SubsystemHealthComponent : RangedIntegerPropertyComponent
	{
		#region Constructors

		public SubsystemHealthComponent(IComponentContainer componentContainer, int initialValue, int maxValue)
			: base(componentContainer, "Health", initialValue, 0, maxValue)
		{
		}

		#endregion

		public override void ApplyDelta(int delta)
		{
			Set(Value + delta);
		}
	}
}

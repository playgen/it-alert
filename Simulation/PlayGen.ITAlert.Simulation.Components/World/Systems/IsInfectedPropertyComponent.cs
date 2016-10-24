using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.World.Systems
{
	public class IsInfectedPropertyComponent : PropertyComponentBase<bool>
	{
		#region Constructors

		public IsInfectedPropertyComponent(IComponentContainer componentContainer, int initialValue = SimulationConstants.MaxShield)
			: base(componentContainer, "IsInfected", true, false)
		{

		}

		#endregion

		protected override bool GetValue()
		{
			return false;
		}
	}
}

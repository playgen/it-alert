using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.World.Systems
{
	public class IsInfectedPropertyComponent : ReadOnlyPropertyComponentBase<bool>
	{
		#region Constructors

		public IsInfectedPropertyComponent(IComponentContainer componentContainer)
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

using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public class IsInfectedPropertyComponent : ReadOnlyProperty<bool>
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

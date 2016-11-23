using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class IsInfectedPropertyComponent : ReadOnlyProperty<bool>
	{
		#region Constructors

		public IsInfectedPropertyComponent(IEntity entity)
			: base(entity, false)
		{

		}

		#endregion

		protected override bool GetValue()
		{
			return false;
		}
	}
}

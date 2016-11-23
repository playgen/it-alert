using Engine.Components;
using Engine.Components.Property;
using Engine.Core.Components;

namespace PlayGen.ITAlert.Simulation.VisitorsProperty.Properties
{
	[ComponentUsage(typeof(IItem))]
	public class OwnerProperty : Property<IITAlertEntity>
	{
		public OwnerProperty(IEntity entity) 
			: base(container, "Owner", true)
		{
		}
	}
}

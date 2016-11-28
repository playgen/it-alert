using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class OwnerProperty : Property<int>
	{
		public OwnerProperty(IEntity entity) 
			: base(entity)
		{
		}
	}
}

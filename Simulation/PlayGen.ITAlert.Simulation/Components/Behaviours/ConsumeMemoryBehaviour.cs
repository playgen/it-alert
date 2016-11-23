using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	public class ConsumeMemory : Property<int>
	{
		public ConsumeMemory(IEntity entity, string propertyName, bool includeInState, int value) 
			: base(entity, 1)
		{

		}
	}
}

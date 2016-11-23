using Engine.Components.Property;
using Engine.Core.Components;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Behaviours
{
	public class ConsumeMemory : Property<int>
	{
		public ConsumeMemory(IEntity entity, string propertyName, bool includeInState, int value) 
			: base(entity, 1)
		{

		}
	}
}

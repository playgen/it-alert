using Engine.Components.Property;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	public class ConsumeMemory : Property<int>
	{
		public ConsumeMemory(string propertyName, bool includeInState, int value) 
			: base(1)
		{

		}
	}
}

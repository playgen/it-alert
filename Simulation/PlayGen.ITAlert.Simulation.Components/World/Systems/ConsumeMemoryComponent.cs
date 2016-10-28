using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.World.Systems
{
	public class ConsumeMemoryComponent : PropertyComponentBase<int>
	{
		public ConsumeMemoryComponent(IComponentContainer container, string propertyName, bool includeInState, int value) 
			: base(container, "ConsumesMemory", false, 1)
		{

		}
	}
}

using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Systems.Behaviours
{
	public class ConsumeMemoryBehaviour : Property<int>
	{
		public ConsumeMemoryBehaviour(IComponentContainer container, string propertyName, bool includeInState, int value) 
			: base(container, "ConsumesMemory", false, 1)
		{

		}
	}
}

using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Resources
{
	public class ConsumeMemory : Property<int>
	{
		public ConsumeMemory() 
			: base(1)
		{

		}

		public ConsumeMemory(int value) 
			: base(value)
		{
		}
	}
}

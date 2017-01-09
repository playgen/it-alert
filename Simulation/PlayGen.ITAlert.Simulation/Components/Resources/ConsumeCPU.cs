using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.Resources
{
	// ReSharper disable once InconsistentNaming
	public class ConsumeCPU : Property<int>
	{
		public ConsumeCPU() 
			: base(1)
		{

		}

		public ConsumeCPU(int value) 
			: base(value)
		{
		}
	}
}

using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public class SystemMemoryProperty : RangedIntegerProperty
	{
		#region Constructors

		public SystemMemoryProperty(IComponentContainer componentContainer, int initialValue, int maxValue)
			: base(componentContainer, "SystemMemory", true, initialValue, 0, maxValue)
		{
		}

		#endregion

		public override void ApplyDelta(int delta)
		{
			Set(Value + delta);
		}
	}
}

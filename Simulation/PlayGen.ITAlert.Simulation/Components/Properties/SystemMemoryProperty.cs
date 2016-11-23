using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public class SystemMemoryProperty : RangedIntegerProperty
	{
		#region Constructors

		public SystemMemoryProperty(IEntity entity, int initialValue, int maxValue)
			: base(entity, initialValue, 0, maxValue)
		{
		}

		#endregion

		public void ApplyDelta(int delta)
		{
			Set(Value + delta);
		}
	}
}

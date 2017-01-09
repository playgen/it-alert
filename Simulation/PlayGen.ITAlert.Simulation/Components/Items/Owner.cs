using Engine.Components;
using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public class OwnerState : IComponentState
	{
		public int? Id { get; }

		public OwnerState(int? id)
		{
			Id = id;
		}
	}

	public class Owner : Property<int?>, IEmitState
	{
		public IComponentState GetState()
		{
			return new OwnerState(Value);
		}
	}
}

using Engine.Components;
using Engine.Components.Property;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class OwnerState : IComponentState
	{
		public int? Id { get; }

		public OwnerState(int? id)
		{
			Id = id;
		}
	}

	public class Owner : Property<Entity>, IEmitState
	{
		public IComponentState GetState()
		{
			return new OwnerState(Value?.Id);
		}
	}
}

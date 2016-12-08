using Engine.Components;
using Engine.Components.Property;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class Owner : IComponentState
	{
		public int? Id { get; }

		public Owner(int? id)
		{
			Id = id;
		}
	}

	public class OwnerProperty : Property<Entity>, IEmitState
	{
		public IComponentState GetState()
		{
			return new Owner(Value?.Id);
		}
	}
}

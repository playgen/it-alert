using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Components.Property;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	/// <summary>
	/// Much as this appears to break the ECS mould, there needs to be a way for the ui to figure out what kind of entity to draw
	/// and for now this will do, although using flag components and/or prefabName / spriteName type properties could work
	/// </summary>
	public class EntityTypeProperty : ReadOnlyProperty<EntityType>, IEmitState, IComponentState
	{
		public EntityTypeProperty(EntityType value) : base(value)
		{
		}

		public IComponentState GetState()
		{
			return this;
		}
	}
}

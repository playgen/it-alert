using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Components.Property;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Behaviours;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public sealed class VisitorPositionState : Dictionary<int, int>
	{
		 
	}

	public class Visitors : ReadOnlyProperty<Dictionary<int, Entity>>, IEmitState
	{
		public Visitors() 
			: base(new Dictionary<int, Entity>())
		{
		}

		public override void Initialize(Entity entity)
		{
			base.Initialize(entity);
			AddSubscription<EntityDestroyedMessage>(VisitorDestroyed);
		}

		private void VisitorDestroyed(EntityDestroyedMessage entityDestroyedMessage)
		{
			Value.Remove(entityDestroyedMessage.Entity.Id);
		}

		public object GetState()
		{
			return Value.Aggregate(new VisitorPositionState(), (vps, kvp) =>
			{
				var visitorPosition = kvp.Value.GetComponent<VisitorPosition>();
				vps.Add(kvp.Key, visitorPosition.Position);
				return vps;
			});
		}

	}
}

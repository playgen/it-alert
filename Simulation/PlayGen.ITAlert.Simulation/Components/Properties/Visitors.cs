using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;
using Engine.Entities.Messages;
using PlayGen.ITAlert.Simulation.Components.Behaviours;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public sealed class VisitorPositionState : Dictionary<int, int>
	{
		 
	}

	public class Visitors : ReadOnlyProperty<Dictionary<int, IEntity>>, IEmitState<VisitorPositionState>
	{
		#region contructor

		public Visitors(IEntity entity) 
			: base(entity)
		{
			AddSubscription<EntityDestroyedMessage>(VisitorDestroyed);
		}

		private void VisitorDestroyed(EntityDestroyedMessage entityDestroyedMessage)
		{
			Value.Remove(entityDestroyedMessage.Entity.Id);
		}

		#endregion

		public object GetState()
		{
			//return Value.Aggregate(new VisitorPositionState(), (vps, kvp) =>
			//{
			//	vps.Add(kvp.Key, kvp.Value.Position);
			//	return vps;
			//});
			return null;
		}

	}
}

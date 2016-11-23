using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Engine.Components;
using Engine.Components.Behaviour;
using Engine.Components.Property;
using Engine.Core.Entities;
using Engine.Core.Serialization;
using Engine.Entities.Messages;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Layout;
using PlayGen.ITAlert.Simulation.Systems.Messages;
using PlayGen.ITAlert.Simulation.Visitors;
using EdgeDirection = PlayGen.ITAlert.Simulation.Common.EdgeDirection;

namespace PlayGen.ITAlert.Simulation.Systems.Behaviours
{
	public sealed class VisitorPositionState : Dictionary<int, int>
	{
		 
	}

	public class Visitors : ReadOnlyProperty<Dictionary<int, VisitorPosition>>, IEmitState<VisitorPositionState>
	{
		#region contructor

		public Visitors(IEntity entity, int positions) 
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
			return Value.Aggregate(new VisitorPositionState(), (vps, kvp) =>
			{
				vps.Add(kvp.Key, kvp.Value.Position);
				return vps;
			});
		}

	}
}

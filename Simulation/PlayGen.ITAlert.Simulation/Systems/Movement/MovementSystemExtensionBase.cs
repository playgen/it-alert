using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public abstract class MovementSystemExtensionBase : IMovementSystemExtension
	{
		public event AddVisitorToNode VisitorTransition;

		public abstract int[] NodeIds { get; }

		protected ComponentMatcherGroup<VisitorPosition, CurrentLocation, MovementSpeed, Destination> VisitorMatcherGroup;

		private readonly ComponentMatcherGroup<Visitors> _visitorsMatcherGroup;
		
		protected MovementSystemExtensionBase(IMatcherProvider matcherProvider)
		{
			VisitorMatcherGroup = matcherProvider.CreateMatcherGroup<VisitorPosition, CurrentLocation, MovementSpeed, Destination>();
			_visitorsMatcherGroup = matcherProvider.CreateMatcherGroup<Visitors>();
		}

		public abstract void MoveVisitors(int currentTick);

		public abstract void AddVisitorToNode(int nodeId, int visitorId, int sourceId, int initialPosition, int currentTick);

		protected void AddVisitor(int nodeId, Visitors nodeVisitors, int visitorId, int position, int currentTick)
		{
			if (VisitorMatcherGroup.TryGetMatchingEntity(visitorId, out var visitorTuple))
			{
				nodeVisitors.Values.Add(visitorId);
				visitorTuple.Component1.SetPosition(position, currentTick);
				visitorTuple.Component2.Value = nodeId;
				visitorTuple.Entity.EntityDisposing += EntityOnEntityDisposing;
			}
		}

		private void EntityOnEntityDisposing(Entity entity)
		{
			RemoveVisitorFromNode(entity.Id);
		}

		private void RemoveVisitorFromNode(int visitorId)
		{
			if (VisitorMatcherGroup.TryGetMatchingEntity(visitorId, out var visitorTuple))
			{
				if (visitorTuple.Component2.Value.HasValue
					&& _visitorsMatcherGroup.TryGetMatchingEntity(visitorTuple.Component2.Value.Value, out var visitorsTuple))
				{
					RemoveVisitorFromNode(visitorsTuple.Entity.Id, visitorsTuple.Component1, visitorTuple.Entity, visitorTuple.Component2);
				}
			}
		}

		public void RemoveVisitorFromNode(int nodeId, Visitors nodeVisitors, Entity visitor, CurrentLocation visitorLocation)
		{
			nodeVisitors.Values.Remove(visitor.Id);
			visitorLocation.Value = null;
			visitor.EntityDisposed -= EntityOnEntityDisposing;
		}

		protected void OnVisitorTransition(int nodeId, int visitorId, int sourceId, int initialposition, int currenttick)
		{
			VisitorTransition?.Invoke(nodeId, visitorId, sourceId, initialposition, currenttick);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				VisitorMatcherGroup?.Dispose();
				_visitorsMatcherGroup?.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}

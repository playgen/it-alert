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

		protected ComponentMatcherGroup<VisitorPosition, CurrentLocation, MovementSpeed, Intents> VisitorMatcherGroup;
		
		protected MovementSystemExtensionBase(IMatcherProvider matcherProvider)
		{
			VisitorMatcherGroup = matcherProvider.CreateMatcherGroup<VisitorPosition, CurrentLocation, MovementSpeed, Intents>();
		}

		public abstract void MoveVisitors(int currentTick);

		public abstract void AddVisitorToNode(int nodeId, int visitorId, int sourceId, int initialPosition, int currentTick);

		protected void AddVisitor(int nodeId, Visitors nodeVisitors, int visitorId, int position, int currentTick)
		{
			ComponentEntityTuple<VisitorPosition, CurrentLocation, MovementSpeed, Intents> visitorTuple;
			if (VisitorMatcherGroup.TryGetMatchingEntity(visitorId, out visitorTuple))
			{
				nodeVisitors.Values.Add(visitorId);
				visitorTuple.Component1.SetPosition(position, currentTick);
				visitorTuple.Component2.Value = nodeId;
			}
		}

		public void RemoveVisitorFromNode(int nodeId, Visitors nodeVisitors, int visitorId, CurrentLocation visitorLocation)
		{
			nodeVisitors.Values.Remove(visitorId);
		}

		protected void OnVisitorTransition(int nodeId, int visitorId, int sourceId, int initialposition, int currenttick)
		{
			VisitorTransition?.Invoke(nodeId, visitorId, sourceId, initialposition, currenttick);
		}

	}
}

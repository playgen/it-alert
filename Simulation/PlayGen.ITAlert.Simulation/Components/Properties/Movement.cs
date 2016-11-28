using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Components.Property;
using Engine.Core.Entities;
using Engine.Core.Messaging;
using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Components.Messages;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	[ComponentDependency(typeof(Visitors))]
	[ComponentDependency(typeof(GraphNode))]
	public abstract class Movement : Component, IMovementComponent
	{
		protected GraphNode GraphNode;
		protected Visitors Visitors;

		public int Positions { get; set; }

		protected Movement(IEntity entity) 
			: base(entity)
		{
			GraphNode = Entity.GetComponent<GraphNode>();
			Visitors = Entity.GetComponent<Visitors>();
		}

		public abstract void MoveVisitors(int currentTick);
		public void AddVisitor(IEntity visitor)
		{
			AddVisitor(visitor, 0, 0);
		}

		public abstract void AddVisitor(IEntity visitor, IEntity source, int initialPosition, int currentTick);

		protected void AddVisitor(IEntity visitor, int position, int currentTick)
		{
			var visitorPosition = visitor.GetComponent<VisitorPosition>();
			visitorPosition.SetHost(Entity);
			visitorPosition.SetPosition(position, currentTick);

			Visitors.Value.Add(visitor.Id, visitor);

			var visitorEnteredNodeMessage = new VisitorEnteredNodeMessage(MessageScope.Internal, visitor, Entity);
			// notify the visitor it has entered a node
			visitor.OnNext(visitorEnteredNodeMessage);
			// notify other components on this node that a visitor has entered
			Entity.OnNext(visitorEnteredNodeMessage);
		}
	}
}

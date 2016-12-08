using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Components.Property;
using Engine.Entities;
using Engine.Messaging;
using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Components.Messages;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	[ComponentDependency(typeof(Visitors))]
	[ComponentDependency(typeof(VisitorPosition))]
	[ComponentDependency(typeof(GraphNode))]
	public abstract class Movement : Component, IMovementComponent
	{
		protected GraphNode GraphNode;
		protected Visitors Visitors;

		public int Positions { get; set; }

		public override void Initialize(Entity entity)
		{
			base.Initialize(entity);
			GraphNode = Entity.GetComponent<GraphNode>();
			Visitors = Entity.GetComponent<Visitors>();
		}

		public abstract void MoveVisitors(int currentTick);
		public void AddVisitor(Entity visitor)
		{
			AddVisitor(visitor, 0, 0);
		}

		public abstract void AddVisitor(Entity visitor, Entity source, int initialPosition, int currentTick);

		protected void AddVisitor(Entity visitor, int position, int currentTick)
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

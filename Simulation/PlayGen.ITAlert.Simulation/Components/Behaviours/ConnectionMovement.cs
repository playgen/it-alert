using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Core.Entities;
using Engine.Core.Messaging;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Messages;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{

	public class ConnectionMovement : Movement
	{


		public ConnectionMovement(IEntity entity) 
			: base(entity)
		{

		}

		public override void MoveVisitors(int currentTick)
		{
			var movementCost = Entity.GetComponent<MovementCost>()?.Value ?? 1;
			var exitNode = GraphNode.ExitPositions.Single();

			foreach (var visitor in Visitors.Value.Values)
			{
				var movementSpeed = visitor.GetComponent<MovementSpeed>().Value;
				var visitorPosition = visitor.GetComponent<VisitorPosition>();
				var nextPosition = visitorPosition.Position + (movementSpeed / movementCost);

				if (nextPosition >= exitNode.Value)
				{
					var overflow = Math.Max(nextPosition - movementCost, currentTick);

					Visitors.Value.Remove(visitor.Id);

					exitNode.Key.GetComponent<IMovementComponent>().AddVisitor(visitor, Entity, overflow, currentTick);

					continue;
				}

			}
		}



		public override void AddVisitor(IEntity visitor, IEntity source, int initialPosition, int currentTick)
		{
			var position = GraphNode.EntrancePositions[source] + initialPosition;
			AddVisitor(visitor, position, currentTick);
		}


	}
}

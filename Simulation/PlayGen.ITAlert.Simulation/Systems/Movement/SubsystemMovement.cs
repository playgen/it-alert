using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Components.Intents;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class SubsystemMovement : MovementSystemComponentBase
	{
		public override EntityType EntityType => EntityType.Subsystem;

		public override void MoveVisitors(Entity node, int currentTick)
		{
			var graphNode = node.GetComponent<GraphNode>();
			var visitors = node.GetComponent<Visitors>();
			var exitRoutes = node.GetComponent<ExitRoutes>();

			foreach (var visitor in visitors.Value.Values)
			{
				var visitorPosition = visitor.GetComponent<VisitorPosition>();
				var visitorIntents = visitor.GetComponent<IntentsProperty>();

				Entity exitNode = null;

				IIntent visitorIntent;
				if (visitorIntents != null && visitorIntents.TryPeek(out visitorIntent))
				{
					var moveIntent = visitorIntent as MoveIntent;
					if (moveIntent != null)
					{
						exitNode = exitRoutes.Value[moveIntent.Destination];
					}
				}

				var movementSpeed = visitor.GetComponent<MovementSpeed>().Value;
				var nextPosition = (visitorPosition.Position + movementSpeed) % SimulationConstants.SubsystemPositions;

				if (exitNode != null)
				{
					var exitPosition = graphNode.ExitPositions[exitNode];
					var exitAfterTop = exitPosition < visitorPosition.Position;
					var nextPositionAfterTop = nextPosition < visitorPosition.Position;
					var nextPositionAfterExit = nextPosition > exitPosition;

					if (visitorPosition.Position == exitPosition
						|| (exitAfterTop && nextPositionAfterTop && nextPositionAfterExit)
						|| (exitAfterTop == false & nextPositionAfterExit))
					{
						var overflow = Math.Max(nextPosition - exitPosition, 0);

						RemoveVisitorFromNode(node, visitor);

						//exitNode.GetComponent<IMovementSystemComponent>().AddVisitor(visitor, Entity, overflow, currentTick);
						OnVisitorTransition(exitNode, visitor, node, overflow, currentTick);
					}
				}
				else
				{
					visitorPosition.SetPosition(nextPosition, currentTick);
				}
			}
		}

		public override void AddVisitorToNode(Entity node, Entity visitor, Entity source, int initialPosition, int currentTick)
		{
			var graphNode = node.GetComponent<GraphNode>();

			// determine entrance position
			var direction = graphNode.EntrancePositions.ContainsKey(source) 
				? graphNode.EntrancePositions[source].FromPosition(SimulationConstants.SubsystemPositions) 
				: EdgeDirection.North;

			var position = direction.ToPosition(SimulationConstants.SubsystemPositions) + initialPosition;

			AddVisitor(node, visitor, position, currentTick);

		}
	}
}

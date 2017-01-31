﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class ConnectionMovement : MovementSystemExtensionBase
	{
		public override EntityType EntityType => EntityType.Connection;

		public ConnectionMovement(IEntityRegistry entityRegistry)
			: base(entityRegistry)
		{
		}

		public override void MoveVisitors(Entity node, int currentTick)
		{
			var movementCost = node.GetComponent<MovementCost>()?.Value ?? 1;

			var graphNode = node.GetComponent<GraphNode>();
			var exitNode = graphNode.ExitPositions.Single();

			var visitors = node.GetComponent<Visitors>().Values.ToArray();

			foreach (var visitorId in visitors)
			{
				Entity visitor;
				if (EntityRegistry.TryGetEntityById(visitorId, out visitor))
				{
					//TODO handle failed lookups
					var movementSpeed = visitor.GetComponent<MovementSpeed>().Value;
					var visitorPosition = visitor.GetComponent<VisitorPosition>();

					var nextPosition = visitorPosition.PositionFloat + ((float)movementSpeed / movementCost);

					if (nextPosition >= exitNode.Value)
					{
						var overflow = Math.Max((int)nextPosition - exitNode.Value, 0);

						RemoveVisitorFromNode(node, visitor);

						OnVisitorTransition(exitNode.Key, visitor, node, overflow, currentTick);
					}
					else
					{
						visitorPosition.SetPosition(nextPosition, currentTick);
					}
				}
			}
		}

		public override void AddVisitorToNode(Entity node, Entity visitor, Entity source, int initialPosition, int currentTick)
		{
			var graphNode = node.GetComponent<GraphNode>();

			var position = graphNode.EntrancePositions[source.Id] + initialPosition;
			AddVisitor(node, visitor, position, currentTick);
		}
	}
}

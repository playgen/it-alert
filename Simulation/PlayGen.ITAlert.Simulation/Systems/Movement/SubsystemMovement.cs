using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Intents;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class SubsystemMovement : MovementSystemExtensionBase
	{
		public override EntityType EntityType => EntityType.Subsystem;

		public SubsystemMovement(IEntityRegistry entityRegistry)
			: base (entityRegistry)
		{
		}

		public override void MoveVisitors(Entity node, int currentTick)
		{
			var graphNode = node.GetComponent<GraphNode>();
			var visitors = node.GetComponent<Visitors>().Values.ToArray();
			var exitRoutes = node.GetComponent<ExitRoutes>();

			foreach (var visitorId in visitors)
			{
				Entity visitor;
				if (EntityRegistry.TryGetEntityById(visitorId, out visitor))
				{
					var visitorPosition = visitor.GetComponent<VisitorPosition>();

					int? exitNode = null;

					#region movement intent handling

					// TODO: extract this into the intent system?

					IIntent visitorIntent;
					Intents visitorIntents;
					if (visitor.TryGetComponent(out visitorIntents) && visitorIntents.TryPeek(out visitorIntent))
					{
						var moveIntent = visitorIntent as MoveIntent;
						if (moveIntent != null)
						{
							exitNode = exitRoutes[moveIntent.Destination];
						}
					}

					#endregion

					var movementSpeed = visitor.GetComponent<MovementSpeed>().Value;
					var nextPosition = (visitorPosition.Position + movementSpeed) % SimulationConstants.SubsystemPositions;

					if (exitNode != null)
					{
						var exitPosition = graphNode.ExitPositions[exitNode.Value];
						var exitAfterTop = exitPosition < visitorPosition.Position;
						var nextPositionAfterTop = nextPosition < visitorPosition.Position;
						var nextPositionAfterExit = nextPosition > exitPosition;

						if (visitorPosition.Position == exitPosition
							|| (exitAfterTop && nextPositionAfterTop && nextPositionAfterExit)
							|| (exitAfterTop == false & nextPositionAfterExit))
						{
							var overflow = Math.Max(nextPosition - exitPosition, 0);

							RemoveVisitorFromNode(node, visitor);

							//exitNode.GetComponent<IMovementSystemExtension>().AddVisitor(visitor, Entity, overflow, currentTick);
							OnVisitorTransition(exitNode.Value, visitor, node, overflow, currentTick);
						}
						else
						{
							visitorPosition.SetPosition(nextPosition, currentTick);
						}
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

			// determine entrance position
			var direction = graphNode.EntrancePositions.ContainsKey(source.Id) 
				? graphNode.EntrancePositions[source.Id].FromPosition(SimulationConstants.SubsystemPositions) 
				: EdgeDirection.North;

			var position = direction.ToPosition(SimulationConstants.SubsystemPositions) + initialPosition;

			AddVisitor(node, visitor, position, currentTick);

			IIntent visitorIntent;
			var visitorIntents = visitor.GetComponent<Intents>();
			if (visitorIntents != null && visitorIntents.TryPeek(out visitorIntent))
			{
				var moveIntent = visitorIntent as MoveIntent;
				if (moveIntent != null && moveIntent.Destination == node.Id)
				{
					visitorIntents.Pop();
				}
			}
		}
	}
}

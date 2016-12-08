using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Intents;
using PlayGen.ITAlert.Simulation.Components.Messages;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	[ComponentDependency(typeof(IntentsProperty))]
	[ComponentDependency(typeof(MovementSpeed))]
	[ComponentDependency(typeof(ExitRoutes))]
	public class SubsystemMovement : Movement
	{
		private ExitRoutes _exitRoutes;

		public override void Initialize(Entity entity)
		{
			base.Initialize(entity);
			_exitRoutes = Entity.GetComponent<ExitRoutes>();
		}

		public override void MoveVisitors(int currentTick)
		{
			foreach (var visitor in Visitors.Value.Values)
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
						exitNode = _exitRoutes.Value[moveIntent.Destination];
					}
				}

				var movementSpeed = visitor.GetComponent<MovementSpeed>().Value;
				var nextPosition = (visitorPosition.Position + movementSpeed) % SimulationConstants.SubsystemPositions;

				if (exitNode != null)
				{
					var exitPosition = GraphNode.ExitPositions[exitNode];
					var exitAfterTop = exitPosition < visitorPosition.Position;
					var nextPositionAfterTop = nextPosition < visitorPosition.Position;
					var nextPositionAfterExit = nextPosition > exitPosition;

					if (visitorPosition.Position == exitPosition
						|| (exitAfterTop && nextPositionAfterTop && nextPositionAfterExit)
						|| (exitAfterTop == false & nextPositionAfterExit))
					{
						var overflow = Math.Max(nextPosition - exitPosition, 0);

						Visitors.Value.Remove(visitor.Id);

						exitNode.GetComponent<IMovementComponent>().AddVisitor(visitor, Entity, overflow, currentTick);
					}
				}
				else
				{
					visitorPosition.SetPosition(nextPosition, currentTick);
				}
			}
		}

		public override void AddVisitor(Entity visitor, Entity source, int initialPosition, int currentTick)
		{
			// determine entrance position
			var direction = GraphNode.EntrancePositions.ContainsKey(source) 
				? GraphNode.EntrancePositions[source].FromPosition(SimulationConstants.SubsystemPositions) 
				: EdgeDirection.North;

			var position = direction.ToPosition(SimulationConstants.SubsystemPositions) + initialPosition;

			AddVisitor(visitor, position, currentTick);

		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class ConnectionMovement : MovementSystemExtensionBase
	{
		private readonly ComponentMatcherGroup<Connection, GraphNode, Visitors, MovementCost> _connectionMatcherGroup;

		public override int[] NodeIds => _connectionMatcherGroup.MatchingEntityKeys;

		public ConnectionMovement(IMatcherProvider matcherProvider)
			: base (matcherProvider)
		{
			_connectionMatcherGroup = matcherProvider.CreateMatcherGroup<Connection, GraphNode, Visitors, MovementCost>();
		}

		public override void MoveVisitors(int currentTick)
		{
			foreach (var connectionTuple in _connectionMatcherGroup.MatchingEntities)
			{
				var exitNode = connectionTuple.Component2.ExitPositions.Single();

				foreach (var visitorId in connectionTuple.Component3.Values.ToArray())
				{
					ComponentEntityTuple<VisitorPosition, CurrentLocation, MovementSpeed, Intents> visitorTuple;
					if (VisitorMatcherGroup.TryGetMatchingEntity(visitorId, out visitorTuple))
					{
						var nextPosition = visitorTuple.Component1.PositionDecimal + (visitorTuple.Component3.Value / connectionTuple.Component4.Value);

						if (nextPosition >= exitNode.Value)
						{
							var overflow = Math.Max((int)nextPosition - exitNode.Value, 0);

							RemoveVisitorFromNode(connectionTuple.Entity.Id, connectionTuple.Component3, visitorTuple.Entity.Id, visitorTuple.Component2);

							OnVisitorTransition(exitNode.Key, visitorId, connectionTuple.Entity.Id, overflow, currentTick);
						}
						else
						{
							visitorTuple.Component1.SetPosition(nextPosition, currentTick);
						}
					}
				}
			}
		}

		public override void AddVisitorToNode(int nodeId, int visitorId, int sourceId, int initialPosition, int currentTick)
		{
			ComponentEntityTuple<Connection, GraphNode, Visitors, MovementCost> connectionTuple;
			if (_connectionMatcherGroup.TryGetMatchingEntity(nodeId, out connectionTuple))
			{
				var position = connectionTuple.Component2.EntrancePositions.ContainsKey(sourceId)
					? connectionTuple.Component2.EntrancePositions[sourceId] + initialPosition
					: initialPosition;
				AddVisitor(nodeId, connectionTuple.Component3, visitorId, position, currentTick);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Intents;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	/// <summary>
	/// Responsible for Visitor movement on subsystem nodes, this is where Visitors can idle and is a continuous cyclical movement
	/// This is also where Visitor routing takes place
	/// Movement is modulated by the visitor movement speed and node movement cost
	/// </summary>
	public class SubsystemMovement : MovementSystemExtensionBase
	{
		private readonly ComponentMatcherGroup<Subsystem, GraphNode, Visitors, ExitRoutes, MovementCost> _subsystemMatcherGroup;

		public override int[] NodeIds => _subsystemMatcherGroup.MatchingEntityKeys;

		public SubsystemMovement(IMatcherProvider matcherProvider)
			: base (matcherProvider)
		{
			// TODO: this is a good example where creating arbitrarily larger n-tuples might not be the best option
			// the flag components will never be used in the tuple but consume one valuable slot
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, GraphNode, Visitors, ExitRoutes, MovementCost>();
		}
		public override void MoveVisitors(int currentTick)
		{
			foreach (var subsystemTuple in _subsystemMatcherGroup.MatchingEntities)
			{

				foreach (var visitorId in subsystemTuple.Component3.Values.ToArray())
				{
					ComponentEntityTuple<VisitorPosition, CurrentLocation, MovementSpeed, Intents> visitorTuple;
					if (VisitorMatcherGroup.TryGetMatchingEntity(visitorId, out visitorTuple))
					{
						int? exitNode = null;

						#region movement intent handling

						// TODO: extract this into the intent system?

						IIntent visitorIntent;
						if (visitorTuple.Component4.TryPeek(out visitorIntent))
						{
							var moveIntent = visitorIntent as MoveIntent;
							if (moveIntent != null)
							{
								if (moveIntent.Destination == subsystemTuple.Entity.Id)
								{
									visitorTuple.Component4.Pop();
								}
								else
								{
									exitNode = subsystemTuple.Component4[moveIntent.Destination];
								}
							}
						}

						#endregion

						var nextPosition = (visitorTuple.Component1.PositionDecimal + visitorTuple.Component3.Value / Math.Max(1, subsystemTuple.Component5.Value)) % SimulationConstants.SubsystemPositions;

						if (exitNode != null)
						{
							var exitPosition = subsystemTuple.Component2.ExitPositions[exitNode.Value];
							var exitAfterTop = exitPosition < visitorTuple.Component1.Position;
							var nextPositionAfterTop = nextPosition < visitorTuple.Component1.Position;
							var nextPositionAfterExit = nextPosition > exitPosition;

							if (visitorTuple.Component1.Position == exitPosition
								|| (exitAfterTop && nextPositionAfterTop && nextPositionAfterExit)
								|| (exitAfterTop == false & nextPositionAfterExit))
							{
								var overflow = Math.Max((int)nextPosition - exitPosition, 0);

								RemoveVisitorFromNode(subsystemTuple.Entity.Id, subsystemTuple.Component3, visitorTuple.Entity.Id, visitorTuple.Component2);

								//exitNode.GetComponent<IMovementSystemExtension>().AddVisitor(visitor, Entity, overflow, currentTick);
								OnVisitorTransition(exitNode.Value, visitorId, subsystemTuple.Entity.Id, overflow, currentTick);
							}
							else
							{
								visitorTuple.Component1.SetPosition(nextPosition, currentTick);
							}
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
			ComponentEntityTuple<Subsystem, GraphNode, Visitors, ExitRoutes, MovementCost> subsystemTuple;
			if (_subsystemMatcherGroup.TryGetMatchingEntity(nodeId, out subsystemTuple))
			{
				// determine entrance position
				var direction = subsystemTuple.Component2.EntrancePositions.ContainsKey(sourceId)
					? subsystemTuple.Component2.EntrancePositions[sourceId].FromPosition(SimulationConstants.SubsystemPositions)
					: EdgeDirection.North;

				var position = direction.ToPosition(SimulationConstants.SubsystemPositions) + initialPosition;

				AddVisitor(nodeId, subsystemTuple.Component3, visitorId, position, currentTick);

				ComponentEntityTuple<VisitorPosition, CurrentLocation, MovementSpeed, Intents> visitorTuple;
				IIntent visitorIntent;
				if (VisitorMatcherGroup.TryGetMatchingEntity(visitorId, out visitorTuple)
					&& visitorTuple.Component4.TryPeek(out visitorIntent))
				{
					var moveIntent = visitorIntent as MoveIntent;
					if (moveIntent != null && moveIntent.Destination == nodeId)
					{
						visitorTuple.Component4.Pop();
					}
				}
			}
		}
	}
}

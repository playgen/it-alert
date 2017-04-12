using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public delegate void AddVisitorToNode(int nodeId, int visitorId, int sourceId, int initialPosition, int currentTick);

	public interface IMovementSystemExtension : ISystemExtension
	{
		event AddVisitorToNode VisitorTransition;

		int[] NodeIds { get; }

		void MoveVisitors(int currentTick);

		void AddVisitorToNode(int nodeId, int visitorId, int sourceId, int initialPosition, int currentTick);

		void RemoveVisitorFromNode(int nodeId, Visitors nodeVisitors, Entity visitor, CurrentLocation visitorLocation);
	}
}

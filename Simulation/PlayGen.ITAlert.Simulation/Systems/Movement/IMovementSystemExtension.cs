using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public delegate void AddVisitorToNode(int nodeId, Entity visitor, Entity source, int initialPosition, int currentTick);

	public interface IMovementSystemExtension : ISystemExtension
	{
		event AddVisitorToNode VisitorTransition;

		EntityType EntityType { get; }

		void MoveVisitors(Entity node, int currentTick);

		void AddVisitor(Entity node, Entity visitor);

		void AddVisitorToNode(Entity node, Entity visitor, Entity source, int initialPosition, int currentTick);

		void RemoveVisitorFromNode(Entity node, Entity visitor);
	}
}

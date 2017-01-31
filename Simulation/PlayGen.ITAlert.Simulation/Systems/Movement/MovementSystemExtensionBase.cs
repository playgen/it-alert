using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public abstract class MovementSystemExtensionBase : IMovementSystemExtension
	{
		public event AddVisitorToNode VisitorTransition;
		public abstract EntityType EntityType { get; }

		protected readonly IEntityRegistry EntityRegistry;

		protected MovementSystemExtensionBase(IEntityRegistry entityRegistry)
		{
			EntityRegistry = entityRegistry;
		}

		public abstract void MoveVisitors(Entity node, int currentTick);

		public abstract void AddVisitorToNode(Entity node, Entity visitor, Entity source, int initialPosition, int currentTick);

		public void AddVisitor(Entity node, Entity visitor)
		{
			AddVisitor(node, visitor, 0, 0);
		}

		protected void AddVisitor(Entity node, Entity visitor, int position, int currentTick)
		{
			var visitorPosition = visitor.GetComponent<VisitorPosition>();
			visitorPosition.SetHost(node);
			visitorPosition.SetPosition(position, currentTick);

			var visitors = node.GetComponent<Visitors>().Values;
			visitors.Add(visitor.Id);

			// TODO: reimplement - we can use the current location property of the visitor to remove them from the appropriate node
			// visitor.EntityDestroyed += v => RemoveVisitorFromNode(node, v);

			var currentLocation = visitor.GetComponent<CurrentLocation>();
			currentLocation.Value = node.Id;
		}

		public void RemoveVisitorFromNode(Entity node, Entity visitor)
		{
			var visitors = node.GetComponent<Visitors>().Values;
			//visitor.EntityDestroyed -= RemoveVisitor;
			visitors.Remove(visitor.Id);
		}

		protected void OnVisitorTransition(int nodeId, Entity visitor, Entity source, int initialposition, int currenttick)
		{
			VisitorTransition?.Invoke(nodeId, visitor, source, initialposition, currenttick);
		}

	}
}

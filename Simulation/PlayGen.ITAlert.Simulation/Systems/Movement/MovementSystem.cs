using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using Engine.Systems;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Intents;
using PlayGen.ITAlert.Simulation.Components.Movement;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	/// <summary>
	/// Responsible for moving Visitors around the graph
	/// Specific types of movement behaviour (connection, subsystem) are implemented by extensions that handle the relevant logic
	/// </summary>
	public class MovementSystem : ISystem, ITickableSystem
	{
		private readonly List<IMovementSystemExtension> _movementSystems;

		public MovementSystem([InjectOptional] List<IMovementSystemExtension> movementSystemExtensions) // TODO: remove zenject dependency when implicit optional collection paramters is implemented
		{
			_movementSystems = movementSystemExtensions;

			foreach (var entityMovementSystem in _movementSystems)
			{
				entityMovementSystem.VisitorTransition += ValueOnVisitorTransition;
			}
		}

		public void AddVisitor(Entity node, Entity visitor, int initialPosition = 0)
		{
			ExecuteMovementSystemAction(node.Id, system => system.AddVisitorToNode(node.Id, visitor.Id, 0, initialPosition, 0));
		}

		public void AddVisitor(int nodeId, int visitorId)
		{
			ExecuteMovementSystemAction(nodeId, system => system.AddVisitorToNode(nodeId, visitorId, 0, 0, 0));
		}

		private void ValueOnVisitorTransition(int nodeId, int visitorId, int sourceId, int initialPosition, int currentTick)
		{
			ExecuteMovementSystemAction(nodeId, system => system.AddVisitorToNode(nodeId, visitorId, sourceId, initialPosition, currentTick));
		}

		/// <summary>
		/// Movement Tick
		/// Perform the move visitors action on all supported node types
		/// </summary>
		/// <param name="currentTick"></param>
		public void Tick(int currentTick)
		{
			foreach (var movementSystemExtension in _movementSystems)
			{
				movementSystemExtension.MoveVisitors(currentTick);
			}
		}

		/// <summary>
		/// Execute an action using the approproate movement system for the node type
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="action"></param>
		private void ExecuteMovementSystemAction(int nodeId, Action<IMovementSystemExtension> action)
		{
			var movementSystemExtension = _movementSystems.SingleOrDefault(ms => ms.NodeIds.Contains(nodeId));
			if (movementSystemExtension == null)
			{
				throw new MovementException($"No movement system extension found for node id {nodeId}");
			}
			action(movementSystemExtension);
		}

		#region command/movement logic

		// TODO: decide if this is where these really belong

		public static bool TrySetDestination(Entity actor, Entity destination)
		{
			Intents visitorIntents;
			if (actor.TryGetComponent(out visitorIntents))
			{
				visitorIntents.Replace(new MoveIntent(destination.Id));
				return true;
			}
			return false;
		}

		#endregion
	}
}

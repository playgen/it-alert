using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Intents;
using PlayGen.ITAlert.Simulation.Components.Movement;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class MovementSystem : Engine.Systems.System, ITickableSystem
	{
		private readonly Dictionary<EntityType, IMovementSystemExtension> _movementSystems;

		private readonly ComponentMatcherGroup _movementNodesMatcher;

		public MovementSystem(IComponentRegistry componentRegistry, 
			IEntityRegistry entityRegistry,
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
			[InjectOptional] List<IMovementSystemExtension> movementSystemExtensions)
			: base(componentRegistry, entityRegistry)
		{
			_movementSystems = movementSystemExtensions.ToDictionary(k => k.EntityType, v => v);

			foreach (var entityMovementSystem in _movementSystems)
			{
				entityMovementSystem.Value.VisitorTransition += ValueOnVisitorTransition;
			}

			_movementNodesMatcher = componentRegistry.CreateMatcherGroup(new[] { typeof(Visitors), typeof(GraphNode) });
			componentRegistry.RegisterMatcher(_movementNodesMatcher);
		}

		public void AddVisitor(Entity node, Entity visitor)
		{
			ExecuteMovementSystemAction(node, system => system.AddVisitor(node, visitor));
		}

		/// <summary>
		/// Handle a vistitor leaving one node and entering another
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="visitor"></param>
		/// <param name="source"></param>
		/// <param name="initialPosition"></param>
		/// <param name="currentTick"></param>
		private void ValueOnVisitorTransition(int nodeId, Entity visitor, Entity source, int initialPosition, int currentTick)
		{
			Entity node;
			if (EntityRegistry.TryGetEntityById(nodeId, out node))
			{
				ExecuteMovementSystemAction(node, system => system.AddVisitorToNode(node, visitor, source, initialPosition, currentTick));
			}
			else
			{
				// something has gone wrong!
			}
		}

		/// <summary>
		/// Movement Tick
		/// Perform the move visitors action on all supported node types
		/// </summary>
		/// <param name="currentTick"></param>
		public void Tick(int currentTick)
		{
			var nodes = _movementNodesMatcher.MatchingEntities;

			foreach (var node in nodes)
			{
				ExecuteMovementSystemAction(node, system => system.MoveVisitors(node, currentTick));
			}
		}

		/// <summary>
		/// Execute an action using the approproate movement system for the node type
		/// </summary>
		/// <param name="node"></param>
		/// <param name="action"></param>
		private void ExecuteMovementSystemAction(Entity node, Action<IMovementSystemExtension> action)
		{
			var entityType = node.GetComponent<EntityTypeProperty>().Value;
			
			IMovementSystemExtension movementSystemExtension;
			if (_movementSystems.TryGetValue(entityType, out movementSystemExtension))
			{
				action(movementSystemExtension);
			}
			else
			{
				// TODO: log unknown node type
			}

		}

		#region command/movement logic

		// TODO: decide if this is where these really belong

		public static bool TrySetDestination(Entity actor, Entity destination)
		{
			IntentsProperty visitorIntents;
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class MovementSystem : Engine.Systems.System
	{
		private readonly Dictionary<EntityType, IMovementSystemComponent> _movementSystems;

		private readonly ComponentMatcherGroup _movementNodesMatcher;

		public MovementSystem(ComponentRegistry componentRegistry, EntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
			_movementSystems = ModuleLoader.InstantiateTypesImplementing<IMovementSystemComponent>().ToDictionary(k => k.EntityType, v => v);

			foreach (var entityMovementSystem in _movementSystems)
			{
				entityMovementSystem.Value.VisitorTransition += ValueOnVisitorTransition;
			}

			_movementNodesMatcher = new ComponentMatcherGroup(new[] { typeof(Visitors), typeof(GraphNode) });
			componentRegistry.RegisterMatcher(_movementNodesMatcher);
		}

		public void AddVisitor(Entity node, Entity visitor)
		{
			ExecuteMovementSystemAction(node, system => system.AddVisitor(node, visitor));
		}

		/// <summary>
		/// Handle a vistitor leaving one node and entering another
		/// </summary>
		/// <param name="node"></param>
		/// <param name="visitor"></param>
		/// <param name="source"></param>
		/// <param name="initialPosition"></param>
		/// <param name="currentTick"></param>
		private void ValueOnVisitorTransition(Entity node, Entity visitor, Entity source, int initialPosition, int currentTick)
		{
			ExecuteMovementSystemAction(node, system => system.AddVisitorToNode(node, visitor, source, initialPosition, currentTick));
		}

		/// <summary>
		/// Movement Tick
		/// Perform the move visitors action on all supported node types
		/// </summary>
		/// <param name="currentTick"></param>
		public override void Tick(int currentTick)
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
		private void ExecuteMovementSystemAction(Entity node, Action<IMovementSystemComponent> action)
		{
			var entityType = node.GetComponent<EntityTypeProperty>().Value;
			
			IMovementSystemComponent movementSystemComponent;
			if (_movementSystems.TryGetValue(entityType, out movementSystemComponent))
			{
				action(movementSystemComponent);
			}
			else
			{
				// TODO: log unknown node type
			}

		}
	}
}
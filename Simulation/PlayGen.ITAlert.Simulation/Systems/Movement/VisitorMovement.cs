using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class VisitorMovement : Engine.Systems.System
	{
		private readonly Dictionary<string, Movement> _malwareEffects;


		private ComponentMatcherGroup _movementNodesMatcher;

		public VisitorMovement(ComponentRegistry componentRegistry, EntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
			_movementNodesMatcher = new ComponentMatcherGroup(new[] { typeof(Visitors), typeof(GraphNode) });
			componentRegistry.RegisterMatcher(_movementNodesMatcher);
		}

		public override void Tick(int currentTick)
		{
			var nodes = _movementNodesMatcher.MatchingEntities;

			foreach (var node in nodes)
			{
				node.MoveVisitors(currentTick);
			}
		}
	}
}

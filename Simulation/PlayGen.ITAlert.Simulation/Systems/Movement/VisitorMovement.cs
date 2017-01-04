using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Behaviours;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	[ComponentDependency(typeof(IMovementComponent))]
	public class VisitorMovement : Engine.Systems.System
	{
		public VisitorMovement(ComponentRegistry componentRegistry, EntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
		}

		public override void Tick(int currentTick)
		{
			var nodes = ComponentRegistry.GetComponentEntitesImplmenting<IMovementComponent>();

			foreach (var node in nodes)
			{
				node.MoveVisitors(currentTick);
			}
		}
	}
}

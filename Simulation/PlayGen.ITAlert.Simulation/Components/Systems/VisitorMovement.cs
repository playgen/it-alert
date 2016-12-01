using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Behaviours;

namespace PlayGen.ITAlert.Simulation.Components.Systems
{
	[ComponentDependency(typeof(IMovementComponent))]
	public class VisitorMovement : Engine.Systems.System
	{
		public VisitorMovement(IComponentRegistry componentRegistry, IEntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
		}

		public void Tick(int currentTick)
		{
			var nodes = ComponentRegistry.GetComponentEntitesImplmenting<IMovementComponent>();

			foreach (var node in nodes)
			{
				node.Component.MoveVisitors(currentTick);
			}
		}
	}
}

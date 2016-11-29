using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Engine.Components;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Components.Messages;
using PlayGen.ITAlert.Simulation.Components.Properties;
using PlayGen.ITAlert.Simulation.Intents;

namespace PlayGen.ITAlert.Simulation.Systems
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

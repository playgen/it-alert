using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components.Flags;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Layout;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class RouteFindingSystem : Engine.Systems.System, IInitializingSystem
	{
		private readonly ComponentMatcherGroup<GraphNode, Subsystem, ExitRoutes> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<GraphNode, Connection> _connectionMatcherGroup;

		public RouteFindingSystem(IComponentRegistry componentRegistry, IEntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
			_subsystemMatcherGroup = componentRegistry.CreateMatcherGroup<GraphNode, Subsystem, ExitRoutes>();
			_connectionMatcherGroup = componentRegistry.CreateMatcherGroup<GraphNode, Connection>();

			// TODO: handle new match route recalculation
			// TODO: handle graph node events that might affect routing
		}

		public void Initialize()
		{
			var subsystems = _subsystemMatcherGroup.MatchingEntities.ToDictionary(k => k.Entity.Id, v => v);
			var connections = _connectionMatcherGroup.MatchingEntities.ToDictionary(k => k.Entity.Id, v => v);

			PathFinder.GenerateRoutes(subsystems, connections);
		}
	}
}

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
	public class PathFindingSystem : Engine.Systems.System, IInitializingSystem
	{
		private readonly ComponentMatcherGroup<GraphNode, Subsystem, ExitRoutes> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<GraphNode, Connection, MovementCost> _connectionMatcherGroup;

		public PathFindingSystem(IMatcherProvider matcherProvider, IEntityRegistry entityRegistry) 
			: base(matcherProvider, entityRegistry)
		{
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<GraphNode, Subsystem, ExitRoutes>();
			_connectionMatcherGroup = matcherProvider.CreateMatcherGroup<GraphNode, Connection, MovementCost>();

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

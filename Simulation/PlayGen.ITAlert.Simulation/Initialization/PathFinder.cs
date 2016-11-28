using System.Collections.Generic;
using System.Linq;
using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Properties;
using PlayGen.ITAlert.Simulation.Systems;
using Priority_Queue;

namespace PlayGen.ITAlert.Simulation.Initialization
{
	public class PathFinder
	{
		/// <summary>
		/// Generate exit connection lookups for the graph
		/// </summary>
		/// <param name="subsystems"></param>
		/// <returns>Dictionary keyed by id of subsystem of (dictionary keyed by destination of exit id)</returns>
		public static Dictionary<int, Dictionary<int, int>> GenerateRoutes(IList<IEntity> subsystems, IList<IEntity> connections)
		{
			var subsystemsById = subsystems.ToDictionary(k => k.Id, v => v);
			var connectionsById = connections.ToDictionary(k => k.Id, v => v);

			var routesBySource = new Dictionary<int, Dictionary<int, int>>(subsystems.Count);

			foreach (var subsystemKvp in subsystemsById)
			{
				var otherSystems = subsystemsById.Values.Except(new[] {subsystemKvp.Value}).ToList();
				//var routesThisSystem = new Dictionary<ISystem, IConnection[]>(otherSystems.Count);

				foreach (var otherSystem in otherSystems)
				{
					//var directNeightbourConnection = subsystemKvp.Value.ExitNodePositions.Keys
					//	.OfType<Connection>()
					//	.SingleOrDefault(c => c.Tail.Equals(otherSystem));
					//if (directNeightbourConnection != null)
					//{
					//	routesThisSystem.Add(otherSystem, new [] {directNeightbourConnection});
					//}
					//else
					//{
					var paths = FindPaths(subsystemsById, connectionsById, subsystemKvp.Value, otherSystem);

					//TODO: reimplement
					//var exitConnections = paths
					//	.Where(p => p.Nodes.Count > 1) // there is more than just the start node
					//	.Select(p => subsystemsById[p.Nodes.First()]
					//		.GetComponent<ExitPositions>().Value
					//		.Select(en => en.Value.Node)
					//		.Single(c => c.Tail == p.Nodes[1])
					//	);
					//routesThisSystem.Add(otherSystem, exitConnections.ToArray());
					//}
				}

				//routesBySource.Add(subsystemKvp.Key, routesThisSystem);
			}

			return routesBySource;
		}

		/// <summary>
		/// Find the set of optimal paths from one subsystem to another
		/// </summary>
		/// <param name="subsystems"></param>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static List<Path> FindPaths(Dictionary<int, IEntity> subsystems, Dictionary<int, IEntity> connections, IEntity source, IEntity destination)
		{
			var paths = new SimplePriorityQueue<Path>();

			var currentPath = new Path(source.Id);
			paths.Enqueue(currentPath, currentPath.Priority);

			var bestPaths = new List<Path>();
			var bestPath = float.MaxValue;

			while (paths.Count > 0)
			{
				currentPath = paths.Dequeue();

				// current path is more expensive, so we dont care about it
				if (currentPath.IsCheaperThanOrEqualTo(bestPath) == false)
				{
					break;
				}
				
				var entryPoint = currentPath.Nodes.Count > 1
						// get hte entry position from the last but one node into the current node
					? subsystems[currentPath.Nodes[currentPath.Nodes.Count - 1]]
						.GetComponent<EntrancePositions>().Value[currentPath.Nodes[currentPath.Nodes.Count - 2]]
						.FromPosition(SimulationConstants.SubsystemPositions)
					: (EdgeDirection?) null;

				//foreach (var neighbourNode in GetAdjacentNodes(currentPath.Nodes.Last(), entryPoint, connections))
				//{
				//	if (currentPath.HasNode(neighbourNode.System) == false)
				//	{
				//		var newPath = new Path(currentPath);
				//		newPath.AddNode(neighbourNode);

				//		if (neighbourNode.System == destination)
				//		{
				//			if (newPath.IsCheaperThanOrEqualTo(bestPath))
				//			{
				//				if (newPath.IsCheaperThan(bestPath))
				//				{

				//					bestPaths.Clear();
				//				}
				//				bestPath = newPath.Priority;
				//				bestPaths.Add(newPath);
				//			}
				//		}
				//		else if (newPath.IsCheaperThanOrEqualTo(bestPath))
				//		{
				//			paths.Enqueue(newPath, newPath.Priority);
				//		}
				//	}
				//}
			}

			return bestPaths;
		}

		private static List<NeighbourNode> GetAdjacentNodes(IEntity source, EdgeDirection? entryPoint, Dictionary<int, IEntity> connections)
		{
			return source.GetComponent<ExitPositions>().Value
				.Select(connection => new NeighbourNode()
				{
					System = connection.Key,
					ConnectionCost = connections[connection.Key].GetComponent<MovementCost>().Value,
					SystemCost = entryPoint?.PositionsToExit(connection.Value.FromPosition(SimulationConstants.SubsystemPositions)) ?? 0
				})
				.ToList();
		}
	}
}

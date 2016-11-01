using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Common;
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
		/// <returns></returns>
		public static Dictionary<int, Dictionary<ISystem, Connection[]>> GenerateRoutes(Dictionary<int, ISystem> subsystems)
		{
			var routesBySource = new Dictionary<int, Dictionary<Systems.System, Connection[]>>(subsystems.Count);

			foreach (var subsystemKvp in subsystems)
			{
				var otherSystems = subsystems.Values.Except(new[] {subsystemKvp.Value}).ToList();
				var routesThisSystem = new Dictionary<ISystem, IConnection[]>(otherSystems.Count);

				foreach (var otherSystem in otherSystems)
				{
					var directNeightbourConnection = subsystemKvp.Value.ExitNodePositions.Keys
						.OfType<Connection>()
						.SingleOrDefault(c => c.Tail.Equals(otherSystem));
					if (directNeightbourConnection != null)
					{
						routesThisSystem.Add(otherSystem, new [] {directNeightbourConnection});
					}
					else
					{
						var paths = FindPaths(subsystems.Values.ToList(), subsystemKvp.Value, otherSystem);
						var exitConnections = paths
							.Where(p => p.Nodes.Count > 1)
							.Select(p =>p.Nodes.First().ExitNodePositions.Select(en => en.Value.Node).OfType<Connection>().Single(c => c.Tail == p.Nodes[1]));
						routesThisSystem.Add(otherSystem, exitConnections.ToArray());
					}
				}

				routesBySource.Add(subsystemKvp.Key, routesThisSystem);
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
		public static List<Path> FindPaths(List<ISystem> subsystems, ISystem source, ISystem destination)
		{
			var paths = new SimplePriorityQueue<Path>();

			Path currentPath = new Path(source);
			paths.Enqueue(currentPath, currentPath.Priority);

			var bestPaths = new List<Path>();
			float bestPath = float.MaxValue;

			while (paths.Count > 0)
			{
				currentPath = paths.Dequeue();

				// current path is more expensive, so we dont care about it
				if (currentPath.IsCheaperThanOrEqualTo(bestPath) == false)
				{
					break;
				}

				var entryPoint = currentPath.Nodes.Count > 1
					? currentPath.Nodes[currentPath.Nodes.Count - 1].EntranceNodePositions[currentPath.Nodes[currentPath.Nodes.Count - 2].Id].Direction
					: (VertexDirection?) null;

				foreach (var neighbourNode in GetAdjacentNodes(currentPath.Nodes.Last(), entryPoint))
				{
					if (currentPath.HasNode(neighbourNode.System) == false)
					{
						var newPath = new Path(currentPath);
						newPath.AddNode(neighbourNode);

						if (neighbourNode.System == destination)
						{
							if (newPath.IsCheaperThanOrEqualTo(bestPath))
							{
								if (newPath.IsCheaperThan(bestPath))
								{
									bestPaths.Clear();
								}
								bestPath = newPath.Priority;
								bestPaths.Add(newPath);
							}
						}
						else if (newPath.IsCheaperThanOrEqualTo(bestPath))
						{
							paths.Enqueue(newPath, newPath.Priority);
						}
					}
				}
			}

			return bestPaths;
		}

		private static List<NeighbourNode> GetAdjacentNodes(ISystem source, VertexDirection? entryPoint)
		{
			return source.ExitNodePositions.Select(node => node.Value.Node)
				.OfType<Connection>()
				.Select(connection => new NeighbourNode()
				{
					System = connection.Tail,
					ConnectionCost = connection.Weight,
					SystemCost = entryPoint?.PositionsToExit(connection.HeadPosition) ?? 0
				})
				.ToList();
		}
	}
}

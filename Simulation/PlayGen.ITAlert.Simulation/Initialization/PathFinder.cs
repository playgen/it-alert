using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Entities.World;
using PlayGen.ITAlert.Simulation.Entities.World.Systems;
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
		public static Dictionary<int, Dictionary<Subsystem, Connection[]>> GenerateRoutes(Dictionary<int, Subsystem> subsystems)
		{
			var routesBySource = new Dictionary<int, Dictionary<Subsystem, Connection[]>>(subsystems.Count);

			foreach (var subsystemKvp in subsystems)
			{
				var otherSubsystems = subsystems.Values.Except(new[] {subsystemKvp.Value}).ToList();
				var routesThisSubsystem = new Dictionary<Subsystem, Connection[]>(otherSubsystems.Count);

				foreach (var otherSubsystem in otherSubsystems)
				{
					var directNeightbourConnection = subsystemKvp.Value.ExitNodePositions.Keys
						.OfType<Connection>()
						.SingleOrDefault(c => c.Tail.Equals(otherSubsystem));
					if (directNeightbourConnection != null)
					{
						routesThisSubsystem.Add(otherSubsystem, new [] {directNeightbourConnection});
					}
					else
					{
						var paths = FindPaths(subsystems.Values.ToList(), subsystemKvp.Value, otherSubsystem);
						var exitConnections = paths
							.Where(p => p.Nodes.Count > 1)
							.Select(p =>p.Nodes.First().ExitNodePositions.Select(en => en.Value.Node).OfType<Connection>().Single(c => c.Tail == p.Nodes[1]));
						routesThisSubsystem.Add(otherSubsystem, exitConnections.ToArray());
					}
				}

				routesBySource.Add(subsystemKvp.Key, routesThisSubsystem);
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
		public static List<Path> FindPaths(List<Subsystem> subsystems, Subsystem source, Subsystem destination)
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
					if (currentPath.HasNode(neighbourNode.Subsystem) == false)
					{
						var newPath = new Path(currentPath);
						newPath.AddNode(neighbourNode);

						if (neighbourNode.Subsystem == destination)
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

		private static List<NeighbourNode> GetAdjacentNodes(Subsystem source, VertexDirection? entryPoint)
		{
			return source.ExitNodePositions.Select(node => node.Value.Node)
				.OfType<Connection>()
				.Select(connection => new NeighbourNode()
				{
					Subsystem = connection.Tail,
					ConnectionCost = connection.Weight,
					SubsystemCost = entryPoint?.PositionsToExit(connection.HeadPosition) ?? 0
				})
				.ToList();
		}
	}
}

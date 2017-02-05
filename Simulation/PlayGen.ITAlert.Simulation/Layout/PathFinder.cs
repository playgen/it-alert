using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Movement;
using Priority_Queue;

namespace PlayGen.ITAlert.Simulation.Layout
{
	public class PathFinder
	{
		/// <summary>
		/// Generate exit connection lookups for the graph
		/// </summary>
		/// <param name="subsystems"></param>
		/// <param name="connections"></param>
		/// <returns>Dictionary keyed by id of subsystem of (dictionary keyed by destination of exit id)</returns>
		public static void GenerateRoutes(Dictionary<int, ComponentEntityTuple<GraphNode, Subsystem, ExitRoutes>> subsystems, Dictionary<int, ComponentEntityTuple<GraphNode, Connection, MovementCost>> connections)
		{
			foreach (var subsystemKvp in subsystems)
			{
				try
				{
					var otherSystems = subsystems.Values.Except(new[] {subsystemKvp.Value}).ToList();
					// destination: exit node
					var routesThisSystem = subsystemKvp.Value.Component3;

					foreach (var otherSystem in otherSystems)
					{
						try
						{
							// find all of the adjacent systems - this will cause IOEX if there are multiple exits on a connection node
							var directNeightbourConnection = subsystemKvp.Value.Component1.ExitPositions.Keys
								.Where(x => connections.Keys.Contains(x))
								.SingleOrDefault(c => connections[c].Component1.ExitPositions.Single().Key == otherSystem.Entity.Id);

							if (directNeightbourConnection != 0)
							{
								routesThisSystem.Add(otherSystem.Entity.Id, directNeightbourConnection);
							}
							else
							{
								var paths = FindPaths(subsystems, connections, subsystemKvp.Key, otherSystem.Entity.Id);

								var exitConnection = paths
									.Where(p => p.Nodes.Count > 1) // there is more than just the start node
									.Select(p => subsystems[p.Nodes.First()].Component1.ExitPositions.Keys
										.Single(ep => connections[ep].Component1.ExitPositions.Keys.Single() == p.Nodes[1]))
									.First();
								routesThisSystem.Add(otherSystem.Entity.Id, exitConnection);
							}
						}
						catch (Exception ex)
						{
							throw new PathFindingException($"Error attempting to find route from {subsystemKvp.Key} to {otherSystem.Entity.Id}", ex);
						}
					}
				}
				catch (Exception ex)
				{
					throw new PathFindingException($"Error attempting to find routes from {subsystemKvp}", ex);
				}
			}
		}

		/// <summary>
		/// Find the set of optimal paths from one subsystem to another
		/// </summary>
		/// <param name="subsystems"></param>
		/// <param name="connections"></param>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static List<Path> FindPaths(Dictionary<int, ComponentEntityTuple<GraphNode, Subsystem, ExitRoutes>> subsystems, 
			Dictionary<int, ComponentEntityTuple<GraphNode, Connection, MovementCost>> connections, 
			int source, 
			int destination)
		{
			var paths = new SimplePriorityQueue<Path>();

			var currentPath = new Path(source);
			paths.Enqueue(currentPath, currentPath.Priority);

			var bestPaths = new List<Path>();
			var bestPath = float.MaxValue;

			while (paths.Count > 0)
			{
				currentPath = paths.Dequeue();

				// current path is more expensive, and so must all others be since the queue is ordered
				if (currentPath.IsCheaperThanOrEqualTo(bestPath) == false)
				{
					// exit the loop
					break;
				}

				var currentNode = subsystems[currentPath.Nodes[currentPath.Nodes.Count - 1]].Component1;

				EdgeDirection? entryPoint = null;
				if (currentPath.Nodes.Count > 1)
				{
					var previousNode = subsystems[currentPath.Nodes[currentPath.Nodes.Count - 2]].Component1;

					var commonEdge = currentNode.EntrancePositions.Keys.Intersect(previousNode.ExitPositions.Keys).Single();
					entryPoint = currentNode.EntrancePositions[commonEdge].FromPosition(SimulationConstants.SubsystemPositions);
				}

				foreach (var neighbourNode in GetAdjacentNodes(currentNode, entryPoint, connections))
				{
					if (currentPath.HasNode(neighbourNode.NodeId) == false)
					{
						var newPath = new Path(currentPath);
						newPath.AddNode(neighbourNode);

						if (neighbourNode.NodeId == destination)
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

		private static List<NeighbourNode> GetAdjacentNodes(GraphNode source, 
			EdgeDirection? entryPoint,
			Dictionary<int, ComponentEntityTuple<GraphNode, Connection, MovementCost>> connections)
		{
			return source.ExitPositions
				.Select(connection => new NeighbourNode()
				{
					NodeId = connections[connection.Key].Component1.ExitPositions.Keys.Single(),
					ConnectionCost = connections[connection.Key].Component3.Value,
					TraversalCost = entryPoint?.PositionsToExit(connection.Value.FromPosition(SimulationConstants.SubsystemPositions)) ?? 0
				})
				.ToList();
		}
	}
}

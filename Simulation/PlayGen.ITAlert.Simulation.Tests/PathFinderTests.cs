using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Layout;
using PlayGen.ITAlert.Simulation.Startup;

namespace PlayGen.ITAlert.Simulation.Tests
{
	[TestFixture]
	public class PathFinderTests
	{
		public static TestCaseData[] PathTestCases =
		{
			new TestCaseData(
			//
			// 0 > 1
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1)
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, EdgeDirection.East, 1)
				},
				1,
				0,
				new List<int[]>()
				{
				}
				).SetName("negative validation, no route simple")
			,
			new TestCaseData(
				// 0 -> 1
					new List<NodeConfig>()
					{
						new NodeConfig(0),
						new NodeConfig(1)
					},
					new List<EdgeConfig>()
					{
						new EdgeConfig(0, EdgeDirection.East, 1)
					},
					0,
					1,
					new List<int[]>()
					{
						new [] { 0, 1 }
					}
				)
				.SetName("immediate neighbour"),
			new TestCaseData(
			// 0 > 1
			// v   v
			// 2 > 3
			// expected route 0 - 2 - 3
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
					new NodeConfig(3)
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, EdgeDirection.East, 1),
					new EdgeConfig(0, EdgeDirection.South, 2),
					new EdgeConfig(1, EdgeDirection.South, 3),
					new EdgeConfig(2, EdgeDirection.East, 3),
				},
				0,
				3,
				new List<int[]>()
				{
					new [] { 0, 2, 3 },
				})
				.SetName("2x2 square, top left to bottom right, acyclic")
			,
			new TestCaseData(
			// 0 <> 1
			// ^    ^
			// v    v
			// 2 <> 3
			// expected route 0 - 2 - 3
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
					new NodeConfig(3)
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, EdgeDirection.East, 1),
					new EdgeConfig(1, EdgeDirection.West, 0),

					new EdgeConfig(0, EdgeDirection.South, 2),
					new EdgeConfig(2, EdgeDirection.North, 0),

					new EdgeConfig(1, EdgeDirection.South, 3),
					new EdgeConfig(3, EdgeDirection.North, 1),

					new EdgeConfig(2, EdgeDirection.East, 3),
					new EdgeConfig(3, EdgeDirection.West, 2),
				},
				0,
				3,
				new List<int[]>()
				{
					new [] { 0, 2, 3 },
				})
				.SetName("2x2 square, top left to bottom right, cyclic")
			,
			new TestCaseData(
			// square, with cycles
			// 0 <> 1
			// ^    ^
			// v    v
			// 2 <> 3
			// expected route 0 - 2 - 3
				ConfigurationHelper.GenerateGraphNodes(2, 2),
				ConfigurationHelper.GenerateFullyConnectedConfiguration(2, 2, 1),
				0,
				3,
				new List<int[]>()
				{
					new [] { 0, 2, 3 },
				})
				.SetName("2x2 square, top left to bottom right, cyclic, config generator")
			,
			new TestCaseData(
			// parallel routes, with cycles
			// 0 <> 1 <> 4
			// ^    ^    ^
			// v    v    v
			// 2 <> 3 <> 5
			// ^    ^    ^
			// v    v    v
			// 6 <> 7 <> 8
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
					new NodeConfig(3),
					new NodeConfig(4),
					new NodeConfig(5),
					new NodeConfig(6),
					new NodeConfig(7),
					new NodeConfig(8),
				},
				new List<EdgeConfig>()
				{
					// top row
					new EdgeConfig(0, EdgeDirection.East, 1),
					new EdgeConfig(1, EdgeDirection.West, 0),
					new EdgeConfig(1, EdgeDirection.East, 4),
					new EdgeConfig(4, EdgeDirection.West, 1),
					// middle row
					new EdgeConfig(2, EdgeDirection.East, 3),
					new EdgeConfig(3, EdgeDirection.West, 2),
					new EdgeConfig(3, EdgeDirection.East, 5),
					new EdgeConfig(5, EdgeDirection.West, 3),
					// bottom row
					new EdgeConfig(6, EdgeDirection.East, 7),
					new EdgeConfig(7, EdgeDirection.West, 6),
					new EdgeConfig(7, EdgeDirection.East, 8),
					new EdgeConfig(8, EdgeDirection.West, 7),
					// top to middle
					new EdgeConfig(0, EdgeDirection.South, 2),
					new EdgeConfig(2, EdgeDirection.North, 0),
					new EdgeConfig(1, EdgeDirection.South, 3),
					new EdgeConfig(3, EdgeDirection.North, 1),
					new EdgeConfig(4, EdgeDirection.South, 5),
					new EdgeConfig(5, EdgeDirection.North, 4),
					// middle to bottom
					new EdgeConfig(2, EdgeDirection.South, 6),
					new EdgeConfig(6, EdgeDirection.North, 2),
					new EdgeConfig(3, EdgeDirection.South, 7),
					new EdgeConfig(7, EdgeDirection.North, 3),
					new EdgeConfig(5, EdgeDirection.South, 8),
					new EdgeConfig(8, EdgeDirection.North, 5),
				},
				0,
				8,
				new List<int[]>()
				{
					new [] { 0, 2, 6, 7, 8 },
					new [] { 0, 2, 3, 7, 8 },
				})
				.SetName("parallel routes, with cycles")
			,
			new TestCaseData(
			// variable weights
			// 0
			// v
			// 2 2> 1
			// v    v
			// 3 3> 4
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
					new NodeConfig(3),
					new NodeConfig(4),
				},
				new List<EdgeConfig>()
				{
					// middle row
					new EdgeConfig(2, EdgeDirection.East, 1, 2),
					new EdgeConfig(3, EdgeDirection.East, 4, 3),

					new EdgeConfig(0, EdgeDirection.South, 2),
					new EdgeConfig(2, EdgeDirection.South, 3),
					new EdgeConfig(1, EdgeDirection.South, 4),
				},
				0,
				4,
				new List<int[]>()
				{
					new [] { 0, 2, 1, 4 },
				})
				.SetName("2x2 square, variable weights")
			,
			new TestCaseData(
			// single path with early destination
			// 0  > 1  > 4
			//
			//           v
			// 2 <  3 <  5
			//
			// v
			// 6  > 7  > 8
			// expected route 0, 1, 4, 5, 3, 2, 6,
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
					new NodeConfig(3),
					new NodeConfig(4),
					new NodeConfig(5),
					new NodeConfig(6),
					new NodeConfig(7),
					new NodeConfig(8),
				},
				new List<EdgeConfig>()
				{
					// top row
					new EdgeConfig(0, EdgeDirection.East, 1),
					new EdgeConfig(1, EdgeDirection.East, 4),
					// middle row
					new EdgeConfig(3, EdgeDirection.West, 2),
					new EdgeConfig(5, EdgeDirection.West, 3),
					// bottom row
					new EdgeConfig(6, EdgeDirection.East, 7),
					new EdgeConfig(7, EdgeDirection.East, 8),
					// top to middle
					new EdgeConfig(4, EdgeDirection.South, 5),
					// middle to bottom
					new EdgeConfig(2, EdgeDirection.South, 6),
				},
				0,
				6,
				new List<int[]>()
				{
					new [] { 0, 1, 4, 5, 3, 2, 6, },
				})
				.SetName("single path with early destination")
			,
		};

		//[TestCaseSource(nameof(PathTestCases))]
		//public void TestFindPath(List<NodeConfig> nodeConfigs, List<EdgeConfig> edgeConfigs, int startId, int endId, List<int[]> pathSteps)
		//{
		//	var sim = new TestSimulation();
		//	var subsystems = sim.CreateSubsystems(nodeConfigs);
		//	var connections = sim.CreateConnections(subsystems, edgeConfigs);

		//	Assert.That(subsystems, Is.Not.Null, "Subsystems are null");
		//	Assert.That(subsystems.Count, Is.Not.EqualTo(0), "No connections returned");
		//	Assert.That(connections, Is.Not.Null, "Subsystems are null");
		//	Assert.That(connections.Count, Is.Not.EqualTo(0), "No connections returned");
		//	Assert.That(subsystems.ContainsKey(startId), "Source not found in subsystems");
		//	Assert.That(subsystems.ContainsKey(endId), "Destination not found in subsystems");

		//	var paths = PathFinder.FindPaths(subsystems.Values.ToList(), subsystems[startId], subsystems[endId]);
		//	Assert.That(paths, Is.Not.Null, "Paths was null");

		//	if (pathSteps.Count == 0)
		//	{
		//		Assert.That(paths.Count, Is.EqualTo(0), "Paths found where there should be none");
		//	}
		//	else
		//	{
		//		Assert.That(paths.Count, Is.EqualTo(pathSteps.Count), "Path count not equal");

		//		foreach (var path in paths)
		//		{
		//			var matchedPath = pathSteps.SingleOrDefault(p =>
		//			{
		//				for (var i = 0; i < p.Length; i++)
		//				{
		//					var node = subsystems[p[i]];
		//					if (node.Id != path.Nodes[i].Id)
		//					{
		//						return false;
		//					}
		//				}
		//				return true;
		//			});
		//			Assert.NotNull(matchedPath);
		//		}

		//	}
		//}

		//private static readonly TestCaseData[] RouteTestCases = new TestCaseData[]
		//{
		//	new TestCaseData(
		//		SimulationHelper.GenerateGraphNodes(2, 2),
		//		SimulationHelper.GenerateFullyConnectedGrid(2, 2, 1)
		//	).SetName("2x2, verify route count"),
		//};

		//[TestCaseSource(nameof(RouteTestCases))]
		//public void TestGenerateRoutes(List<NodeConfig> nodeConfigs, List<EdgeConfig> edgeConfigs)
		//{
		//	var sim = new TestSimulation();
		//	var subsystems = sim.CreateSubsystems(nodeConfigs);
		//	var connections = sim.CreateConnections(subsystems, edgeConfigs);

		//	Assert.That(subsystems, Is.Not.Null, "Subsystems are null");
		//	Assert.That(subsystems.Count, Is.GreaterThan(0), "No connections returned");
		//	Assert.That(connections, Is.Not.Null, "Subsystems are null");
		//	Assert.That(connections.Count, Is.GreaterThan(0), "No connections returned");

		//	var routes = PathFinder.GenerateRoutes(subsystems);

		//	Assert.That(routes.Count, Is.EqualTo(subsystems.Count), "Route dictionary does not contain correct number of entries");
		//	foreach (var routeDict in routes)
		//	{
		//		Assert.That(routeDict.Value.Count, Is.EqualTo(routes.Count - 1), $"Route dictionary for origin {routeDict.Key} has wrong number of entries");
		//		foreach (var destination in routeDict.Value)
		//		{
		//			Assert.That(destination.Value.Length, Is.GreaterThan(0), $"No exit connection for origin {routeDict.Key} and destination {destination.Key}");
		//		}
		//	}
		//}
	}
}

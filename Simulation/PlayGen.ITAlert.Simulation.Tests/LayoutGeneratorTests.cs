using System.Collections.Generic;
using NUnit.Framework;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Layout;

namespace PlayGen.ITAlert.Simulation.Tests
{
	[TestFixture]
	public class LayoutGeneratorTests
	{
		#region validation

		private static readonly object[] ValidationCases =
		{
			new object[] // no nodes or edges
			{
				new List<NodeConfig>(),
				new List<EdgeConfig>()
			},
			new object[]
			{
				// no edges
				new List<NodeConfig>()
				{
					new NodeConfig(0)
				},
				new List<EdgeConfig>()
			},
			new object[]
			{
				// node nodes
				new List<NodeConfig>(),
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, VertexDirection.Bottom, 0)
				}
			},
			new object[] // duplicate node
			{
				new List<NodeConfig>()
				{
					new NodeConfig(0), new NodeConfig(0)
				},
				new List<EdgeConfig>()
			},
			new object[] // duplicate edge
			{
				new List<NodeConfig>()
				{
					new NodeConfig(0),
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, VertexDirection.Bottom, 0),
					new EdgeConfig(0, VertexDirection.Bottom, 0)
				}
			},
			new object[] // edge to non-existent node
			{
				new List<NodeConfig>()
				{
					new NodeConfig(0),
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, VertexDirection.Bottom, 1),
				}
			},
			new object[] // unconnected node
			{
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, VertexDirection.Bottom, 1),
				}
			},
		};

		[TestCaseSource(nameof(ValidationCases))]
		public void TestValidation(List<NodeConfig> nodeConfigs, List<EdgeConfig> edgeConfigs)
		{
			try
			{
				Assert.Throws< LayoutException>(() => LayoutGenerator.Layout(nodeConfigs, edgeConfigs));
			}
			catch (LayoutException)
			{
				//suppress
			}
		}



		#endregion

		#region layout

		private static readonly TestCaseData[] LayoutCases = new []
		{
			new TestCaseData( // 
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, VertexDirection.Right, 1),
					new EdgeConfig(1, VertexDirection.Right, 2),
				},
				new Dictionary<int, Vector>()
				{
					{0, new Vector(0, 0)},
					{1, new Vector(1, 0)},
					{2, new Vector(2, 0)},
				})
				.SetName("three in a row, left to right"),
			new TestCaseData(
				// 0 - 1 - 2
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, VertexDirection.Right, 1),
					new EdgeConfig(0, VertexDirection.Left, 2),
				},
				new Dictionary<int, Vector>()
				{
					{ 0, new Vector(1, 0) },
					{ 1, new Vector(2, 0) },
					{ 2, new Vector(0, 0) },
				})
				.SetName("three in a row, with shift"),
			new TestCaseData( 
			// 
			// 2 - 0 - 1
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, VertexDirection.Right, 1),
					new EdgeConfig(0, VertexDirection.Bottom, 2),
				},
				new Dictionary<int, Vector>()
				{
					{ 0, new Vector(0, 0) },
					{ 1, new Vector(1, 0) },
					{ 2, new Vector(0, 1) },
				})
				.SetName("three in a row, left to right, with shift"),
			new TestCaseData( 
			// 0 - 1
			// |   |
			// 2 - 3
				new List<NodeConfig>()
				{
					new NodeConfig(0),
					new NodeConfig(1),
					new NodeConfig(2),
					new NodeConfig(3),
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, VertexDirection.Right, 1),
					new EdgeConfig(0, VertexDirection.Bottom, 2),
					new EdgeConfig(1, VertexDirection.Bottom, 3),
					new EdgeConfig(1, VertexDirection.Right, 3),
				},
				new Dictionary<int, Vector>()
				{
					{ 0, new Vector(0, 0) },
					{ 1, new Vector(1, 0) },
					{ 2, new Vector(0, 1) },
					{ 3, new Vector(1, 1) },
				})
				.SetName("square, left to right, top to bottom"),
			new TestCaseData(
			// 
			// 0 - - - 1
			// |       |
			// 2 - 3 - 4
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
					new EdgeConfig(0, VertexDirection.Right, 1),
					new EdgeConfig(0, VertexDirection.Bottom, 2),
					new EdgeConfig(1, VertexDirection.Bottom, 4),
					new EdgeConfig(2, VertexDirection.Right, 3),
					new EdgeConfig(3, VertexDirection.Right, 4),
				},
				new Dictionary<int, Vector>()
				{
					{ 0, new Vector(0, 0) },
					{ 1, new Vector(2, 0) },
					{ 2, new Vector(0, 1) },
					{ 3, new Vector(1, 1) },
					{ 4, new Vector(2, 1) },
				})
				.SetName("rectangle, with double length horizontal edge"),
			new TestCaseData( 
			// 3   0 - 1
			// |   |   |
			// 2   |   4
			// |   |   |
			// 5 - 7 - 6  
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
				},
				new List<EdgeConfig>()
				{
					new EdgeConfig(0, VertexDirection.Right, 1),
					new EdgeConfig(0, VertexDirection.Bottom, 7),
					new EdgeConfig(1, VertexDirection.Bottom, 4),
					new EdgeConfig(4, VertexDirection.Bottom, 6),
					new EdgeConfig(7, VertexDirection.Right, 6),
					new EdgeConfig(7, VertexDirection.Left, 5),
					new EdgeConfig(5, VertexDirection.Top, 2),
					new EdgeConfig(2, VertexDirection.Top, 3),
				},
				new Dictionary<int, Vector>()
				{
					{ 0, new Vector(1, 0) },
					{ 1, new Vector(2, 0) },
					{ 2, new Vector(0, 1) },
					{ 3, new Vector(0, 0) },
					{ 4, new Vector(2, 1) },
					{ 5, new Vector(0, 2) },
					{ 6, new Vector(2, 2) },
					{ 7, new Vector(1, 2) },
				})
				.SetName("rectangle, with double length vertical edge")
		};

		[Test,TestCaseSource(nameof(LayoutCases))]
		public void TestLayout(List<NodeConfig> nodeConfigs, List<EdgeConfig> edgeConfigs, Dictionary<int, Vector> expectedCoordinates)
		{
			var layout = LayoutGenerator.Layout(nodeConfigs, edgeConfigs);

			Assert.That(expectedCoordinates.Count, Is.EqualTo(layout.Vectors.Count), "Coordinate collections lengths not equal");

			foreach (var expectedCoordinate in expectedCoordinates)
			{
				Assert.That(layout.NodeVectors[expectedCoordinate.Key].X, Is.EqualTo(expectedCoordinate.Value.X), $"X value not equal for node {expectedCoordinate.Key}");
				Assert.That(layout.NodeVectors[expectedCoordinate.Key].Y, Is.EqualTo(expectedCoordinate.Value.Y), $"Y value not equal for node {expectedCoordinate.Key}");
			}
		}

	#endregion
	}
}
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Layout
{
	public static class LayoutGenerator
	{
		public static Layout Layout(List<NodeConfig> nodes, List<EdgeConfig> edges)
		{
			var nodeQueue = new Queue<NodeConfig>(nodes);

			var layout = new Layout(nodes.First().Id);

			while (nodeQueue.Count > 0)
			{
				var node = nodeQueue.Dequeue();

				var exitEdges = edges
					.FindAll(e => e.Source == node.Id)
					// return edges to existing nodes first
					.OrderByDescending(ne => layout.IdExists(ne.Destination));

				if (layout.IdExists(node.Id) == false 
					&& exitEdges.All(ee => layout.IdExists(ee.Destination)) == false)
				{
					nodeQueue.Enqueue(node);
					continue;
				}

				foreach (var edge in exitEdges)
				{
					// this node is not in the graph yet
					if (layout.IdExists(node.Id) == false)
					{
						AddToLayout(layout, edge.Destination, edge.SourcePosition.OppositePosition(), node.Id);
					}
					// the destination is not in the graph
					if (layout.IdExists(edge.Destination) == false)
					{
						AddToLayout(layout, node.Id, edge.SourcePosition, edge.Destination);
					}
				}

				//if (layout.IdExists(node.Id) == false)
				//{
				//	throw new LayoutException($"Graph invalid: Node {node.Id} has no connections.");
				//}
			}

			return layout;
		}

		private static Vector PlusX = new Vector(1,0);
		private static Vector PlusY = new Vector(0, 1);
		private static Vector MinusX = new Vector(-1, 0);
		private static Vector MinusY = new Vector(0, -1);

		private static void AddToLayout(Layout layout, int sourceId, Common.EdgeDirection sourcePosition, int destinationId)
		{
			var currentVector = layout.GetVectorById(sourceId);
			var nextVector = currentVector;

			switch (sourcePosition)
			{
				case Common.EdgeDirection.East:
					nextVector = currentVector + PlusX;
					if (layout.VectorExists(nextVector))
					{
						layout.PushCol(nextVector.X);
					}
					break;
				case Common.EdgeDirection.South:
					nextVector = currentVector + PlusY;
					if (layout.VectorExists(nextVector))
					{
						layout.PushRow(nextVector.Y);
					}
					break;
				case Common.EdgeDirection.West:
					nextVector = currentVector + MinusX;
					if (layout.VectorExists(nextVector) || nextVector.X < 0)
					{
						layout.PushCol(currentVector.X);
						nextVector = currentVector;
					}
					break;
				case Common.EdgeDirection.North:
					nextVector = currentVector + MinusY;
					if (layout.VectorExists(nextVector) || nextVector.Y < 0)
					{
						layout.PushRow(currentVector.Y);
						nextVector = currentVector;
					}
					break;
			}
			layout.AddNode(destinationId, nextVector);
		}

	}
}

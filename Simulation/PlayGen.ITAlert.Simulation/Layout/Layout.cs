using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Layout
{
	public class Layout
	{
		public Dictionary<int, Vector> NodeVectors { get; } = new Dictionary<int, Vector>();

		public Dictionary<int, Vector> Vectors => NodeVectors;

		public Layout(int rootNodeId)
		{
			AddNode(rootNodeId, new Vector(0,0));
		}

		public void AddNode(int nodeId, Vector coordinate)
		{
			NodeVectors.Add(nodeId,coordinate);
		}

		public Vector GetVectorById(int id)
		{
			return NodeVectors[id];
		}

		public bool IdExists(int id)
		{
			return NodeVectors.ContainsKey(id);
		}

		public bool VectorExists(Vector coordinate)
		{
			return NodeVectors.ContainsValue(coordinate);
		}

		public void PushCol(int x)
		{
			foreach (var coordinate in NodeVectors.Where(c => c.Value.X >= x).ToList())
			{
				NodeVectors[coordinate.Key] = new Vector(coordinate.Value.X + 1, coordinate.Value.Y);

			}
		}
		public void PushRow(int y)
		{
			foreach (var coordinate in NodeVectors.Where(c => c.Value.Y >= y).ToList())
			{
				NodeVectors[coordinate.Key] = new Vector(coordinate.Value.X, coordinate.Value.Y + 1);
			}
		}

		public void PrintGrid()
		{
			foreach (var coordinate in NodeVectors)
			{
				Console.WriteLine(coordinate.Key + " (" + coordinate.Value.X + ", " + coordinate.Value.Y + ")");
			}
		}
	}
}
using System;
using System.Collections.Generic;

namespace PlayGen.ITAlert.Simulation.Layout
{
	public class Path : IComparable<Path>
	{
		public List<int> Nodes { get; private set; }

		public int ConnectionCost { get; private set; }
		public int SystemCost { get; private set; }

		public Path(int start)
		{
			Nodes = new List<int>() { start };
		}

		public Path(Path path)
		{
			Nodes = new List<int>(path.Nodes);
			ConnectionCost = path.ConnectionCost;
			SystemCost = path.SystemCost;
		}


		public void AddNode(NeighbourNode node)
		{
			ConnectionCost += node.ConnectionCost;
			SystemCost += node.SystemCost;
			Nodes.Add(node.System);
		}

		public bool HasNode(int node)
		{
			return Nodes.Contains(node);
		}

		public int CompareTo(Path other)
		{
			return CompareTo(other.Priority);
		}

		private int CompareTo(float other)
		{
			var diff = (Priority - other);

			return Math.Abs(diff) <= float.Epsilon
				? 0
				: (diff > 0)
					? 1
					: -1;

		}

		public float Priority => (10*ConnectionCost) + (0.1f*SystemCost);

		public bool IsCheaperThan(float otherPriority)
		{
			return CompareTo(otherPriority) < 0;
		}

		public bool IsCheaperThanOrEqualTo(float otherPriority)
		{
			return CompareTo(otherPriority) <= 0;
		}
	}
}


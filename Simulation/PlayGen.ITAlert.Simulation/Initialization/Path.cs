﻿using System;
using System.Collections.Generic;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Initialization
{
	public class Path : IComparable<Path>
	{
		public List<Subsystem> Nodes { get; private set; }

		public int ConnectionCost { get; private set; }
		public int SubsystemCost { get; private set; }

		public Path(Subsystem start)
		{
			Nodes = new List<Subsystem>() { start };
		}

		public Path(Path path)
		{
			Nodes = new List<Subsystem>(path.Nodes);
			ConnectionCost = path.ConnectionCost;
			SubsystemCost = path.SubsystemCost;
		}


		public void AddNode(NeighbourNode node)
		{
			ConnectionCost += node.ConnectionCost;
			SubsystemCost += node.SubsystemCost;
			Nodes.Add(node.Subsystem);
		}

		public bool HasNode(Subsystem node)
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

		public float Priority => (10*ConnectionCost) + (0.1f*SubsystemCost);

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

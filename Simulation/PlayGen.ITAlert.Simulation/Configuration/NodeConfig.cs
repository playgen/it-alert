﻿using System;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class NodeConfig : IEquatable<NodeConfig>
	{
		public int Id { get; }

		public int EntityId { get; set; }

		public NodeType Type { get; }

		public string ArchetypeName { get; set; }
		
		public int X { get; set; }
		public int Y { get; set; }

		public string Name { get; set; }

		public NodeConfig(int id)
		{
			Id = id;
		}

		public bool Equals(NodeConfig other)
		{
			return Id == other?.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return Id.ToString();
		}
	}
}
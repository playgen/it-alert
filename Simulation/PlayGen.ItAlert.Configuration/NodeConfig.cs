﻿using System;

namespace PlayGen.ITAlert.Configuration
{
	public class NodeConfig : IEquatable<NodeConfig>
	{
		public int Id { get; }

		public int EntityId { get; set; }

		public NodeType Type { get; }

		public EnhancementType Enhancement { get; }
		
		public int X { get; set; }
		public int Y { get; set; }

		public string Name { get; set; }

		public NodeConfig(int id, 
			NodeType type = NodeType.Default, 
			EnhancementType enhancement = EnhancementType.None)
		{
			//TODO: allow setting initial health
			Id = id;
			Type = type;
			Enhancement = enhancement;
		}

		public bool Equals(NodeConfig other)
		{
			return Id == other.Id;
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
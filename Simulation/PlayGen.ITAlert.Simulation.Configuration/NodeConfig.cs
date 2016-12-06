using System;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Configuration
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
			NodeType type = NodeType.Subsystem, 
			EnhancementType enhancement = EnhancementType.None)
		{
			//TODO: allow setting initial health
			Id = id;
			Type = type;
			Enhancement = enhancement;
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

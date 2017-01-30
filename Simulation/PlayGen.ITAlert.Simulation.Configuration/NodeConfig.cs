using System;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class NodeConfig : IEquatable<NodeConfig>
	{
		public int Id { get; }

		public int EntityId { get; set; }

		public NodeType Type { get; }

		public string EnhancementName { get; set; }
		
		public int X { get; set; }
		public int Y { get; set; }

		public string Name { get; set; }

		public NodeConfig(int id, 
			string name,
			string enhancementName = null,
			NodeType type = NodeType.Subsystem)
		{
			//TODO: allow setting initial health
			Id = id;
			Name = name;
			EnhancementName = enhancementName;
			EnhancementName = enhancementName;
			Type = type;
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

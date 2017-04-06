using System;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class NodeConfig : IEquatable<NodeConfig>
	{
		public const int IdUnassigned = -1;

		public int Id { get; set; } = IdUnassigned;

		public int EntityId { get; set; }

		public NodeType Type { get; }

		public string Archetype { get; set; }
		
		public int X { get; set; }
		public int Y { get; set; }

		public string Name { get; set; }

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

using System;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class NodeConfig : EntityConfig, IEquatable<NodeConfig>
	{
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
